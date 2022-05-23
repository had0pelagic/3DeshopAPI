namespace Domain.Balance;

public class BalanceHistory
{
    public Guid Id { get; set; }
    public double Balance { get; set; }
    public User? From { get; set; }
    public User? To { get; set; }
    public bool IsPending { get; set; }//pending only if payment is for given order
    public bool IsTopUp { get; set; }
    public DateTime LastTime { get; set; }
    public Order.Order? Order { get; set; }
    public Product.Product? Product { get; set; }
}