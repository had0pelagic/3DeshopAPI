using _3DeshopAPI.Models.User;

namespace _3DeshopAPI.Models.Order;

public class JobProgressDisplayModel
{
    public Guid Id { get; set; }
    public UserDisplayModel User { get; set; }
    public Guid JobId { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; }
    public int Progress { get; set; }
}