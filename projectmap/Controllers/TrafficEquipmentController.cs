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
    public class TrafficEquipmentController : ControllerBase
    {
        private readonly ITrafficEquipmentService _trafficEquipmentService;
        public TrafficEquipmentController(ITrafficEquipmentService trafficEquipmentService)
        {
            _trafficEquipmentService = trafficEquipmentService;
        }

        [HttpPost]
        [Route(nameof(Add))]
        public async Task<PayLoad<TrafficEquipmentDTO>> Add (TrafficEquipmentDTO trafficEquipmentDTO)
        {
            return await _trafficEquipmentService.Add(trafficEquipmentDTO);
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _trafficEquipmentService.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(FindAllTest))]
        public async Task<PayLoad<object>> FindAllTest(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllTest(name, page, pageSize);
        }

        [HttpPost]
        [Route(nameof(LoadFile))]
        public async Task<PayLoad<object>> LoadFile(string path)
        {
            return await _trafficEquipmentService.AddFile(path);
        }
    }
}
