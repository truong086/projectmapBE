﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        public async Task<PayLoad<RepairDetailsDTO>> Add ([FromForm]RepairDetailsDTO data)
        {
            return await _repairDetailsService.Add(data);
        }

        [HttpGet]
        [Route(nameof(FindAll))]
        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            return await _repairDetailsService.FindAll(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllNoDoneByAdmin))]
        public async Task<PayLoad<object>> FindAllNoDoneByAdmin(string? name, int page = 1, int pageSize = 20)
        {
            return await _repairDetailsService.FindAllNoDoneByAdmin(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllDoneByAdmin))]
        public async Task<PayLoad<object>> FindAllDoneByAdmin(string? name, int page = 1, int pageSize = 20)
        {
            return await _repairDetailsService.FindAllDoneByAdmin(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllDoneByAccount))]
        public async Task<PayLoad<object>> FindAllDoneByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _repairDetailsService.FindAllDoneByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindAllNoDoneByAccount))]
        public async Task<PayLoad<object>> FindAllNoDoneByAccount(string? name, int page = 1, int pageSize = 20)
        {
            return await _repairDetailsService.FindAllNoDoneByAccount(name, page, pageSize);
        }

        [HttpGet]
        [Route(nameof(FindOneId))]
        public async Task<PayLoad<object>> FindOneId(int id)
        {
            return await _repairDetailsService.FindOneId(id);
        }

        [HttpGet]
        [Route(nameof(FindAllDone1month))]
        public async Task<PayLoad<object>> FindAllDone1month()
        {
            return await _repairDetailsService.FindAllDone1month();
        }

        [HttpPut]
        [Route(nameof(Update))]
        public async Task<PayLoad<RepairDetailsUpdate>> Update(RepairDetailsUpdate data)
        {
            return await _repairDetailsService.Update(data);
        }

        [HttpPut]
        [Route(nameof(UpdateByAccout))]
        public async Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByAccout(RepairDetailsUpdateByAccont data)
        {
            return await _repairDetailsService.UpdateByAccout(data);
        }

        [HttpPut]
        [Route(nameof(UpdateConfimData))]
        public async Task<PayLoad<ConfirmData>> UpdateConfimData(ConfirmData data)
        {
            return await _repairDetailsService.UpdateConfimData(data);
        }

        [HttpPut]
        [Route(nameof(UpdateByClose))]
        public async Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByClose(RepairDetailsUpdateByAccont data)
        {
            return await _repairDetailsService.UpdateByClose(data);
        }

        [HttpPut]
        [Route(nameof(UpdateByOffClose))]
        public async Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByOffClose(RepairDetailsUpdateByAccont data)
        {
            return await _repairDetailsService.UpdateByOffClose(data);
        }

        [HttpPut]
        [Route(nameof(UpdateImage))]
        public async Task<PayLoad<object>> UpdateImage([FromForm]RepairDetailsUpdateImage data)
        {
            return await _repairDetailsService.UpdateImage(data);
        }
    }
}
