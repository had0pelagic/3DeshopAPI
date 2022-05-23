namespace _3DeshopAPI.Models.Balance;

public class PayForOrderModel
{
    public Guid From { get; set; }
    public Guid To { get; set; }
    public Guid OrderId { get; set; }
}