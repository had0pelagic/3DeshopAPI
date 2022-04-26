﻿using _3DeshopAPI.Models.Product;
using Domain.Product;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDisplayModel>> GetAllProducts();
        Task<List<ProductTableDisplayModel>> GetUserProducts(Guid id);
        Task<ProductModel?> GetProduct(Guid id);
        Task<Product> UploadProduct(ProductUploadModel product);
        Task<ProductModel> ProductToProductModel(Product model, bool isBoughtByUser = false);
        Task<List<ProductDisplayModel>> GetAllPurchases(Guid id, List<Guid> productIds);
    }
}
