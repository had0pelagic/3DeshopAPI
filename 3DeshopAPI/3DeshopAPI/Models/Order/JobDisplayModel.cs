namespace _3DeshopAPI.Models.Order
{
    public class JobDisplayModel
    {
        public Guid Id { get; set; }
        public OrderDisplayModel Order { get; set; }
        public OfferDisplayModel Offer { get; set; }
        public int Progress { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }
        public bool NeedChanges { get; set; }
    }
}
