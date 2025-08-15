using Asp.Versioning;
using E_Commerce.OrderManagement.Application.Commands;
using E_Commerce.OrderManagement.Application.Queries;
using E_Commerce.OrderManagement.Application.DTOs;
using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.OrderManagement.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/orders")
            .WithApiVersionSet()
            .HasApiVersion(1, 0)
            .WithTags("Orders")
            .RequireAuthorization();

        // GET /api/v1/orders
        group.MapGet("/", GetOrdersAsync)
            .WithName("GetOrders")
            .WithOpenApi();

        // GET /api/v1/orders/{id}
        group.MapGet("/{id:guid}", GetOrderByIdAsync)
            .WithName("GetOrderById")
            .WithOpenApi();

        // POST /api/v1/orders
        group.MapPost("/", CreateOrderAsync)
            .WithName("CreateOrder")
            .WithOpenApi();

        // PUT /api/v1/orders/{id}/confirm
        group.MapPut("/{id:guid}/confirm", ConfirmOrderAsync)
            .WithName("ConfirmOrder")
            .WithOpenApi();

        // PUT /api/v1/orders/{id}/ship
        group.MapPut("/{id:guid}/ship", ShipOrderAsync)
            .WithName("ShipOrder")
            .WithOpenApi();

        // PUT /api/v1/orders/{id}/cancel
        group.MapPut("/{id:guid}/cancel", CancelOrderAsync)
            .WithName("CancelOrder")
            .WithOpenApi();

        // GET /api/v1/orders/{id}/items
        group.MapGet("/{id:guid}/items", GetOrderItemsAsync)
            .WithName("GetOrderItems")
            .WithOpenApi();

        // POST /api/v1/orders/{id}/items
        group.MapPost("/{id:guid}/items", AddOrderItemAsync)
            .WithName("AddOrderItem")
            .WithOpenApi();
    }

    private static async Task<IResult> GetOrdersAsync(
        [FromServices] IMediator mediator,
        HttpContext context,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? status = null)
    {
        var tenantId = GetTenantId(context);
        if (tenantId == null)
            return Results.BadRequest("Tenant ID is required");

        var query = new GetOrdersQuery(
            tenantId,
            page,
            pageSize,
            customerId.HasValue ? CustomerId.Create(customerId.Value) : null,
            status);

        var result = await mediator.Send(query);

        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetOrderByIdAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await mediator.Send(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> CreateOrderAsync(
        [FromServices] IMediator mediator,
        [FromBody] CreateOrderRequest request,
        HttpContext context)
    {
        var tenantId = GetTenantId(context);
        if (tenantId == null)
            return Results.BadRequest("Tenant ID is required");

        var items = request.Items.Select(i => new CreateOrderItemRequest(
            i.ProductId,
            i.ProductName,
            i.Quantity,
            i.UnitPrice)).ToList();

        var command = new CreateOrderCommand(
            tenantId,
            CustomerId.Create(request.CustomerId),
            request.Currency,
            items);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.Created($"/api/v1/orders/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ConfirmOrderAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        var command = new ConfirmOrderCommand(OrderId.Create(id));
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ShipOrderAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        var command = new ShipOrderCommand(OrderId.Create(id));
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> CancelOrderAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] CancelOrderRequest request)
    {
        var command = new CancelOrderCommand(OrderId.Create(id), request.Reason);
        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetOrderItemsAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id)
    {
        var query = new GetOrderItemsQuery(id);
        var result = await mediator.Send(query);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Error);
    }

    private static async Task<IResult> AddOrderItemAsync(
        [FromServices] IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] AddOrderItemRequest request)
    {
        var command = new AddOrderItemCommand(
            OrderId.Create(id),
            request.ProductId,
            request.ProductName,
            request.Quantity,
            request.UnitPrice);

        var result = await mediator.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result.Error);
    }

    private static TenantId? GetTenantId(HttpContext context)
    {
        var tenantIdHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        return string.IsNullOrEmpty(tenantIdHeader) ? null : TenantId.Create(tenantIdHeader);
    }
}

public record CancelOrderRequest(string Reason);
public record AddOrderItemRequest(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
