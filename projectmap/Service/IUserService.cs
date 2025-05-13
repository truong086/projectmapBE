using projectmap.Common;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public interface IUserService
    {
        Task<PayLoad<RegisterDTO>> Register (RegisterDTO registerDTO);
        Task<PayLoad<object>> searchName (string? name);
        Task<PayLoad<object>> searchId (int id);
        Task<PayLoad<ReturnLogin>> Login (RegisterDTO registerDTO);
        Task<PayLoad<string>> LogOut();
        Task<PayLoad<object>> CheckToken(string token);
        Task<PayLoad<string>> AddToken(string token);
        Task<PayLoad<object>> GenTokenOld();
    }
}
