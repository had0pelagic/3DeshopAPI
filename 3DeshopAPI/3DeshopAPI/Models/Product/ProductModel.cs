using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Product;

public class ProductModel
{
    public UserDisplayModel User { get; set; }
    public ProductAboutModel About { get; set; }
    public ProductSpecificationsModel Specifications { get; set; }
    public List<ProductCategoryModel> Categories { get; set; }
    public List<ProductFormatModel> Formats { get; set; }
    public List<ImageModel> Images { get; set; }
    public bool IsBoughtByUser { get; set; }
}