using _3DeshopAPI.Models.Balance;
using Domain.Balance;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<UserBalanceModel> GetUserBalance(Guid userId);
        Task<BalanceHistory> BalanceTopUp(Guid userId, double amount);
        Task<BalanceHistory> PayForProduct(PayForProductModel model);
        Task<List<Guid>> GetPurchasedIds(Guid id);
        Task<bool> IsBalanceEnough(Guid userId, double amount);
    }
}
