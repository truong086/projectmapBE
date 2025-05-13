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

        [HttpPost]
        [Route(nameof(AddTest))]
        public async Task<PayLoad<object>> AddTest()
        {
            return await _trafficEquipmentService.AddTest();
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllNameDistric))]
        public async Task<PayLoad<object>> FindAllNameDistric(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllNameDistric(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(GetNormal))]
        public async Task<PayLoad<object>> GetNormal()
        {
            return await _trafficEquipmentService.GetNormal();
        }

        [HttpGet]
        [Route(nameof(GetNormalError))]
        public async Task<PayLoad<object>> GetNormalError()
        {
            return await _trafficEquipmentService.GetNormalError();
        }

        [HttpGet]
        [Route(nameof(GetNormalDistrict))]
        public async Task<PayLoad<object>> GetNormalDistrict()
        {
            return await _trafficEquipmentService.GetNormalDistrict();
        }

        [HttpGet]
        [Route(nameof(GetNormalDistrict2))]
        public async Task<PayLoad<object>> GetNormalDistrict2()
        {
            return await _trafficEquipmentService.GetNormalDistrict2();
        }

        [HttpGet]
        [Route(nameof(GetNormalDistrictUpdate))]
        public async Task<PayLoad<object>> GetNormalDistrictUpdate()
        {
            return await _trafficEquipmentService.GetNormalDistrictUpdate();
        }

        [HttpGet]
        [Route(nameof(GetNormalDistrictUpdate2))]
        public async Task<PayLoad<object>> GetNormalDistrictUpdate2()
        {
            return await _trafficEquipmentService.GetNormalDistrictUpdate2();
        }

        [HttpGet]
        [Route(nameof(FindAllErrorCode0ByAccount))]
        public async Task<PayLoad<object>> FindAllErrorCode0ByAccount(int id, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllErrorCode0ByAccount(id, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllErrorCode0))]
        public async Task<PayLoad<object>> FindAllErrorCode0(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllErrorCode0(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllErrorCode1))]
        public async Task<PayLoad<object>> FindAllErrorCode1(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllErrorCode1(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllErrorCode2))]
        public async Task<PayLoad<object>> FindAllErrorCode2(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllErrorCode2(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllErrorCode3))]
        public async Task<PayLoad<object>> FindAllErrorCode3(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllErrorCode3(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllErrorCode321))]
        public async Task<PayLoad<object>> FindAllErrorCode321(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllErrorCode321(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllNoError))]
        public async Task<PayLoad<object>> FindAllNoError(string? name, int page = 1, int pageSize = 20)
        {
            return await _trafficEquipmentService.FindAllNoError(name, page, pageSize);
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

        [HttpGet]
        [Route(nameof(TotalError))]
        public async Task<PayLoad<object>> TotalError()
        {
            return await _trafficEquipmentService.TotalError();
        }
        [HttpGet]
        [Route(nameof(TotalErrorUpdate))]
        public async Task<PayLoad<object>> TotalErrorUpdate()
        {
            return await _trafficEquipmentService.TotalErrorUpdate();
        }

        [HttpGet]
        [Route(nameof(TotalErrorNoUpdate))]
        public async Task<PayLoad<object>> TotalErrorNoUpdate()
        {
            return await _trafficEquipmentService.TotalErrorNoUpdate();
        }

        [HttpGet]
        [Route(nameof(TotalErrorNoUpdate2))]
        public async Task<PayLoad<object>> TotalErrorNoUpdate2()
        {
            return await _trafficEquipmentService.TotalErrorNoUpdate2();
        }

        [HttpPost]
        [Route(nameof(LoadFile))]
        public async Task<PayLoad<object>> LoadFile(string path)
        {
            return await _trafficEquipmentService.AddFile(path);
        }
    }
}
