namespace Domain.Order;

public class OrderFiles
{
    public Guid Id { get; set; }
    public Order Order { get; set; }
    public Product.File File { get; set; }
}