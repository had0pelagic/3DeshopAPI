namespace Domain.Product
{
    public class ProductFormats
    {
        public Guid Id { get; set; }
        public Format Format { get; set; }
        public Product Product { get; set; }
    }
}
