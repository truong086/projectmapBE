using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectmap.Common;
using projectmap.Service;

namespace projectmap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifitionAppController : ControllerBase
    {
        private readonly INotifitionAppService _notifitionAppService;
        public NotifitionAppController(INotifitionAppService notifitionAppService)
        {
            _notifitionAppService = notifitionAppService;   
        }

        [HttpPost]
        [Route(nameof(notifi))]
        public async Task<PayLoad<string>> notifi()
        {
            return await _notifitionAppService.notifi();
        }
            
    }
}
