using Domain.Payment;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Services.Interfaces;

namespace PaymentAPI.Services;

public class PaymentService : IPaymentService
{
    private readonly Context _context;

    public PaymentService(Context context)
    {
        _context = context;
    }

    /// <summary>
    /// Get payment details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Payment?> GetPayment(Guid id)
    {
        var response = await _context.Payments.FindAsync(id);

        return response;
    }

    /// <summary>
    /// Get order payment details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OrderPayment?> GetOrderPayment(Guid id)
    {
        var response = await _context.OrderPayments.FindAsync(id);

        return response;
    }

    /// <summary>
    /// Returns all existing payments
    /// </summary>
    /// <returns></returns>
    public async Task<List<Payment>> GetPayments()
    {
        var response = await _context.Payments.ToListAsync();

        return response;
    }

    /// <summary>
    /// Make payment
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task Pay(Payment model)
    {
        //check if user is valid

        _context.Payments.Add(model);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Make payment for order
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task PayForOrder(OrderPayment model)
    {
        //check if user is valid

        _context.OrderPayments.Add(model);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if user has paid for given product
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<Payment?> UserHasPaid(Guid productId, Guid userId)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);
    }
}