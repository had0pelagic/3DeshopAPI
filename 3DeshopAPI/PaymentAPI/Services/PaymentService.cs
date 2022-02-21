using Domain.Payment;
using Infrastructure;
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
    }
}
