namespace PaymentAPI.Models;

public class OrderPaymentModel
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public double Amount { get; set; }
}