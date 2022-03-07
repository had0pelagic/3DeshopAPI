using Domain.Payment;

namespace PaymentAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPayment(Guid id);
        Task<List<Payment>> GetPayments();
        Task Pay(Payment model);
    }
}
