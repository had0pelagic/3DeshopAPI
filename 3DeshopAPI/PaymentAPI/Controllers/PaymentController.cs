using AutoMapper;
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
        [HttpGet("{id}")]
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
        [HttpPost]
        public async Task<ActionResult<PaymentDisplayModel>> Pay(PaymentModel model)
        {
            var payment = _mapper.Map<Domain.Payment.Payment>(model);

            await _paymentService.Pay(payment);

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, _mapper.Map<PaymentDisplayModel>(payment));
        }
    }
}
