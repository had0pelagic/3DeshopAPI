using Domain.Product;

namespace _3DeshopAPI.Models.Product
{
    public class ProductUploadModel
    {
        public Guid UserId { get; set; }
        public ProductAboutModel About { get; set; }
        public ProductSpecificationsModel Specifications { get; set; }
        public List<Guid> Categories { get; set; }
        public List<Guid> Formats { get; set; }
        public List<ProductImageModel> Images { get; set; }
    }
}
