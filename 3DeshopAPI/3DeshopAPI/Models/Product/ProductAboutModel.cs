namespace _3DeshopAPI.Models.Product;

public class ProductAboutModel
{
    public string Name { get; set; }
    public DateTime? UploadDate { get; set; } = DateTime.Now;
    public double Price { get; set; }
    public string Description { get; set; }
    public int? Downloads { get; set; }
    public string? VideoLink { get; set; } = null;
    public bool IsActive { get; set; } = true;
}