using _3DeshopAPI.Models.Product;
using Domain.Product;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDisplayModel>> GetAllProducts();
        Task<List<ProductDisplayModel>> GetProductsByName(string name);
        Task<List<ProductDisplayModel>> GetProductsByCriteria(ProductFindByCriteriaModel name);
        Task<List<ProductDisplayModel>> GetProductsByGivenIdsAndOrderByPrice(ProductGetByIdAndOrderModel model);
        Task<List<ProductDisplayModel>> GetProductsByGivenIdsAndOrderByDate(ProductGetByIdAndOrderModel model);
        Task<List<ProductDisplayModel>> GetProductsOrderByPrice(bool ascending);
        Task<List<ProductDisplayModel>> GetProductsOrderByUploadDate(bool ascending);
        Task<List<ProductTableDisplayModel>> GetUserProducts(Guid id);
        Task<ProductModel?> GetProduct(Guid id);
        Task<Product> UploadProduct(ProductUploadModel product);
        Task<ProductModel> ProductToProductModel(Product model, bool isBoughtByUser = false);
        Task<List<ProductDisplayModel>> GetAllPurchases(Guid id, List<Guid> productIds);
    }
}
