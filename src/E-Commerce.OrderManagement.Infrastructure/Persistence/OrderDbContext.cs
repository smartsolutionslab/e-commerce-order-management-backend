using E_Commerce.Common.Infrastructure.Persistence;
using E_Commerce.Common.Infrastructure.Services;
using E_Commerce.OrderManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.OrderManagement.Infrastructure.Persistence;

public class OrderDbContext : BaseDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options, ITenantService tenantService, IPublisher publisher)
        : base(options, tenantService, publisher)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
