using Domain.Product;

namespace _3DeshopAPI.Models.Product
{
    public class ProductModel
    {
        public About About { get; set; }
        public Domain.User User { get; set; }
        public Specifications Specifications { get; set; }
    }
}
