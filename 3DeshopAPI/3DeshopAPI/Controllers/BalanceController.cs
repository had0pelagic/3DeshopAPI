using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Services.Interfaces;
using Domain.Balance;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BalanceController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public BalanceController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    /// <summary>
    /// Returns user total balance
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get-user-balance/{userId}")]
    public async Task<ActionResult<UserBalanceModel>> GetUserBalance(Guid userId)
    {
        var response = await _balanceService.GetUserBalance(userId);

        return Ok(response);
    }

    /// <summary>
    /// Adds selected amount to user balance
    /// </summary>
    /// <returns></returns>
    [HttpPost("balance-top-up")]
    public async Task<BalanceHistory> BalanceTopUp(TopUpModel model)
    {
        var response = await _balanceService.BalanceTopUp(model);

        return response;
    }

    /// <summary>
    /// Pays for selected product
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("pay-for-product")]
    public async Task<BalanceHistory> PayForProduct(PayForProductModel model)
    {
        var response = await _balanceService.PayForProduct(model);

        return response;
    }
}