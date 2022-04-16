using Domain.Product;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IFileService
    {
        Task<List<FileContentResult>> GetProductFiles(Guid productId, Guid userId);
        Task<List<FileContentResult>> GetOrderFiles(Guid orderId, Guid userId);
    }
}
