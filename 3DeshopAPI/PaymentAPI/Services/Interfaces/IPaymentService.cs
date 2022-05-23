using Domain.Payment;

namespace PaymentAPI.Services.Interfaces;

public interface IPaymentService
{
    Task<Payment?> GetPayment(Guid id);
    Task<OrderPayment?> GetOrderPayment(Guid id);
    Task<List<Payment>> GetPayments();
    Task<Payment?> UserHasPaid(Guid productId, Guid userId);
    Task Pay(Payment model);
    Task PayForOrder(OrderPayment model);
}