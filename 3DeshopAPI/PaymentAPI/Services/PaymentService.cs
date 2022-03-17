using Domain.Payment;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Services.Interfaces;

namespace PaymentAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Context _context;

        public PaymentService(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Get payment details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Payment?> GetPayment(Guid id)
        {
            var response = await _context.Payments.FindAsync(id);

            return response;
        }

        public async Task<List<Payment>> GetPayments()
        {
            var response = await _context.Payments.ToListAsync();

            return response;
        }

        /// <summary>
        /// Make payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Pay(Payment model)
        {
            //check if user is valid

            _context.Payments.Add(model);

            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> UserHasPaid(Guid productId, Guid userId)
        {
            return await _context.Payments.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);
        }
    }
}
