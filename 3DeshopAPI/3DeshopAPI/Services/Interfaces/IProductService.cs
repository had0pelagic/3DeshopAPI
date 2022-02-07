using Domain.Product;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<Product?> GetProduct(Guid id);
    }
}
