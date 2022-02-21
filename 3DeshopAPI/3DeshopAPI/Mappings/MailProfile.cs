using _3DeshopAPI.Models.Mail;
using AutoMapper;
using Domain.Mail;

namespace _3DeshopAPI.Mappings
{
    public class MailProfile : Profile
    {
        public MailProfile()
        {
            CreateMap<Mail, MailModel>();
            CreateMap<MailModel, Mail>();
        }
    }
}
