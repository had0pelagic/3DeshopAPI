using Domain.Product;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<Product?> GetProduct(Guid id);
        Task UploadProduct(Product product);
        Task<IActionResult> SetProductCategory(Guid productId, Guid categoryId);
        Task<IActionResult> SetProductFormat(Guid productId, Guid formatId);
        Task<IActionResult> SetProductImage(Guid productId, Image model);
        Task<IActionResult> AddProductComment(Guid productId, Guid userId, Comment model);
    }
}
