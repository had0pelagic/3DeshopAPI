namespace Domain.Product
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public About About { get; set; }
        public Specifications Specifications { get; set; }
    }
}
