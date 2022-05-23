namespace Domain.Product;

public class Comment
{
    public Guid Id { get; set; }
    public User? User { get; set; }
    public Product Product { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; }
}