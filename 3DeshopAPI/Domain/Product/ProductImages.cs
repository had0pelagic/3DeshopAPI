namespace Domain.Product;

public class ProductImages
{
    public Guid Id { get; set; }
    public Product Product { get; set; }
    public Image Image { get; set; }
}