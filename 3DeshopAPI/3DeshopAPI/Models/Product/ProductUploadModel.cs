using Domain.Product;

namespace _3DeshopAPI.Models.Product
{
    public class ProductUploadModel
    {
        public About About { get; set; }
        public Guid UserId { get; set; }
        public Specifications Specifications { get; set; }
    }
}
