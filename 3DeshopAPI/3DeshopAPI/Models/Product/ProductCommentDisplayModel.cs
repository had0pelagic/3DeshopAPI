using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Product;

public class ProductCommentDisplayModel
{
    public UserDisplayModel User { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; }
}