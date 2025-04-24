using projectmap.Common;

namespace projectmap.Service
{
    public interface INotifitionAppService
    {
        Task<PayLoad<string>> notifi();
    }
}
