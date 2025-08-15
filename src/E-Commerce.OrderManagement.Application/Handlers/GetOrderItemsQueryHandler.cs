using E_Commerce.Common.Application.Abstractions;
using E_Commerce.OrderManagement.Application.DTOs;
using E_Commerce.OrderManagement.Application.Interfaces;
using E_Commerce.OrderManagement.Application.Queries;
using E_Commerce.OrderManagement.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.OrderManagement.Application.Handlers;

public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, Result<List<OrderItemResponse>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderItemsQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderItemResponse>>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(OrderId.Create(request.OrderId), cancellationToken);
            if (order == null)
                return Result.Failure<List<OrderItemResponse>>(new Error("Order.NotFound", "Order not found"));

            var orderItems = order.Items.Select(i => new OrderItemResponse(
                i.Id.Value,
                i.ProductId.Value,
                i.ProductName,
                i.Quantity,
                i.UnitPrice.Amount,
                i.TotalPrice.Amount
            )).ToList();

            return Result.Success(orderItems);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<OrderItemResponse>>(new Error("GetOrderItems.Failed", ex.Message));
        }
    }
}
