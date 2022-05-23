using Domain.Product;

namespace Domain.Order;

public class OrderImages
{
    public Guid Id { get; set; }
    public Order Order { get; set; }
    public Image Image { get; set; }
}