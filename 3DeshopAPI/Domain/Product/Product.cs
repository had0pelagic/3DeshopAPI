namespace Domain.Product
{
    public class Product
    {
        public Guid Id { get; set; }
        public User Owner { get; set; }
        public About About { get; set; }
        public Specifications Specifications { get; set; }
    }
}
