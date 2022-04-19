using _3DeshopAPI.Models.User;
using AutoMapper;
using Domain;

namespace _3DeshopAPI.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.Password, opt => opt.DoNotValidate());

            CreateMap<UserRegisterModel, User>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore());

            CreateMap<UserModel, User>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Password, opt => opt.Ignore())
                .ForMember(x => x.UserRole, opt => opt.Ignore());

            CreateMap<UserLoginModel, User>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.FirstName, opt => opt.Ignore())
                .ForMember(x => x.LastName, opt => opt.Ignore())
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.Password, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.UserRole, opt => opt.Ignore());

            CreateMap<User, UserDisplayModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.Password, opt => opt.DoNotValidate());

            CreateMap<UserDisplayModel, User>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Password, opt => opt.Ignore())
                .ForMember(x => x.FirstName, opt => opt.Ignore())
                .ForMember(x => x.LastName, opt => opt.Ignore())
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.UserRole, opt => opt.Ignore());

            CreateMap<User, UserTableDisplayModel>()
                .ForSourceMember(x => x.Id, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.Password, opt => opt.DoNotValidate());

            CreateMap<UserTableDisplayModel, User>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Password, opt => opt.Ignore())
                .ForMember(x => x.FirstName, opt => opt.Ignore())
                .ForMember(x => x.LastName, opt => opt.Ignore())
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.Image, opt => opt.Ignore())
                .ForMember(x => x.UserRole, opt => opt.Ignore());
        }
    }
}
