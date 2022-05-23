using _3DeshopAPI.Models.Balance;
using Domain.Balance;

namespace _3DeshopAPI.Services.Interfaces;

public interface IBalanceService
{
    Task<UserBalanceModel> GetUserBalance(Guid userId);
    Task<BalanceHistory> BalanceTopUp(TopUpModel model);
    Task<BalanceHistory> PayForProduct(PayForProductModel model);
    Task RemoveBalanceHistoryByOrder(Guid orderId);
    Task<BalanceHistory> PayForOrder(PayForOrderModel model);
    Task<BalanceHistory> PayForCompletedOrder(Guid workerId, Guid orderId);
    Task<List<Guid>> GetPurchasedProductIds(Guid id);
    Task<bool> IsBalanceEnough(Guid userId, double amount);
}