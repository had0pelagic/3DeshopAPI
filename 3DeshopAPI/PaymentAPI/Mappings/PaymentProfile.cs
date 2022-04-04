using AutoMapper;
using Domain.Payment;
using PaymentAPI.Models;

namespace PaymentAPI.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<PaymentModel, Payment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ValidTill, opt => opt.Ignore())
                .ForMember(x => x.Receiver, opt => opt.Ignore());

            CreateMap<Payment, PaymentDisplayModel>()
                 .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<PaymentDisplayModel, Payment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ValidTill, opt => opt.Ignore())
                .ForMember(x => x.Receiver, opt => opt.Ignore());

            CreateMap<PaymentDisplayModel, PaymentModel>()
                .ForSourceMember(x => x.Receiver, opt => opt.DoNotValidate());
            CreateMap<PaymentModel, PaymentDisplayModel>()
                .ForMember(x => x.Receiver, opt => opt.Ignore());

            CreateMap<OrderPayment, OrderPaymentModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<OrderPaymentModel, OrderPayment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ValidTill, opt => opt.Ignore())
                .ForMember(x => x.Receiver, opt => opt.Ignore());

            CreateMap<OrderPayment, OrderPaymentDisplayModel>()
                 .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<OrderPaymentDisplayModel, OrderPayment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ValidTill, opt => opt.Ignore())
                .ForMember(x => x.Receiver, opt => opt.Ignore());

            CreateMap<OrderPaymentDisplayModel, OrderPaymentModel>()
                .ForSourceMember(x => x.Receiver, opt => opt.DoNotValidate());
            CreateMap<OrderPaymentModel, OrderPaymentDisplayModel>()
                .ForMember(x => x.Receiver, opt => opt.Ignore());
        }
    }
}
