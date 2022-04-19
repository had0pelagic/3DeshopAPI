using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Order
{
    public class OfferDisplayModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public UserTableDisplayModel User { get; set; }
        public DateTime Created { get; set; }
        public DateTime CompleteTill { get; set; }
    }
}
