namespace Domain.Product
{
    public class ProductCategories
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
    }
}
