using E_Commerce.Common.Application.Abstractions;
using E_Commerce.OrderManagement.Application.Commands;
using E_Commerce.OrderManagement.Domain.Entities;
using E_Commerce.OrderManagement.Domain.ValueObjects;
using E_Commerce.OrderManagement.Application.Interfaces;
using MediatR;

namespace E_Commerce.OrderManagement.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderId>>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderId>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = Order.Create(request.TenantId, request.CustomerId, request.Currency);

            foreach (var item in request.Items)
            {
                var productId = ProductId.Create(item.ProductId);
                var unitPrice = Money.Create(item.UnitPrice, request.Currency);
                
                order.AddItem(productId, item.Quantity, unitPrice, item.ProductName);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success(order.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<OrderId>(new Error("CreateOrder.Failed", ex.Message));
        }
    }
}

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;

    public ConfirmOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure(new Error("Order.NotFound", "Order not found"));

            order.Confirm();

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("ConfirmOrder.Failed", ex.Message));
        }
    }
}

public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;

    public ShipOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return Result.Failure(new Error("Order.NotFound", "Order not found"));

            order.Ship();

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("ShipOrder.Failed", ex.Message));
        }
    }
}
