using AutoMapper;
using Domain.Payment;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Models;
using PaymentAPI.Services.Interfaces;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get payment details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-product-payment/{id}")]
        public async Task<ActionResult<PaymentDisplayModel>> GetPayment(Guid id)
        {
            var response = await _paymentService.GetPayment(id);

            return response != null ? Ok(_mapper.Map<PaymentDisplayModel>(response)) : NotFound();
        }

        /// <summary>
        /// Make payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("pay-for-product")]
        public async Task<ActionResult<PaymentDisplayModel>> Pay(PaymentModel model)
        {
            var payment = _mapper.Map<Payment>(model);

            await _paymentService.Pay(payment);

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, _mapper.Map<PaymentDisplayModel>(payment));
        }

        /// <summary>
        /// Get payment details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-order-payment/{id}")]
        public async Task<ActionResult<OrderPaymentDisplayModel>> GetOrderPayment(Guid id)
        {
            var response = await _paymentService.GetOrderPayment(id);

            return response != null ? Ok(_mapper.Map<PaymentDisplayModel>(response)) : NotFound();
        }

        /// <summary>
        /// Make payment for order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("pay-for-order")]
        public async Task<ActionResult<OrderPaymentDisplayModel>> PayForOrder(OrderPaymentModel model)
        {
            var payment = _mapper.Map<OrderPayment>(model);

            await _paymentService.PayForOrder(payment);

            return CreatedAtAction(nameof(GetOrderPayment), new { id = payment.Id }, _mapper.Map<OrderPaymentDisplayModel>(payment));
        }
    }
}
