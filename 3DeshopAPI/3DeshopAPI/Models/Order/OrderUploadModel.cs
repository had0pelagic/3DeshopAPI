using _3DeshopAPI.Models.Product;

namespace _3DeshopAPI.Models.Order
{
    public class OrderUploadModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<ImageModel> Images { get; set; }
        public Guid UserId { get; set; }
        public DateTime CompleteTill { get; set; }
    }
}