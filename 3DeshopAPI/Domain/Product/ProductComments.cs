namespace Domain.Product
{
    public class ProductComments
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public Comment Comment { get; set; }
    }
}
