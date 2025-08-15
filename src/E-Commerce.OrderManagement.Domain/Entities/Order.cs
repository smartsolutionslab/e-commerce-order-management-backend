using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.Events;

namespace E_Commerce.OrderManagement.Domain.Entities;

public sealed class Order : Entity<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    
    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order(OrderId id, TenantId tenantId, CustomerId customerId, string currency)
        : base(id, tenantId)
    {
        CustomerId = customerId;
        Status = OrderStatus.Draft;
        Currency = currency;
        TotalAmount = Money.Zero(currency);
        OrderDate = DateTime.UtcNow;
    }

    private Order() : base() { } // For EF

    public static Order Create(TenantId tenantId, CustomerId customerId, string currency = "USD")
    {
        var order = new Order(OrderId.NewId(), tenantId, customerId, currency);
        
        order.RaiseDomainEvent(new OrderCreatedEvent(
            order.Id,
            order.TenantId,
            order.CustomerId,
            order.OrderDate));

        return order;
    }

    public void AddItem(ProductId productId, int quantity, Money unitPrice, string productName)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify confirmed order");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice.Currency != Currency)
            throw new ArgumentException($"Unit price currency {unitPrice.Currency} doesn't match order currency {Currency}");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = OrderItem.Create(productId, quantity, unitPrice, productName);
            _items.Add(orderItem);
        }

        RecalculateTotal();
        MarkAsUpdated();

        RaiseDomainEvent(new OrderItemAddedEvent(Id, TenantId, productId, quantity));
    }

    public void RemoveItem(ProductId productId)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify confirmed order");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
            MarkAsUpdated();

            RaiseDomainEvent(new OrderItemRemovedEvent(Id, TenantId, productId));
        }
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException($"Cannot confirm order in {Status} status");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm order without items");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        MarkAsUpdated();

        RaiseDomainEvent(new OrderConfirmedEvent(Id, TenantId, CustomerId, TotalAmount));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException($"Cannot ship order in {Status} status");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        MarkAsUpdated();

        RaiseDomainEvent(new OrderShippedEvent(Id, TenantId, CustomerId));
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException($"Cannot deliver order in {Status} status");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        MarkAsUpdated();

        RaiseDomainEvent(new OrderDeliveredEvent(Id, TenantId, CustomerId));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel delivered order");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Order is already cancelled");

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        MarkAsUpdated();

        RaiseDomainEvent(new OrderCancelledEvent(Id, TenantId, CustomerId, reason));
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Aggregate(
            Money.Zero(Currency),
            (total, item) => total.Add(item.TotalPrice));
    }
}

public enum OrderStatus
{
    Draft,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}
