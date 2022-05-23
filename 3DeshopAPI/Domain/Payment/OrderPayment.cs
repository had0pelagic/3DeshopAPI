﻿namespace Domain.Payment;

public class OrderPayment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public string Sender { get; set; }
    public string Receiver { get; set; } = "5555555555554444";
    public double Amount { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime ValidTill { get; set; } = DateTime.Now.AddYears(1);
}