using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectmap.Common;
using projectmap.Service;
using projectmap.ViewModel;

namespace projectmap.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RepairDetailsController : ControllerBase
    {
        private readonly IRepairDetailsService _repairDetailsService;
        public RepairDetailsController(IRepairDetailsService repairDetailsService)
        {
            _repairDetailsService = repairDetailsService;
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<RepairDetailsDTO>> Add (RepairDetailsDTO data)
        {
            return await _repairDetailsService.Add(data);
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _repairDetailsService.FindAll(name, page, pageSize);
        }
    }
}
