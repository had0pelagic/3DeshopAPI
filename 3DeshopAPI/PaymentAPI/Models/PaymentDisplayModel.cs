namespace PaymentAPI.Models
{
    public class PaymentDisplayModel
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
