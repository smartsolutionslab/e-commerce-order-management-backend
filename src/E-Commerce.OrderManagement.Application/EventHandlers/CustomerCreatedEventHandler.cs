using E_Commerce.Common.Infrastructure.Messaging;
using E_Commerce.OrderManagement.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_Commerce.OrderManagement.Application.EventHandlers;

public record CustomerCreatedIntegrationEvent(
    Guid CustomerId,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName);

public class CustomerCreatedEventHandler : INotificationHandler<CustomerCreatedIntegrationEvent>
{
    private readonly ICustomerCacheService _customerCacheService;
    private readonly ILogger<CustomerCreatedEventHandler> _logger;

    public CustomerCreatedEventHandler(
        ICustomerCacheService customerCacheService,
        ILogger<CustomerCreatedEventHandler> logger)
    {
        _customerCacheService = customerCacheService;
        _logger = logger;
    }

    public async Task Handle(CustomerCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing customer created event for customer {CustomerId}", notification.CustomerId);

        // Cache customer data for order processing
        await _customerCacheService.CacheCustomerAsync(new CustomerCacheData
        {
            Id = notification.CustomerId,
            TenantId = notification.TenantId,
            Email = notification.Email,
            FullName = $"{notification.FirstName} {notification.LastName}"
        }, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} cached successfully", notification.CustomerId);
    }
}

public interface ICustomerCacheService
{
    Task CacheCustomerAsync(CustomerCacheData customer, CancellationToken cancellationToken);
    Task<CustomerCacheData?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken);
}

public class CustomerCacheData
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
