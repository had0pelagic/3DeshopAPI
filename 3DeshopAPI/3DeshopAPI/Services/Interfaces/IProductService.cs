using _3DeshopAPI.Models.Product;
using Domain.Product;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<ProductModel?> GetProduct(Guid id);
        Task<Product> UploadProduct(ProductUploadModel product);
        Task<ProductModel> ToProductModel(Product model);
    }
}
