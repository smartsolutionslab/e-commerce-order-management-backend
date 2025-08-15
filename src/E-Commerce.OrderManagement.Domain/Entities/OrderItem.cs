using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.ValueObjects;

namespace E_Commerce.OrderManagement.Domain.Entities;

public class OrderItem : Entity<OrderItemId>
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }

    private OrderItem(OrderItemId id, TenantId tenantId, ProductId productId, int quantity, Money unitPrice, string productName)
        : base(id, tenantId)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    private OrderItem() : base() { } // For EF

    public static OrderItem Create(ProductId productId, int quantity, Money unitPrice, string productName)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        return new OrderItem(
            OrderItemId.NewId(),
            TenantId.NewId(), // Will be set by Order aggregate
            productId,
            quantity,
            unitPrice,
            productName);
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = UnitPrice.Multiply(newQuantity);
        MarkAsUpdated();
    }
}

public record OrderItemId
{
    public Guid Value { get; }

    private OrderItemId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderItemId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static OrderItemId Create(Guid value) => new(value);
    public static OrderItemId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(OrderItemId orderItemId) => orderItemId.Value;
    public override string ToString() => Value.ToString();
}

public record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static ProductId Create(Guid value) => new(value);
    public static ProductId Create(string value) => new(Guid.Parse(value));
    public static ProductId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(ProductId productId) => productId.Value;
    public override string ToString() => Value.ToString();
}
