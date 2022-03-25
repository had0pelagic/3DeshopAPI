namespace _3DeshopAPI.Models.Order
{
    public class OfferUploadModel
    {
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime CompleteTill { get; set; }
    }
}
