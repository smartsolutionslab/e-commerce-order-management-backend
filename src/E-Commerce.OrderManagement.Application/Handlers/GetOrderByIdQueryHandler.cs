using E_Commerce.Common.Application.Abstractions;
using E_Commerce.OrderManagement.Application.DTOs;
using E_Commerce.OrderManagement.Application.Interfaces;
using E_Commerce.OrderManagement.Application.Queries;
using E_Commerce.OrderManagement.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.OrderManagement.Application.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(OrderId.Create(request.OrderId), cancellationToken);
            if (order == null)
                return Result.Failure<OrderResponse>(new Error("Order.NotFound", "Order not found"));

            var orderResponse = new OrderResponse(
                order.Id.Value,
                order.CustomerId.Value,
                order.Status.ToString(),
                order.TotalAmount.Amount,
                order.TotalAmount.Currency,
                order.OrderDate,
                order.ConfirmedAt,
                order.ShippedAt,
                order.DeliveredAt,
                order.CancelledAt,
                order.CancellationReason,
                order.Items.Select(i => new OrderItemResponse(
                    i.Id.Value,
                    i.ProductId.Value,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice.Amount,
                    i.TotalPrice.Amount
                )).ToList()
            );

            return Result.Success(orderResponse);
        }
        catch (Exception ex)
        {
            return Result.Failure<OrderResponse>(new Error("GetOrder.Failed", ex.Message));
        }
    }
}
