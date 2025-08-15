namespace E_Commerce.OrderManagement.Domain.ValueObjects;

public record OrderId
{
    public Guid Value { get; }

    private OrderId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static OrderId Create(Guid value) => new(value);
    public static OrderId Create(string value) => new(Guid.Parse(value));
    public static OrderId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(OrderId orderId) => orderId.Value;
    public static implicit operator string(OrderId orderId) => orderId.Value.ToString();

    public override string ToString() => Value.ToString();
}
