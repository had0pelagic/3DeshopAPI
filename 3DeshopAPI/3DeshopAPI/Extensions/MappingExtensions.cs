using _3DeshopAPI.Models.Product;
using Domain.Product;

namespace _3DeshopAPI.Extensions
{
    public static class MappingExtensions
    {
        public static ProductPageModel ToProductPageModel(this Product product)
        {
            return new ProductPageModel()
            {
                About = new ProductAboutModel()
                {
                    Description = product.About.Description,
                    Downloads = product.About.Downloads,
                    Name = product.About.Name,
                    Price = product.About.Price,
                    UploadDate = product.About.UploadDate
                }
            };
        }
    }
}