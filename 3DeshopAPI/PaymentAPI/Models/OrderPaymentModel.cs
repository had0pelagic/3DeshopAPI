namespace PaymentAPI.Models
{
    public class OrderPaymentModel
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Sender { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}