namespace Domain.Order
{
    public class JobProgress
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid JobId { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public int Progress { get; set; }
    }
}
