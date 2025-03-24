using projectmap.Common;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public interface IRepairDetailsService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<RepairDetailsDTO>> Add(RepairDetailsDTO data);
    }
}
