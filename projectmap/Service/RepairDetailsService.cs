using AutoMapper;
using Microsoft.EntityFrameworkCore;
using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public class RepairDetailsService : IRepairDetailsService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserTokenService _userTokenService;
        public RepairDetailsService(DBContext context, IMapper mapper, IUserTokenService userTokenService)
        {
            _context = context;
            _mapper = mapper;
            _userTokenService = userTokenService;
        }
        public async Task<PayLoad<RepairDetailsDTO>> Add(RepairDetailsDTO data)
        {
            try
            {
                var checkData = _context.repairDetails.FirstOrDefault(x => x.TE_id == data.traff_id && x.RepairStatus != 3);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(Status.DATATONTAI));

                var checkDataTraff = _context.trafficEquipments.FirstOrDefault(x => x.id == data.traff_id);
                var user = _userTokenService.name();
                var checkAccunt = _context.users.FirstOrDefault(x => x.id == int.Parse(user));

                if(checkAccunt == null || checkDataTraff == null)
                    return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<RepairDetails>(data);
                mapData.MaintenanceEngineer = checkAccunt.id;
                mapData.user = checkAccunt;
                mapData.trafficEquipment = checkDataTraff;
                mapData.TE_id = checkDataTraff.id;

                _context.repairDetails.Add(mapData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RepairDetailsDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.repairDetails.Include(u => u.user).Include(t => t.trafficEquipment)
                    .Select(x => new RepairDetailsGetAll
                    {
                        lat = x.trafficEquipment.Latitude,
                        log = x.trafficEquipment.Longitude,
                        traff_id = x.trafficEquipment.id,
                        ManagementUnit = x.trafficEquipment.ManagementUnit,
                        SignalNumber = x.trafficEquipment.SignalNumber,
                        FaultCodes = x.FaultCodes,
                        RepairStatus = x.RepairStatus,
                        user_id  = x.user.id,
                        user_name = x.user.Name,
                        Remark = x.Remark
                    }).ToList();

                if(!string.IsNullOrEmpty(name))
                    checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();

                var pageList = new PageList<object>(checkData, page - 1, pageSize);

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = pageList,
                    page,
                    pageList.pageSize,
                    pageList.totalCounts,
                    pageList.totalPages
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
