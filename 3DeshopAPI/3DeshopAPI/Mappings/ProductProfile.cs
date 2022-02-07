using _3DeshopAPI.Models.Product;
using AutoMapper;
using Domain.Product;

namespace _3DeshopAPI.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());

            CreateMap<ProductModel, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore());

        }
    }
}
