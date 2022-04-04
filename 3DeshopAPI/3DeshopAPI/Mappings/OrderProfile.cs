using _3DeshopAPI.Models.Order;
using AutoMapper;
using Domain.Order;

namespace _3DeshopAPI.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderUploadModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForMember(x => x.Images, opt => opt.Ignore());
            CreateMap<OrderUploadModel, Order>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.Approved, opt => opt.Ignore());

            CreateMap<Offer, OfferUploadModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForMember(x => x.OrderId, opt => opt.Ignore());
            CreateMap<OfferUploadModel, Offer>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
