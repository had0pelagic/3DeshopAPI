namespace PaymentAPI.Models
{
    public class PaymentModel
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public string Sender { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
