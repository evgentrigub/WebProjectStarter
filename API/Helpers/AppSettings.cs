using API.Dtos;
using API.Models;
using API.Models.ViewModels;
using AutoMapper;

namespace API.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
    }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
            CreateMap<UserAuthenticateModel, User>();
            CreateMap<User, UserAuthenticateModel>();
        }
    }
}