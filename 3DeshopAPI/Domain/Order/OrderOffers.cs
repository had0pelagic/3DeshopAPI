namespace Domain.Order;

public class OrderOffers
{
    public Guid Id { get; set; }
    public Order Order { get; set; }
    public Offer Offer { get; set; }
}