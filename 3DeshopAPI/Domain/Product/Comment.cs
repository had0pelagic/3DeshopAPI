namespace Domain.Product
{
    public class Comment
    {
        public Guid Id { get; set; }
        public User Owner { get; set; }
        public string ProductComment { get; set; }
    }
}
