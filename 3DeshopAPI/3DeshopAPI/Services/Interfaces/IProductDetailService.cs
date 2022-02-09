using Domain.Product;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IProductDetailService
    {
        Task<List<Category>> GetAllCategories();
        Task<List<Format>> GetAllFormats();
    }
}
