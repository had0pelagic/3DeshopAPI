namespace Domain.Order
{
    public class Offer
    {
        public Guid Id { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime CompleteTill { get; set; }
    }
}
