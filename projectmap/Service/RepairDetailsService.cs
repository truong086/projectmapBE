using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using projectmap.Clouds;
using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;
using System.Runtime.InteropServices;

namespace projectmap.Service
{
    public class RepairDetailsService : IRepairDetailsService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IUserTokenService _userTokenService;
        private readonly Cloud _cloud;
        public RepairDetailsService(DBContext context, IMapper mapper, IUserTokenService userTokenService, IOptions<Cloud> cloud)
        {
            _context = context;
            _mapper = mapper;
            _userTokenService = userTokenService;
            _cloud = cloud.Value;
        }
        public async Task<PayLoad<RepairDetailsDTO>> Add(RepairDetailsDTO data)
        {
            try
            {
                var checkData = _context.repairdetails.FirstOrDefault(x => x.TE_id == data.traff_id && x.RepairStatus != 4 && !x.deleted);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(Status.DATATONTAI));

                var checkDataTraff = _context.trafficequipments.FirstOrDefault(x => x.id == data.traff_id && !x.deleted);
                var user = _userTokenService.name();
                var checkAccunt = _context.users.FirstOrDefault(x => x.id == int.Parse(user) && !x.deleted);
                

                if(checkAccunt == null || checkDataTraff == null)
                    return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(Status.DATANULL));

                var mapData = _mapper.Map<RepairDetails>(data);

                if(data.user_id != 0)
                {
                    var checkAccountEngineer = _context.users.FirstOrDefault(x => x.id == data.user_id);
                    if (checkAccountEngineer == null)
                        return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(Status.DATANULL));

