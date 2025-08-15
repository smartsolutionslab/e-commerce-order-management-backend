using E_Commerce.Common.Domain.ValueObjects;
using E_Commerce.OrderManagement.Domain.Entities;
using E_Commerce.OrderManagement.Domain.ValueObjects;

namespace E_Commerce.OrderManagement.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
    void Remove(Order order);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
