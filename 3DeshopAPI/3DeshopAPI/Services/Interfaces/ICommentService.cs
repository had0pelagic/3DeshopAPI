using Domain.Product;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IActionResult> AddProductComment(Guid productId, Guid userId, Comment model);
    }
}
