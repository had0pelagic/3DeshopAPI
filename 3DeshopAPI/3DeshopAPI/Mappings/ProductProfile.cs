﻿using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Models.User;
using AutoMapper;
using Domain;
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

            CreateMap<Product, ProductPageModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.UserId, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.Specifications, opt => opt.DoNotValidate());
            CreateMap<ProductPageModel, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Specifications, opt => opt.Ignore());

            CreateMap<Image, ProductImageModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductImageModel, Image>()
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
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate());
            CreateMap<ProductModel, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<ProductUploadModel, Product>()
                .ForMember(x => x.Id, opt => opt.Ignore());

        }
    }
}
