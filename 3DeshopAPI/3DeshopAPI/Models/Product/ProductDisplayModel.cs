namespace _3DeshopAPI.Models.Product
{
    public class ProductDisplayModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public double Price { get; set; }
        public int? Downloads { get; set; }
        public ImageModel Image { get; set; }
        public List<ProductCategoryModel> Categories { get; set; }
    }
}
