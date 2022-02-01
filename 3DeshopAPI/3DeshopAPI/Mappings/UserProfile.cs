using _3DeshopAPI.Models;
using AutoMapper;
using Domain;

namespace _3DeshopAPI.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserModel>()
                .ForSourceMember(x => x.Password, opt => opt.DoNotValidate());
            CreateMap<UserModel, User>()
                .ForMember(x => x.Password, opt => opt.Ignore());
        }
    }
}
