using _3DeshopAPI.Models.Product;
using AutoMapper;
using Domain.Product;

namespace _3DeshopAPI.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<About, ProductAboutModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductAboutModel, About>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Specifications, ProductSpecificationsModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductSpecificationsModel, Specifications>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Image, ImageModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ImageModel, Image>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Domain.Product.File, FileModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<FileModel, Domain.Product.File>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore());

            CreateMap<Category, ProductCategoryModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductCategoryModel, Category>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Format, ProductFormatModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductFormatModel, Format>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Comment, ProductCommentModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.Created, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.Product, opt => opt.DoNotValidate());
            CreateMap<ProductCommentModel, Comment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.User, opt => opt.Ignore())
                .ForMember(x => x.Created, opt => opt.Ignore())
                .ForMember(x => x.Product, opt => opt.Ignore());

            CreateMap<Product, ProductModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForMember(x => x.Formats, opt => opt.Ignore())
                .ForMember(x => x.Categories, opt => opt.Ignore())
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ForMember(x => x.IsBoughtByUser, opt => opt.Ignore());
            CreateMap<ProductModel, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Product, ProductUploadModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForMember(x => x.Formats, opt => opt.Ignore())
                .ForMember(x => x.Categories, opt => opt.Ignore())
                .ForMember(x => x.Images, opt => opt.Ignore())
                .ForMember(x => x.Files, opt => opt.Ignore());
            CreateMap<ProductUploadModel, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Comment, ProductCommentDisplayModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductCommentDisplayModel, Comment>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Product, opt => opt.Ignore());
        }
    }
}
