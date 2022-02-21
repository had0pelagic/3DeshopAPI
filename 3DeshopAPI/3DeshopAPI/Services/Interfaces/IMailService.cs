using Domain.Mail;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendMail(Mail model);
    }
}
