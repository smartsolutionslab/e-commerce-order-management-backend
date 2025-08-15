using E_Commerce.Common.Infrastructure.Messaging;
using E_Commerce.OrderManagement.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_Commerce.OrderManagement.Application.EventHandlers;

public record ProductInventoryUpdatedIntegrationEvent(
    Guid ProductId,
    Guid TenantId,
    int OldQuantity,
    int NewQuantity);

public class ProductInventoryUpdatedEventHandler : INotificationHandler<ProductInventoryUpdatedIntegrationEvent>
{
    private readonly IInventoryValidationService _inventoryService;
    private readonly ILogger<ProductInventoryUpdatedEventHandler> _logger;

    public ProductInventoryUpdatedEventHandler(
        IInventoryValidationService inventoryService,
        ILogger<ProductInventoryUpdatedEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task Handle(ProductInventoryUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing inventory update for product {ProductId}: {OldQuantity} -> {NewQuantity}", 
            notification.ProductId, notification.OldQuantity, notification.NewQuantity);

        await _inventoryService.UpdateProductInventoryAsync(
            notification.ProductId, 
            notification.NewQuantity, 
            cancellationToken);

        _logger.LogInformation("Product inventory updated successfully for {ProductId}", notification.ProductId);
    }
}

public interface IInventoryValidationService
{
    Task UpdateProductInventoryAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    Task<bool> ValidateInventoryAsync(Guid productId, int requestedQuantity, CancellationToken cancellationToken);
}
