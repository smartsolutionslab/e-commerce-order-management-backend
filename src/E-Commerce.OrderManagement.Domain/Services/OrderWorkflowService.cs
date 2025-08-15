using E_Commerce.OrderManagement.Domain.Entities;

namespace E_Commerce.OrderManagement.Domain.Services;

public interface IOrderWorkflowService
{
    bool CanTransitionTo(OrderStatus currentStatus, OrderStatus targetStatus);
    List<OrderStatus> GetAllowedTransitions(OrderStatus currentStatus);
}

public class OrderWorkflowService : IOrderWorkflowService
{
    private readonly Dictionary<OrderStatus, List<OrderStatus>> _allowedTransitions = new()
    {
        { OrderStatus.Draft, new List<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
        { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
        { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
        { OrderStatus.Delivered, new List<OrderStatus>() }, // Terminal state
        { OrderStatus.Cancelled, new List<OrderStatus>() }  // Terminal state
    };

    public bool CanTransitionTo(OrderStatus currentStatus, OrderStatus targetStatus)
    {
        return _allowedTransitions.ContainsKey(currentStatus) && 
               _allowedTransitions[currentStatus].Contains(targetStatus);
    }

    public List<OrderStatus> GetAllowedTransitions(OrderStatus currentStatus)
    {
        return _allowedTransitions.ContainsKey(currentStatus) 
            ? _allowedTransitions[currentStatus] 
            : new List<OrderStatus>();
    }
}
