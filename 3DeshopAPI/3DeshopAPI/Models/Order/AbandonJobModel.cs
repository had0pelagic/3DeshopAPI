namespace _3DeshopAPI.Models.Order
{
    public class AbandonJobModel
    {
        public Guid UserId { get; set; }
        public Guid JobId { get; set; }
    }
}
