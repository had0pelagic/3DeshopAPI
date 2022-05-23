namespace Domain.Product;

public class Product
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public About About { get; set; }
    public Specifications Specifications { get; set; }
}