using _3DeshopAPI.Models.Mail;
using _3DeshopAPI.Models.Order;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Order;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public OrderController(IMailService mailService, IMapper mapper, IOrderService orderService)
        {
            _mailService = mailService;
            _mapper = mapper;
            _orderService = orderService;
        }

        /// <summary>
        /// Sends email through google smtp server
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("send-mail")]
        public async Task<IActionResult> SendMail(MailModel model)
        {
            var mail = _mapper.Map<Domain.Mail.Mail>(model);

            return Ok(await _mailService.SendMail(mail));
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
        /// Returns all inactive orders
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-inactive-orders")]
        public async Task<List<Order>> GetInactiveOrders()
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
        public async Task<List<Offer>> GetOrderOffers(Guid id)
        {
            var response = await _orderService.GetOrderOffers(id);

            return response;
        }

        /// <summary>
        /// Approves offer by creating new active job
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="offerId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost("approve-offer/{userId}/{offerId}/{orderId}")]
        public async Task<Job> ApproveOffer(Guid userId, Guid offerId, Guid orderId)
        {
            var response = await _orderService.ApproveOffer(userId, offerId, orderId);

            return response;
        }
    }
}
