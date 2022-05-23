namespace Domain.Product;

public class About
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime UploadDate { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public int? Downloads { get; set; }
    public string? VideoLink { get; set; } = null;
    public bool IsActive { get; set; } = true;
}