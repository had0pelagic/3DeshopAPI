using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Services.Interfaces;
using Domain.Balance;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly Context _context;

        public BalanceService(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns user total balance
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<UserBalanceModel> GetUserBalance(Guid userId)
        {
            var userExists = _context.Users.Any(x => x.Id == userId);

            if (!userExists)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var balanceHistoryList = await _context.BalanceHistory
                .Include(x => x.From)
                .Include(x => x.To)
                .AsQueryable()
                .Where(x => x.To.Id == userId || x.From.Id == userId)
                .ToListAsync();

            var totalBalance = balanceHistoryList
                .Where(x => x.To?.Id == userId /*&& !x.IsPending*/)
                .Sum(x => x.Balance);

            var paymentSum = balanceHistoryList
                .Where(x => x.From?.Id == userId)
                .Sum(x => x.Balance);

            var userBalance = new UserBalanceModel()
            {
                Balance = totalBalance - paymentSum,
            };

            return userBalance;
        }

        /// <summary>
        /// Adds selected amount to user balance
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<BalanceHistory> BalanceTopUp(TopUpModel model)
        {
            var userExists = _context.Users.Any(x => x.Id == model.UserId);

            if (!userExists)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var user = await _context.Users.FindAsync(model.UserId);
            var balanceHistory = new BalanceHistory()
            {
                Balance = model.Amount,
                To = user,
                IsPending = false,
                IsTopUp = true,
                LastTime = DateTime.UtcNow
            };

            await _context.BalanceHistory.AddAsync(balanceHistory);
            await _context.SaveChangesAsync();

            return balanceHistory;
        }

        /// <summary>
        /// Pays for selected product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<BalanceHistory> PayForProduct(PayForProductModel model)
        {
            var userExists = _context.Users.Any(x => x.Id == model.UserId);

            if (!userExists)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var product = _context.Products
                .Include(x => x.About)
                .Where(x => x.Id == model.ProductId)
                .First();
            var productOwner = await _context.Users.FindAsync(product.UserId);

            if (productOwner == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var user = await _context.Users.FindAsync(model.UserId);

            if (model.UserId == productOwner.Id)
            {
                throw new InvalidClientOperationException(ErrorCodes.OwnerUnableToBuyProduct);
            }

            var isBalanceEnough = await IsBalanceEnough(model.UserId, product.About.Price);

            if (!isBalanceEnough)
            {
                throw new InvalidClientOperationException(ErrorCodes.NotEnoughBalance);
            }

            var purchasedIds = await GetPurchasedIds(model.UserId);

            if (purchasedIds.Contains(model.ProductId))
            {
                throw new InvalidClientOperationException(ErrorCodes.DuplicateBuy);
            }

            var balanceHistory = new BalanceHistory()
            {
                Balance = product.About.Price,
                From = user,
                To = productOwner,
                IsPending = false,
                IsTopUp = false,
                LastTime = DateTime.UtcNow,
                Product = product
            };

            await _context.BalanceHistory.AddAsync(balanceHistory);
            await _context.SaveChangesAsync();

            return balanceHistory;
        }

        /// <summary>
        /// Removes balance history entry by given order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task RemoveBalanceHistoryByOrder(Guid orderId)
        {
            var balanceHistories = await _context.BalanceHistory.Include(x => x.Order).ToListAsync();
            var orderBalanceHistory = balanceHistories.Where(x => x.Order?.Id == orderId).First();

            if (orderBalanceHistory == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.BalanceHistoryNotFound);
            }

            _context.BalanceHistory.Remove(orderBalanceHistory);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if user balance is enough for given the amount
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<bool> IsBalanceEnough(Guid userId, double amount)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var userBalance = await GetUserBalance(userId);
            var balanceLeft = userBalance.Balance - amount;

            if (balanceLeft < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Pays/reserves money for created order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<BalanceHistory> PayForOrder(PayForOrderModel model)
        {
            var userExists = _context.Users.Any(x => x.Id == model.UserId);

            if (!userExists)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = _context.Orders
                .Where(x => x.Id == model.OrderId)
                .First();
            var user = await _context.Users.FindAsync(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var balanceHistory = new BalanceHistory()
            {
                Balance = order.Price,
                From = user,
                IsPending = true,
                IsTopUp = false,
                LastTime = DateTime.UtcNow,
                Order = order
            };

            await _context.BalanceHistory.AddAsync(balanceHistory);
            await _context.SaveChangesAsync();

            return balanceHistory;
        }

        /// <summary>
        /// Sends pending payment to worker after order completion and approvement
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<BalanceHistory> PayForCompletedOrder(Guid workerId, Guid orderId)
        {
            var toUser = await _context.Users.FindAsync(workerId);

            if (toUser == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = _context.Orders
                .Where(x => x.Id == orderId)
                .First();

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            var orderBalanceHistory = await _context.BalanceHistory
                .Include(x => x.Order)
                .Where(x => x.Order.Id == orderId && x.IsPending)
                .FirstAsync();

            if (orderBalanceHistory == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.BalanceHistoryNotFound);
            }

            var balanceHistory = new BalanceHistory()
            {
                Balance = orderBalanceHistory.Balance,
                From = null,
                To = toUser,
                IsPending = false,
                IsTopUp = false,
                LastTime = DateTime.UtcNow,
                Order = order
            };

            await _context.BalanceHistory.AddAsync(balanceHistory);
            await _context.SaveChangesAsync();

            return balanceHistory;
        }

        /// <summary>
        /// Returns all user purchased product ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<List<Guid>> GetPurchasedIds(Guid id)
        {
            var userExists = _context.Users.Any(x => x.Id == id);

            if (!userExists)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var ids = await _context.BalanceHistory
                .Include(x => x.Product)
                .AsQueryable()
                .Where(x => x.From.Id == id)
                .Where(x => x.Product.Id != null)
                .Select(x => x.Product.Id)
                .ToListAsync();

            return ids;
        }
    }
}