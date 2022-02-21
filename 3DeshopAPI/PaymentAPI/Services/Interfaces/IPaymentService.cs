using Domain.Payment;

namespace PaymentAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetPayment(Guid id);
        Task Pay(Payment model);
    }
}
