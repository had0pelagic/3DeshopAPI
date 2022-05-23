using _3DeshopAPI.Models.Order;
using _3DeshopAPI.Services.Interfaces;
using Domain.Order;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Returns all orders
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-orders")]
    public async Task<List<Order>> GetOrders()
    {
        var response = await _orderService.GetOrders();

        return response;
    }

    /// <summary>
    /// Returns all orders associated with user
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-user-orders/{id}")]
    public async Task<List<Order>> GetUserOrders(Guid id)
    {
        var response = await _orderService.GetUserOrders(id);

        return response;
    }

    /// <summary>
    /// Returns users completed job count
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get-user-completed-job-count/{id}")]

    public async Task<int> GetUserCompletedJobs(Guid id)
    {
        var response = await _orderService.GetUserCompletedJobs(id);

        return response;
    }

    /// <summary>
    /// Returns all inactive orders
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-inactive-orders")]
    public async Task<List<InactiveOrderDisplayModel>> GetInactiveOrders()
    {
        var response = await _orderService.GetInactiveOrders();

        return response;
    }

    /// <summary>
    /// Returns order by given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get-order/{id}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        var response = await _orderService.GetOrder(id);

        return Ok(response);
    }

    /// <summary>
    /// Returns OrderDisplayModel by given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get-display-order/{id}")]
    public async Task<ActionResult<OrderDisplayModel>> GetDisplayOrder(Guid id)
    {
        var response = await _orderService.GetDisplayOrder(id);

        return Ok(response);
    }

    /// <summary>
    /// Uploads new order
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("post-order")]
    public async Task<ActionResult<Order>> PostOrder(OrderUploadModel model)
    {
        var response = await _orderService.PostOrder(model);

        return CreatedAtAction(nameof(GetOrder), new { id = response.Id }, response);
    }

    /// <summary>
    /// Removes user order
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("remove-order/{userId}/{orderId}")]
    public async Task<ActionResult<Order>> RemoveOrder(Guid userId, Guid orderId)
    {
        var response = await _orderService.RemoveOrder(userId, orderId);

        return Ok(response);
    }

    /// <summary>
    /// Sets job progress and writes progress comment
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("set-progress")]
    public async Task<ActionResult<Job>> SetJobProgress(JobProgressModel model)
    {
        var response = await _orderService.SetJobProgress(model);

        return Ok(response);
    }

    /// <summary>
    /// Checks if order job is active
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("is-order-job-active/{orderId}")]
    public async Task<ActionResult<bool>> IsOrderJobActive(Guid orderId)
    {
        var response = await _orderService.IsOrderJobActive(orderId);

        return Ok(response);
    }

    /// <summary>
    /// Creates new job progress and sets job as needed for changes
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPost("request-job-changes")]
    public async Task<ActionResult<Job>> RequestJobChanges(ChangeRequestModel model)
    {
        var response = await _orderService.RequestJobChanges(model);

        return Ok(response);
    }

    /// <summary>
    /// Checks if user is the owner of the given order
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("is-order-owner/{userId}/{orderId}")]
    public async Task<ActionResult<bool>> IsOrderOwner(Guid userId, Guid orderId)
    {
        var response = await _orderService.IsOrderOwner(userId, orderId);

        return Ok(response);
    }

    /// <summary>
    /// Sets job progress and uploads given files
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("set-job-completion")]
    public async Task<ActionResult<Job>> SetJobCompletion(JobCompletionModel model)
    {
        var response = await _orderService.SetJobCompletion(model);

        return Ok(response);
    }

    /// <summary>
    /// Resets all progress of a give job
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("worker-abandon-job")]
    public async Task<ActionResult<Job>> WorkerAbandonJob(AbandonJobModel model)
    {
        var response = await _orderService.WorkerAbandonJob(model);

        return Ok(response);
    }

    /// <summary>
    /// Returns all progresses associated with given job
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("get-job-progress/{userId}/{orderId}")]
    public async Task<ActionResult<List<JobProgressDisplayModel>>> GetJobProgress(Guid userId, Guid orderId)
    {
        var response = await _orderService.GetJobProgress(userId, orderId);

        return Ok(response);
    }

    /// <summary>
    /// Returns all offers
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-offers")]
    public async Task<List<Offer>> GetOffers()
    {
        var response = await _orderService.GetOffers();

        return response;
    }

    /// <summary>
    /// Returns offer by given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get-offer/{id}")]
    public async Task<ActionResult<Offer>> GetOffer(Guid id)
    {
        var response = await _orderService.GetOffer(id);

        return Ok(response);
    }

    /// <summary>
    /// Uploads new offer
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("post-offer")]
    public async Task<ActionResult<Order>> PostOffer(OfferUploadModel model)
    {
        var response = await _orderService.PostOffer(model);

        return CreatedAtAction(nameof(GetOffer), new { id = response.Id }, response);
    }

    /// <summary>
    /// Returns all offers associated with selected order
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-order-offers/{id}")]
    public async Task<List<OfferDisplayModel>> GetOrderOffers(Guid id)
    {
        var response = await _orderService.GetOrderOffers(id);

        return response;
    }

    /// <summary>
    /// Accepts offer by creating new active job
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="offerId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPost("accept-offer/{userId}/{offerId}/{orderId}")]
    public async Task<Job> AcceptOffer(Guid userId, Guid offerId, Guid orderId)
    {
        var response = await _orderService.AcceptOffer(userId, offerId, orderId);

        return response;
    }

    /// <summary>
    /// Declines offer, removes offer from database
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="offerId"></param>
    /// <returns></returns>
    [HttpPost("decline-offer/{userId}/{offerId}")]
    public async Task<Offer> DeclineOffer(Guid userId, Guid offerId)
    {
        var response = await _orderService.DeclineOffer(userId, offerId);

        return response;
    }

    /// <summary>
    /// Returns all user active jobs
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get-user-jobs/{id}")]
    public async Task<List<JobDisplayModel>> GetUserJobs(Guid id)
    {
        var response = await _orderService.GetUserJobs(id);

        return response;
    }

    /// <summary>
    /// Approves order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("approve-order/{orderId}/{userId}")]
    public async Task<Order> ApproveOrder(Guid orderId, Guid userId)
    {
        var response = await _orderService.ApproveOrder(orderId, userId);

        return response;
    }
}