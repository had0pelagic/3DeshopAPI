namespace PaymentAPI.Models;

public class PaymentDisplayModel
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string Receiver { get; set; }
    public double Amount { get; set; }
}