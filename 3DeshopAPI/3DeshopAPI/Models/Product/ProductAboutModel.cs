namespace _3DeshopAPI.Models.Product
{
    public class ProductAboutModel
    {
        public string Name { get; set; }
        public DateTime UploadDate { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int? Downloads { get; set; }
    }
}