                    mapData.user = checkAccountEngineer;
                    mapData.MaintenanceEngineer = checkAccountEngineer.id;
                }
                
                mapData.trafficEquipment = checkDataTraff;
                mapData.TE_id = checkDataTraff.id;
                mapData.cretoredit = checkAccunt.Name + " Create Data " + DateTime.UtcNow;
                checkDataTraff.UseStatus = data.RepairStatus == 4 ? 1 : 2;

                _context.trafficequipments.Update(checkDataTraff);
                _context.repairdetails.Add(mapData);
                _context.SaveChanges();

                var dataNew = _context.repairdetails.OrderByDescending(x => x.FaultReportingTime).FirstOrDefault();
                if(dataNew == null)
                    return await Task.FromResult(PayLoad<RepairDetailsDTO>.CreatedFail(Status.DATANULL));

                if (data.images != null && data.images.Count > 0)
                {
                    foreach (var image in data.images)
                    {
                        _context.repairrecords.Add(new RepairRecord
                        {
                            Engineer_id = null,
                            user = null,
                            trafficEquipment = checkDataTraff,
                            TE_id = checkDataTraff.id,
                            repairDetails = dataNew,
                            RD_id = dataNew.id,
                            NotificationRecord = data.Remark
                        });
                        _context.SaveChanges();
                        var dataNewRecord = _context.repairrecords.OrderByDescending(x => x.id).FirstOrDefault();
                        if (dataNewRecord != null)
                        {
                            uploadCloud.CloudInaryIFromAccount(image, Status.REPAIRRECORD + "" + dataNew.id.ToString(), _cloud);
                            dataNewRecord.Picture = uploadCloud.Link;
                            dataNewRecord.PublicId = uploadCloud.publicId;

                            _context.repairrecords.Update(dataNewRecord);
                            _context.SaveChanges();
                        }
                    }
                }
                else
                {
                    _context.repairrecords.Add(new RepairRecord
                    {
                        Engineer_id = null,
                        user = null,
                        trafficEquipment = checkDataTraff,
                        TE_id = checkDataTraff.id,
                        repairDetails = dataNew,
                        RD_id = dataNew.id,
                        NotificationRecord = data.Remark
                    });
                    _context.SaveChanges();
                }

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
                var checkData = _context.repairdetails.Include(t => t.trafficEquipment)
                    .Include(r => r.RepairRecords)
                    .ThenInclude(u => u.user)
                    .Where(x => !x.deleted)
                    .Select(x1 => new
                    {
                        paird = x1,
                        RepairRecordsData = x1.RepairRecords.FirstOrDefault(x => x.RD_id == x1.id),
                        RepairRecordsDataToList = x1.RepairRecords
                    })
                    .AsNoTracking()
                    .Select(x => new RepairDetailsGetAll
                    {
                        id = x.paird.id,
                        lat = x.paird.trafficEquipment.Latitude,
                        log = x.paird.trafficEquipment.Longitude,
                        traff_id = x.paird.trafficEquipment.id,
                        ManagementUnit = x.paird.trafficEquipment.ManagementUnit,
                        SignalNumber = x.paird.trafficEquipment.SignalNumber,
                        FaultCodes = x.paird.FaultCodes,
                        RepairStatus = x.paird.RepairStatus,
                        user_id = x.RepairRecordsData.user.id,
                        user_name = x.RepairRecordsData.user.Name,
                        identificationCode = x.paird.trafficEquipment.IdentificationCode,
                        typesOfSignal = x.paird.trafficEquipment.TypesOfSignal,
                        categoryCode = x.paird.trafficEquipment.CategoryCode,
                        images = x.RepairRecordsDataToList.Select(x => x.Picture).ToList()
                    }).ToList();

                if (!string.IsNullOrEmpty(name))
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

        public async Task<PayLoad<object>> FindAllDoneByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var use = _userTokenService.name();

                var checkData = _context.repairdetails.Include(t => t.trafficEquipment)
                     .Include(r => r.RepairRecords)
                     .ThenInclude(u => u.user)
                     .Select(x1 => new
                     {
                         paird = x1,
                         RepairRecordsData = x1.RepairRecords.FirstOrDefault(x => x.RD_id == x1.id),
                         RepairRecordsDataToList = x1.RepairRecords
                     })
                     .Where(x => !x.paird.deleted && x.paird.RepairStatus == 4 && x.RepairRecordsData.user.id == int.Parse(use))
                     .AsNoTracking()
                     .Select(x => new RepairDetailsGetAll
                     {
                         id = x.paird.id,
                         lat = x.paird.trafficEquipment.Latitude,
                         log = x.paird.trafficEquipment.Longitude,
                         traff_id = x.paird.trafficEquipment.id,
                         ManagementUnit = x.paird.trafficEquipment.ManagementUnit,
                         SignalNumber = x.paird.trafficEquipment.SignalNumber,
                         FaultCodes = x.paird.FaultCodes,
                         RepairStatus = x.paird.RepairStatus,
                         user_id = x.RepairRecordsData.user.id,
                         user_name = x.RepairRecordsData.user.Name,
                         identificationCode = x.paird.trafficEquipment.IdentificationCode,
                         typesOfSignal = x.paird.trafficEquipment.TypesOfSignal,
                         categoryCode = x.paird.trafficEquipment.CategoryCode,
                         images = x.RepairRecordsDataToList.Select(x => x.Picture).ToList()
                     }).ToList();

                if (!string.IsNullOrEmpty(name))
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
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllDoneByAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.repairdetails.Include(t => t.trafficEquipment)
                    .Include(r => r.RepairRecords)
                    .ThenInclude(u => u.user)
                    .Where(x => !x.deleted && x.RepairStatus == 4)
                    .Select(x1 => new
                    {
                        paird = x1,
                        RepairRecordsData = x1.RepairRecords.FirstOrDefault(x => x.RD_id == x1.id),
                        RepairRecordsDataToList = x1.RepairRecords
                    })
                    .AsNoTracking()
                    .Select(x => new RepairDetailsGetAll
                    {
                        id = x.paird.id,
                        lat = x.paird.trafficEquipment.Latitude,
                        log = x.paird.trafficEquipment.Longitude,
                        traff_id = x.paird.trafficEquipment.id,
                        ManagementUnit = x.paird.trafficEquipment.ManagementUnit,
                        SignalNumber = x.paird.trafficEquipment.SignalNumber,
                        FaultCodes = x.paird.FaultCodes,
                        RepairStatus = x.paird.RepairStatus,
                        user_id = x.RepairRecordsData.user.id,
                        user_name = x.RepairRecordsData.user.Name,
                        identificationCode = x.paird.trafficEquipment.IdentificationCode,
                        typesOfSignal = x.paird.trafficEquipment.TypesOfSignal,
                        categoryCode = x.paird.trafficEquipment.CategoryCode,
                        images = x.RepairRecordsDataToList.Select(x => x.Picture).ToList()
                    }).ToList();

                if (!string.IsNullOrEmpty(name))
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
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllNoDoneByAccount(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var use = _userTokenService.name();

                var checkData = _context.repairdetails.Include(t => t.trafficEquipment)
                    .Include(r => r.RepairRecords)
                    .ThenInclude(u => u.user)
                    .Select(x1 => new
                    {
                        paird = x1,
                        RepairRecordsData = x1.RepairRecords.FirstOrDefault(x => x.RD_id == x1.id),
                        RepairRecordsDataToList = x1.RepairRecords
                    })
                    .Where(x => !x.paird.deleted && x.paird.RepairStatus != 4 && x.RepairRecordsData.user.id == int.Parse(use))
                    .AsNoTracking()
                    .Select(x => new RepairDetailsGetAll
                    {
                        id = x.paird.id,
                        lat = x.paird.trafficEquipment.Latitude,
                        log = x.paird.trafficEquipment.Longitude,
                        traff_id = x.paird.trafficEquipment.id,
                        ManagementUnit = x.paird.trafficEquipment.ManagementUnit,
                        SignalNumber = x.paird.trafficEquipment.SignalNumber,
                        FaultCodes = x.paird.FaultCodes,
                        RepairStatus = x.paird.RepairStatus,
                        user_id = x.RepairRecordsData.user.id,
                        user_name = x.RepairRecordsData.user.Name,
                        identificationCode = x.paird.trafficEquipment.IdentificationCode,
                        typesOfSignal = x.paird.trafficEquipment.TypesOfSignal,
                        categoryCode = x.paird.trafficEquipment.CategoryCode,
                        images = x.RepairRecordsDataToList.Select(x => x.Picture).ToList()
                    }).ToList();

                if (!string.IsNullOrEmpty(name))
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
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllNoDoneByAdmin(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.repairdetails.Include(t => t.trafficEquipment)
                    .Include(r => r.RepairRecords)
                    .ThenInclude(u => u.user)
                    .Where(x => !x.deleted && x.RepairStatus != 4)
                    .Select(x1 => new
                    {
                        paird = x1,
                        RepairRecordsData = x1.RepairRecords.FirstOrDefault(x => x.RD_id == x1.id),
                        RepairRecordsDataToList = x1.RepairRecords
                    })
                    .AsNoTracking()
                    .Select(x => new RepairDetailsGetAll
                    {
                        id = x.paird.id,
                        lat = x.paird.trafficEquipment.Latitude,
                        log = x.paird.trafficEquipment.Longitude,
                        traff_id = x.paird.trafficEquipment.id,
                        ManagementUnit = x.paird.trafficEquipment.ManagementUnit,
                        SignalNumber = x.paird.trafficEquipment.SignalNumber,
                        FaultCodes = x.paird.FaultCodes,
                        RepairStatus = x.paird.RepairStatus,
                        user_id = x.RepairRecordsData.user.id,
                        user_name = x.RepairRecordsData.user.Name,
                        identificationCode = x.paird.trafficEquipment.IdentificationCode,
                        typesOfSignal = x.paird.trafficEquipment.TypesOfSignal,
                        categoryCode = x.paird.trafficEquipment.CategoryCode,
                        images = x.RepairRecordsDataToList.Select(x => x.Picture).ToList()
                    }).ToList();

                if (!string.IsNullOrEmpty(name))
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
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindOneId(int id)
        {
            try
            {
                var checkData = _context.repairdetails.Include(t => t.trafficEquipment)
                    .Include(r => r.RepairRecords)
                    .ThenInclude(u => u.user)
                    .Where(x => x.id == id && !x.deleted && x.RepairStatus != 4)
                    .Select(x1 => new
                    {
                       repai = x1,
                        RepairRecordsData = x1.RepairRecords.FirstOrDefault(x => x.RD_id == x1.id),
                        RepairRecordsDataToList = x1.RepairRecords
                    })
                    .AsNoTracking()
                    .Select(x => new RepairDetailsGetAll
                    {
                        id = x.repai.id,
                        lat = x.repai.trafficEquipment.Latitude,
                        log = x.repai.trafficEquipment.Longitude,
                        traff_id = x.repai.trafficEquipment.id,
                        ManagementUnit = x.repai.trafficEquipment.ManagementUnit,
                        SignalNumber = x.repai.trafficEquipment.SignalNumber,
                        FaultCodes = x.repai.FaultCodes,
                        RepairStatus = x.repai.RepairStatus,
                        user_id = x.RepairRecordsData.user.id,
                        user_name = x.RepairRecordsData.user.Name,
                        identificationCode = x.repai.trafficEquipment.IdentificationCode,
                        typesOfSignal = x.repai.trafficEquipment.TypesOfSignal,
                        categoryCode = x.repai.trafficEquipment.CategoryCode,
                        images = x.RepairRecordsDataToList.Select(x => x.Picture).ToList()

                    }).FirstOrDefault();
                if (checkData == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(checkData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<RepairDetailsUpdate>> Update(RepairDetailsUpdate data)
        {
            try
            {
                var use = _userTokenService.name();
                var checkData = _context.repairdetails.FirstOrDefault(x => x.id == data.id && x.RepairStatus == 1 && !x.deleted);
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(use) && !x.deleted);
                
                if (checkData == null || checkAccount == null)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdate>.CreatedFail(Status.DATANULL));

                checkData.RepairStatus = 2;
                checkData.cretoredit = checkData.cretoredit + ", " + checkAccount.Name + " Update " + DateTime.UtcNow;

                _context.repairdetails.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RepairDetailsUpdate>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RepairDetailsUpdate>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByAccout(RepairDetailsUpdateByAccont data)
        {
            try
            {
                var user = _userTokenService.name();
                var checkData = _context.repairdetails.FirstOrDefault(x => x.id == data.id && x.RepairStatus != 4 && !x.deleted);
                if(checkData == null)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                if(data.status > 3 || data.status < 0)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                var checkRecordRepairDetails = _context.repairrecords.Include(u => u.user).FirstOrDefault(x => x.RD_id == checkData.id);
                if(checkRecordRepairDetails == null)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                var checkDataTraff = _context.trafficequipments.FirstOrDefault(x => x.id == checkData.TE_id);
                if(checkDataTraff == null)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                var checkAccountUpdateData = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user));
                if(checkAccountUpdateData == null || checkAccountUpdateData.id != checkData.MaintenanceEngineer)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                if (data.status == 4)
                    checkData.FaultCodes = 0;
                else checkData.FaultCodes = data.FaultCodes;

                checkData.RepairStatus = data.status;
                checkData.cretoredit = checkData.cretoredit + ", " + checkAccountUpdateData.Name + " Update Status " + DateTime.UtcNow;
                checkDataTraff.UseStatus = data.status == 4 ? 1 : 2;

                _context.trafficequipments.Update(checkDataTraff);
                _context.repairdetails.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByClose(RepairDetailsUpdateByAccont data)
        {
            try
            {
                var checkId = _context.repairdetails.FirstOrDefault(x => x.id == data.id);
                if(checkId == null)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                checkId.RepairStatus = data.status;

                _context.repairdetails.Update(checkId);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<RepairDetailsUpdateByAccont>> UpdateByOffClose(RepairDetailsUpdateByAccont data)
        {
            try
            {
                var checkData = _context.repairdetails.FirstOrDefault(x => x.id == data.id);
                if(checkData == null)
                    return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(Status.DATANULL));

                if (checkData.MaintenanceEngineer == null)
                    checkData.RepairStatus = 1;
                else checkData.RepairStatus = 2;

                _context.repairdetails.Update(checkData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<RepairDetailsUpdateByAccont>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<ConfirmData>> UpdateConfimData(ConfirmData data)
        {
            try
            {
                var user = _userTokenService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == int.Parse(user));
                var checkAccountEngineer = _context.users.FirstOrDefault(x => x.id == data.id_user);
                var checkRecordDetails = _context.repairdetails.FirstOrDefault(x => x.id == data.id);

                if (checkAccount == null || checkAccountEngineer == null || checkRecordDetails == null)
                    return await Task.FromResult(PayLoad<ConfirmData>.CreatedFail(Status.DATANULL));

                var checkDataRecordRepair = _context.repairrecords.FirstOrDefault(x => x.RD_id == checkRecordDetails.id);
                if (checkDataRecordRepair == null)
                    return await Task.FromResult(PayLoad<ConfirmData>.CreatedFail(Status.DATANULL));

                checkDataRecordRepair.user = checkAccountEngineer;
                checkDataRecordRepair.Engineer_id = checkAccountEngineer.id;
                checkRecordDetails.MaintenanceEngineer = checkAccountEngineer.id;
                checkRecordDetails.user = checkAccountEngineer;
                checkRecordDetails.RepairStatus = 2;
                checkRecordDetails.cretoredit += ", " + checkAccount.Name + " Create " + DateTime.UtcNow;

                _context.repairrecords.Update(checkDataRecordRepair);
                _context.repairdetails.Update(checkRecordDetails);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<ConfirmData>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ConfirmData>.CreatedFail(ex.Message));
            }
        }
    }
}