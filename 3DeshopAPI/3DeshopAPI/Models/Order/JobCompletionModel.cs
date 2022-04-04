using _3DeshopAPI.Models.Product;

namespace _3DeshopAPI.Models.Order
{
    public class JobCompletionModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public int Progress { get; set; }
        public List<FileModel> Files { get; set; }
    }
}
