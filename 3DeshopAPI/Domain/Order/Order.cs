namespace Domain.Order
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Guid UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime CompleteTill { get; set; }
        public bool Approved { get; set; } = false;
    }
}
