using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public interface ITrafficEquipmentService
    {
        Task<PayLoad<TrafficEquipmentDTO>> Add(TrafficEquipmentDTO data);
        Task<PayLoad<object>> AddTest();
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode0(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode1(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode2(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode3(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode321(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllNoError(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode0ByAccount(int id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllTest(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneId(int id);
        Task<PayLoad<object>> AddFile(string file);
    }
}
