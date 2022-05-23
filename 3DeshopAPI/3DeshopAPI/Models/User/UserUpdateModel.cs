namespace _3DeshopAPI.Models.User;

public class UserUpdateModel
{
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ImageModel? Image { get; set; }
    public string? Email { get; set; }
}