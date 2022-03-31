namespace Domain.Order
{
    public class Job
    {
        public Guid Id { get; set; }
        public Order Order { get; set; }
        public Offer Offer { get; set; }
        public int Progress { get; set; } = 0;
        public DateTime Created { get; set; }
        public bool Active { get; set; } = true;
    }
}
