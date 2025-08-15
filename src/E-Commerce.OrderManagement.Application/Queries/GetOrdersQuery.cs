using E_Commerce.Common.Application.Abstractions;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Application.DTOs;

namespace E_Commerce.OrderManagement.Application.Queries;

public record GetOrdersQuery(
    TenantId TenantId,
    int Page = 1,
    int PageSize = 20,
    CustomerId? CustomerId = null,
    string? Status = null) : IQuery<OrdersResponse>;

public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderResponse>;

public record GetOrderItemsQuery(Guid OrderId) : IQuery<List<OrderItemResponse>>;
