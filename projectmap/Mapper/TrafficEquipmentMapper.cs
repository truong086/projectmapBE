using AutoMapper;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Mapper
{
    public class TrafficEquipmentMapper : Profile
    {
        public TrafficEquipmentMapper()
        {
            CreateMap<TrafficEquipmentDTO, TrafficEquipment>();
            CreateMap<TrafficEquipment, TrafficEquipmentDTO>();
        }
    }
}
