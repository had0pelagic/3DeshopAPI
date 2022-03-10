namespace Domain.Product
{
    public class ProductFiles
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public File File { get; set; }
    }
}
