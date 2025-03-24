using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public interface ITrafficEquipmentService
    {
        Task<PayLoad<TrafficEquipmentDTO>> Add(TrafficEquipmentDTO data);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> AddFile(string file);
    }
}
