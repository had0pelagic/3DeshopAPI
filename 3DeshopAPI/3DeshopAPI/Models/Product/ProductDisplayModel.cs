using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Product;

public class ProductDisplayModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserDisplayModel User { get; set; }
    public double Price { get; set; }
    public int? Downloads { get; set; }
    public ImageModel Image { get; set; }
    public List<ProductCategoryModel> Categories { get; set; }
    public bool IsActive { get; set; } = true;
}