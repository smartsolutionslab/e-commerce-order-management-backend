using E_Commerce.Common.Application.Abstractions;
using E_Commerce.OrderManagement.Application.DTOs;
using E_Commerce.OrderManagement.Application.Interfaces;
using MediatR;

namespace E_Commerce.OrderManagement.Application.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<OrdersResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrdersResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
            var totalCount = await _orderRepository.GetTotalCountAsync(cancellationToken);

            var orderResponses = orders.Select(o => new OrderResponse(
                o.Id.Value,
                o.CustomerId.Value,
                o.Status.ToString(),
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.OrderDate,
                o.ConfirmedAt,
                o.ShippedAt,
                o.DeliveredAt,
                o.CancelledAt,
                o.CancellationReason,
                o.Items.Select(i => new OrderItemResponse(
                    i.Id.Value,
                    i.ProductId.Value,
                    i.ProductName,
                    i.Quantity,
                    i.UnitPrice.Amount,
                    i.TotalPrice.Amount
                )).ToList()
            )).ToList();

            return Result.Success(new OrdersResponse(orderResponses, totalCount, request.Page, request.PageSize));
        }
        catch (Exception ex)
        {
            return Result.Failure<OrdersResponse>(new Error("GetOrders.Failed", ex.Message));
        }
    }
}
