using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Order
{
    public class InactiveOrderDisplayModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public UserDisplayModel User { get; set; }
        public DateTime Created { get; set; }
        public DateTime CompleteTill { get; set; }
        public bool Approved { get; set; } = false;
    }
}
