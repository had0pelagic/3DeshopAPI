namespace _3DeshopAPI.Models.User
{
    public class UserRegisterModel
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ImageUrl { get; set; }
        public string? UserRole { get; set; }
    }
}
