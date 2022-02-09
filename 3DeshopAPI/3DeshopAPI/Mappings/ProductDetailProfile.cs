using _3DeshopAPI.Models.ProductDetail;
using AutoMapper;
using Domain.Product;

namespace _3DeshopAPI.Mappings
{
    public class ProductDetailProfile : Profile
    {
        public ProductDetailProfile()
        {
            CreateMap<Category, CategoryModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<CategoryModel, Category>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Format, FormatModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<FormatModel, Format>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
