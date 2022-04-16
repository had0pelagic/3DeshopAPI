using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Product
{
    public class ProductModel
    {
        /// <summary>
        /// Model used to return full product data (when opening product on the web)
        /// </summary>
        public UserDisplayModel User { get; set; }
        public ProductAboutModel About { get; set; }
        public ProductSpecificationsModel Specifications { get; set; }
        public List<ProductCategoryModel> Categories { get; set; }
        public List<ProductFormatModel> Formats { get; set; }
        public List<ImageModel> Images { get; set; }
        public bool IsBoughtByUser { get; set; }
    }
}
