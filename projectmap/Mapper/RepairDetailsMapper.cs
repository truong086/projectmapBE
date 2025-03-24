using AutoMapper;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Mapper
{
    public class RepairDetailsMapper : Profile
    {
        public RepairDetailsMapper()
        {
            CreateMap<RepairDetailsDTO, RepairDetails>();
            CreateMap<RepairDetails, RepairDetailsDTO>();
        }
    }
}
