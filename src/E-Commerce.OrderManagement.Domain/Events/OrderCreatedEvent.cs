using E_Commerce.Common.Domain.Primitives;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.ValueObjects;

namespace E_Commerce.OrderManagement.Domain.Events;

public record OrderCreatedEvent(
    OrderId OrderId,
    TenantId TenantId,
    CustomerId CustomerId,
    DateTime OrderDate) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderConfirmedEvent(
    OrderId OrderId,
    TenantId TenantId,
    CustomerId CustomerId,
    Money TotalAmount) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderShippedEvent(
    OrderId OrderId,
    TenantId TenantId,
    CustomerId CustomerId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderDeliveredEvent(
    OrderId OrderId,
    TenantId TenantId,
    CustomerId CustomerId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderCancelledEvent(
    OrderId OrderId,
    TenantId TenantId,
    CustomerId CustomerId,
    string Reason) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderItemAddedEvent(
    OrderId OrderId,
    TenantId TenantId,
    ProductId ProductId,
    int Quantity) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record OrderItemRemovedEvent(
    OrderId OrderId,
    TenantId TenantId,
    ProductId ProductId) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
