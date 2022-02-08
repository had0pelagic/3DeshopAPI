using Domain.Product;

namespace _3DeshopAPI.Models.Product
{
    public class ProductModel
    {
        /// <summary>
        /// Model used to return full product data (when opening product on the web)
        /// </summary>
        public ProductAboutModel About { get; set; }
        public Guid UserId { get; set; }
        public ProductSpecificationsModel Specifications { get; set; }
    }
}
