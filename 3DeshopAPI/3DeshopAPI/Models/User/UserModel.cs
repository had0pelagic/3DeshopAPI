using Domain.Product;

namespace _3DeshopAPI.Models.User
{
    public class UserModel
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Image Image { get; set; }
        public string? Email { get; set; }
    }
}
