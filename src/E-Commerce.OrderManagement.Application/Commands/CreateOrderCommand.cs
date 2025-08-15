using E_Commerce.Common.Application.Abstractions;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.ValueObjects;

namespace E_Commerce.OrderManagement.Application.Commands;

public record CreateOrderCommand(
    TenantId TenantId,
    CustomerId CustomerId,
    string Currency,
    List<CreateOrderItemRequest> Items) : ICommand<OrderId>;

public record CreateOrderItemRequest(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);

public record AddOrderItemCommand(
    OrderId OrderId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice) : ICommand;

public record ConfirmOrderCommand(OrderId OrderId) : ICommand;

public record ShipOrderCommand(OrderId OrderId) : ICommand;

public record CancelOrderCommand(OrderId OrderId, string Reason) : ICommand;
