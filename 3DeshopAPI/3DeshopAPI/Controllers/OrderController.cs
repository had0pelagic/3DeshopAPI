using _3DeshopAPI.Models.Mail;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public OrderController(IMailService mailService, IMapper mapper)
        {
            _mailService = mailService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(MailModel model)
        {
            var mail = _mapper.Map<Domain.Mail.Mail>(model);

            return Ok(await _mailService.SendMail(mail));
        }
    }
}
