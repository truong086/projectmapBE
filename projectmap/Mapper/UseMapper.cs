using AutoMapper;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Mapper
{
    public class UseMapper : Profile
    {
        public UseMapper()
        {
            CreateMap<RegisterDTO, User>();
            CreateMap<User, RegisterDTO>();
        }
    }
}
