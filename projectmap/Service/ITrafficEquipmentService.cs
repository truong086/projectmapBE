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
        Task<PayLoad<object>> GetNormal();
        Task<PayLoad<object>> GetNormalError();
        Task<PayLoad<object>> GetNormalDistrict();
        Task<PayLoad<object>> GetNormalDistrict2();
        Task<PayLoad<object>> GetNormalDistrictUpdate();
        Task<PayLoad<object>> GetNormalDistrictUpdate2();
        Task<PayLoad<object>> TotalError();
        Task<PayLoad<object>> TotalErrorUpdate();
        Task<PayLoad<object>> TotalErrorNoUpdate();
        Task<PayLoad<object>> TotalErrorNoUpdate2();
        Task<PayLoad<object>> FindAllNoError(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllNameDistric(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllErrorCode0ByAccount(int id, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllTest(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindOneId(int id);
        Task<PayLoad<object>> AddFile(string file);
    }
}
