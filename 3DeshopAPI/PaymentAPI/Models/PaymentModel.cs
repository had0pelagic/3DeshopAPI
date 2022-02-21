namespace PaymentAPI.Models
{
    public class PaymentModel
    {
        public string Sender { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
