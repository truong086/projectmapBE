using projectmap.Common;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public interface IUserService
    {
        Task<PayLoad<RegisterDTO>> Register (RegisterDTO registerDTO);
        Task<PayLoad<ReturnLogin>> Login (RegisterDTO registerDTO);
        Task<PayLoad<string>> LogOut();
    }
}
