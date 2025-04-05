using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;
using System.Globalization;
using System.Xml;

namespace projectmap.Service
{
    public class TrafficEquipmentService : ITrafficEquipmentService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        public TrafficEquipmentService(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PayLoad<TrafficEquipmentDTO>> Add(TrafficEquipmentDTO data)
        {
            try
            {
                var checkData = _context.trafficequipments.FirstOrDefault(x => x.Longitude == data.Longitude && x.Latitude == data.Latitude && !x.deleted);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<TrafficEquipmentDTO>.CreatedFail(Status.DATATONTAI));

                var mapData = _mapper.Map<TrafficEquipment>(data);
                //mapData.CategoryCode = RanDomCode.geneActionInteGer(7);
                //mapData.IdentificationCode = Convert.ToDouble(RanDomCode.geneActionInteGer(15));
                _context.trafficequipments.Add(mapData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<TrafficEquipmentDTO>.Successfully(data));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<TrafficEquipmentDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> AddFile(string filePath)
        {
            var equipments = new List<TrafficEquipment>();
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList placemarks = doc.GetElementsByTagName("Placemark");

            foreach (XmlNode placemark in placemarks)  
            {
                var equipment = new TrafficEquipment();

                // Lấy dữ liệu từ ExtendedData
                //XmlNodeList simpleDataList = placemark.SelectNodes(".//SimpleData");
                XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

                XmlNodeList simpleDataList = placemark.SelectNodes(".//kml:SimpleData", nsManager);
                foreach (XmlNode node in simpleDataList)
                {
                    string name = node.Attributes["name"].Value;
                    string value = node.InnerText.Trim();

                    switch (name)
                    {
                        case "類別碼": equipment.CategoryCode = int.Parse(value); break;
                        case "識別碼": equipment.IdentificationCode = double.Parse(value); break;
                        case "管理單位": equipment.ManagementUnit = value; break;
                        case "作業區分": equipment.JobClassification = int.Parse(value); break;
                        case "timePosition":
                            DateTime parsedDate;
                            if (DateTime.TryParseExact(value, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                equipment.timePosition = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                            }
                            break;
                        case "號誌編號": equipment.SignalNumber = value; break;
                        case "號誌種類": equipment.TypesOfSignal = value; break;
                        case "號誌架設方式": equipment.SignalInstallation = int.Parse(value); break;
                        case "長度": equipment.Length = double.Parse(value); break;
                        case "使用狀態": equipment.UseStatus = int.Parse(value); break;
                        case "資料狀態": equipment.DataStatus = int.Parse(value); break;
                        case "備註": equipment.Remark = value; break;
                    }
                }


                // Lấy tọa độ
                XmlNode coordNode = placemark.SelectSingleNode(".//kml:coordinates", nsManager);
                if (coordNode != null)
                {
                    string[] coords = coordNode.InnerText.Trim().Split(',');
                    if (coords.Length >= 2)
                    {
                        equipment.Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        equipment.Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                    }
                }

                _context.trafficequipments.Add(equipment);
                _context.SaveChanges();
                equipments.Add(equipment);
            }

            return await Task.FromResult(PayLoad<object>.Successfully(new
            {
                data = Status.SUCCESS
            }));
        }

        public async Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments
    .Where(x => !x.deleted)
    .AsNoTracking()
    .Select(x => new
    {
        x.id,
        x.CategoryCode,
        x.IdentificationCode,
        x.ManagementUnit,
        x.JobClassification,
        x.SignalNumber,
        x.TypesOfSignal,
        x.SignalInstallation,
        x.UseStatus,
        x.DataStatus,
        x.Remark,
        x.Length,
        x.Longitude,
        x.Latitude,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        isError = x.FirstRepair != null,
        statusError = x.FirstRepair.RepairStatus ?? 0,
        CategoryCode = x.CategoryCode,
        IdentificationCode = x.IdentificationCode,
        ManagementUnit = x.ManagementUnit,
        JobClassification = x.JobClassification,
        SignalNumber = x.SignalNumber,
        TypesOfSignal = x.TypesOfSignal,
        SignalInstallation = x.SignalInstallation,
        UseStatus = x.UseStatus,
        DataStatus = x.DataStatus,
        Remark = x.Remark,
        Length = x.Length,
        Longitude = x.Longitude,
        Latitude = x.Latitude,
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
    })
    .ToList();
                /*var checkData = _context.trafficEquipments
                     .Include(x => x.RepairDetails)
                     .ThenInclude(r => r.RepairRecords)
                     .ThenInclude(u => u.user)
                     .Where(x => !x.deleted)
                     .Select(x => new
                     {
                         Equipment = x,
                         FirstRepair = x.RepairDetails.FirstOrDefault(x1 => x1.TE_id == x.id && !x1.deleted)
                     })
                     .AsNoTracking()
                     .Select(x => new TrafficEquipmentGetAll
                     {
                         id = x.Equipment.id,
                         isError = x.FirstRepair != null,
                         statusError = x.FirstRepair.RepairStatus ?? 0,
                         CategoryCode = x.Equipment.CategoryCode,
                         IdentificationCode = x.Equipment.IdentificationCode,
                         ManagementUnit = x.Equipment.ManagementUnit,
                         JobClassification = x.Equipment.JobClassification,
                         SignalNumber = x.Equipment.SignalNumber,
                         TypesOfSignal = x.Equipment.TypesOfSignal,
                         SignalInstallation = x.Equipment.SignalInstallation,
                         UseStatus = x.Equipment.UseStatus,
                         DataStatus = x.Equipment.DataStatus,
                         Remark = x.Equipment.Remark,
                         Length = x.Equipment.Length,
                         Longitude = x.Equipment.Longitude,
                         Latitude = x.Equipment.Latitude,
                         account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecords.FirstOrDefault(x2 => x2.RD_id == x.FirstRepair.id).user.Name : null,
                     }).ToList(); 
                */

                //var checkData = _context.trafficEquipments
                //    .Select(x => new
                //    {
                //        Equipment = x,
                //        FirstRepair = _context.repairDetails
                //                              .Where(r => r.TE_id == x.id)
                //                              .OrderBy(r => r.id)
                //                              .Select(r => new { r.RepairStatus }) // Chỉ lấy cột cần thiết
                //                              .FirstOrDefault()
                //    })
                //    .AsNoTracking() // ✅ Tăng tốc bằng cách không theo dõi trạng thái entity
                //    .ToList()
                //    .Select(x => new TrafficEquipmentGetAll
                //    {
                //        isError = x.FirstRepair != null,
                //        statusError = x.FirstRepair?.RepairStatus ?? 0, // ✅ Tránh lỗi null
                //        CategoryCode = x.Equipment.CategoryCode,
                //        IdentificationCode = x.Equipment.IdentificationCode,
                //        ManagementUnit = x.Equipment.ManagementUnit,
                //        JobClassification = x.Equipment.JobClassification,
                //        SignalNumber = x.Equipment.SignalNumber,
                //        TypesOfSignal = x.Equipment.TypesOfSignal,
                //        SignalInstallation = x.Equipment.SignalInstallation,
                //        UseStatus = x.Equipment.UseStatus,
                //        DataStatus = x.Equipment.DataStatus,
                //        Remark = x.Equipment.Remark,
                //        Length = x.Equipment.Length,
                //        Longitude = x.Equipment.Longitude,
                //        Latitude = x.Equipment.Latitude
                //    }).ToList();

                if (!string.IsNullOrEmpty(name))
                {
                    string[] coords = name.Trim().Split(',');
                    if(coords.Count() == 2)
                    {
                        var Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        var Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                        checkData = checkData.Where(x => x.Latitude == Latitude && x.Longitude == Longitude).ToList();
                    }
                    else if (int.TryParse(name, out int n) )
                    {
                        checkData = checkData.Where(x => x.CategoryCode == n).ToList();
                    }
                    else
                    {
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();
                    }

                    
                }
                    

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

        public async Task<PayLoad<object>> FindAllErrorCode0(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments
    .Where(x => !x.deleted)
    .AsNoTracking()
    .Select(x => new
    {
        x.id,
        x.CategoryCode,
        x.IdentificationCode,
        x.ManagementUnit,
        x.JobClassification,
        x.SignalNumber,
        x.TypesOfSignal,
        x.SignalInstallation,
        x.UseStatus,
        x.DataStatus,
        x.Remark,
        x.Length,
        x.Longitude,
        x.Latitude,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        isError = x.FirstRepair != null,
        statusError = x.FirstRepair.RepairStatus ?? 0,
        CategoryCode = x.CategoryCode,
        IdentificationCode = x.IdentificationCode,
        ManagementUnit = x.ManagementUnit,
        JobClassification = x.JobClassification,
        SignalNumber = x.SignalNumber,
        TypesOfSignal = x.TypesOfSignal,
        SignalInstallation = x.SignalInstallation,
        UseStatus = x.UseStatus,
        DataStatus = x.DataStatus,
        Remark = x.Remark,
        Length = x.Length,
        Longitude = x.Longitude,
        Latitude = x.Latitude,
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
    })
    .Where(x => x.isError == true && x.statusError == 0)
    .ToList();
              

                if (!string.IsNullOrEmpty(name))
                {
                    string[] coords = name.Trim().Split(',');
                    if (coords.Count() == 2)
                    {
                        var Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        var Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                        checkData = checkData.Where(x => x.Latitude == Latitude && x.Longitude == Longitude).ToList();
                    }
                    else if (int.TryParse(name, out int n))
                    {
                        checkData = checkData.Where(x => x.CategoryCode == n).ToList();
                    }
                    else
                    {
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();
                    }


                }


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

        public async Task<PayLoad<object>> FindAllErrorCode1(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments
    .Where(x => !x.deleted)
    .AsNoTracking()
    .Select(x => new
    {
        x.id,
        x.CategoryCode,
        x.IdentificationCode,
        x.ManagementUnit,
        x.JobClassification,
        x.SignalNumber,
        x.TypesOfSignal,
        x.SignalInstallation,
        x.UseStatus,
        x.DataStatus,
        x.Remark,
        x.Length,
        x.Longitude,
        x.Latitude,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        isError = x.FirstRepair != null,
        statusError = x.FirstRepair.RepairStatus ?? 0,
        CategoryCode = x.CategoryCode,
        IdentificationCode = x.IdentificationCode,
        ManagementUnit = x.ManagementUnit,
        JobClassification = x.JobClassification,
        SignalNumber = x.SignalNumber,
        TypesOfSignal = x.TypesOfSignal,
        SignalInstallation = x.SignalInstallation,
        UseStatus = x.UseStatus,
        DataStatus = x.DataStatus,
        Remark = x.Remark,
        Length = x.Length,
        Longitude = x.Longitude,
        Latitude = x.Latitude,
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
    })
    .Where(x => x.isError == true && x.statusError == 1)
    .ToList();


                if (!string.IsNullOrEmpty(name))
                {
                    string[] coords = name.Trim().Split(',');
                    if (coords.Count() == 2)
                    {
                        var Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        var Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                        checkData = checkData.Where(x => x.Latitude == Latitude && x.Longitude == Longitude).ToList();
                    }
                    else if (int.TryParse(name, out int n))
                    {
                        checkData = checkData.Where(x => x.CategoryCode == n).ToList();
                    }
                    else
                    {
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();
                    }


                }


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

        public async Task<PayLoad<object>> FindAllErrorCode2(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments
    .Where(x => !x.deleted)
    .AsNoTracking()
    .Select(x => new
    {
        x.id,
        x.CategoryCode,
        x.IdentificationCode,
        x.ManagementUnit,
        x.JobClassification,
        x.SignalNumber,
        x.TypesOfSignal,
        x.SignalInstallation,
        x.UseStatus,
        x.DataStatus,
        x.Remark,
        x.Length,
        x.Longitude,
        x.Latitude,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        isError = x.FirstRepair != null,
        statusError = x.FirstRepair.RepairStatus ?? 0,
        CategoryCode = x.CategoryCode,
        IdentificationCode = x.IdentificationCode,
        ManagementUnit = x.ManagementUnit,
        JobClassification = x.JobClassification,
        SignalNumber = x.SignalNumber,
        TypesOfSignal = x.TypesOfSignal,
        SignalInstallation = x.SignalInstallation,
        UseStatus = x.UseStatus,
        DataStatus = x.DataStatus,
        Remark = x.Remark,
        Length = x.Length,
        Longitude = x.Longitude,
        Latitude = x.Latitude,
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
    })
    .Where(x => x.isError == true && x.statusError == 2)
    .ToList();


                if (!string.IsNullOrEmpty(name))
                {
                    string[] coords = name.Trim().Split(',');
                    if (coords.Count() == 2)
                    {
                        var Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        var Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                        checkData = checkData.Where(x => x.Latitude == Latitude && x.Longitude == Longitude).ToList();
                    }
                    else if (int.TryParse(name, out int n))
                    {
                        checkData = checkData.Where(x => x.CategoryCode == n).ToList();
                    }
                    else
                    {
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();
                    }


                }


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

        public async Task<PayLoad<object>> FindAllErrorCode3(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments
    .Where(x => !x.deleted)
    .AsNoTracking()
    .Select(x => new
    {
        x.id,
        x.CategoryCode,
        x.IdentificationCode,
        x.ManagementUnit,
        x.JobClassification,
        x.SignalNumber,
        x.TypesOfSignal,
        x.SignalInstallation,
        x.UseStatus,
        x.DataStatus,
        x.Remark,
        x.Length,
        x.Longitude,
        x.Latitude,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        isError = x.FirstRepair != null,
        statusError = x.FirstRepair.RepairStatus ?? 0,
        CategoryCode = x.CategoryCode,
        IdentificationCode = x.IdentificationCode,
        ManagementUnit = x.ManagementUnit,
        JobClassification = x.JobClassification,
        SignalNumber = x.SignalNumber,
        TypesOfSignal = x.TypesOfSignal,
        SignalInstallation = x.SignalInstallation,
        UseStatus = x.UseStatus,
        DataStatus = x.DataStatus,
        Remark = x.Remark,
        Length = x.Length,
        Longitude = x.Longitude,
        Latitude = x.Latitude,
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
    })
    .Where(x => x.isError == true && x.statusError == 3)
    .ToList();


                if (!string.IsNullOrEmpty(name))
                {
                    string[] coords = name.Trim().Split(',');
                    if (coords.Count() == 2)
                    {
                        var Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        var Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                        checkData = checkData.Where(x => x.Latitude == Latitude && x.Longitude == Longitude).ToList();
                    }
                    else if (int.TryParse(name, out int n))
                    {
                        checkData = checkData.Where(x => x.CategoryCode == n).ToList();
                    }
                    else
                    {
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();
                    }


                }


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

        public async Task<PayLoad<object>> FindAllNoError(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments
    .Where(x => !x.deleted)
    .AsNoTracking()
    .Select(x => new
    {
        x.id,
        x.CategoryCode,
        x.IdentificationCode,
        x.ManagementUnit,
        x.JobClassification,
        x.SignalNumber,
        x.TypesOfSignal,
        x.SignalInstallation,
        x.UseStatus,
        x.DataStatus,
        x.Remark,
        x.Length,
        x.Longitude,
        x.Latitude,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefault()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        isError = x.FirstRepair != null,
        statusError = x.FirstRepair.RepairStatus ?? 0,
        CategoryCode = x.CategoryCode,
        IdentificationCode = x.IdentificationCode,
        ManagementUnit = x.ManagementUnit,
        JobClassification = x.JobClassification,
        SignalNumber = x.SignalNumber,
        TypesOfSignal = x.TypesOfSignal,
        SignalInstallation = x.SignalInstallation,
        UseStatus = x.UseStatus,
        DataStatus = x.DataStatus,
        Remark = x.Remark,
        Length = x.Length,
        Longitude = x.Longitude,
        Latitude = x.Latitude,
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
    })
    .Where(x => x.isError == false)
    .ToList();


                if (!string.IsNullOrEmpty(name))
                {
                    string[] coords = name.Trim().Split(',');
                    if (coords.Count() == 2)
                    {
                        var Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture);
                        var Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture);
                        checkData = checkData.Where(x => x.Latitude == Latitude && x.Longitude == Longitude).ToList();
                    }
                    else if (int.TryParse(name, out int n))
                    {
                        checkData = checkData.Where(x => x.CategoryCode == n).ToList();
                    }
                    else
                    {
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name)).ToList();
                    }


                }


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

        public async Task<PayLoad<object>> FindAllTest(string? name, int page = 1, int pageSize = 20)
        {
            try
            {
                var checkData = _context.trafficequipments.Where(x => !x.deleted)
                     .ToList();

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
                    var checkData = _context.trafficequipments
                    .Where(x => !x.deleted && x.id == id)
                    .AsNoTracking()
                    .Select(x => new
                    {
                        x.id,
                        x.CategoryCode,
                        x.IdentificationCode,
                        x.ManagementUnit,
                        x.JobClassification,
                        x.SignalNumber,
                        x.TypesOfSignal,
                        x.SignalInstallation,
                        x.UseStatus,
                        x.DataStatus,
                        x.Remark,
                        x.Length,
                        x.Longitude,
                        x.Latitude,
                        FirstRepair = x.RepairDetails
                            .Where(r => r.TE_id == x.id && !r.deleted)
                            .OrderBy(r => r.id)
                            .Select(r => new
                            {
                                r.RepairStatus,
                                RepairRecord = r.RepairRecords
                                    .OrderBy(rr => rr.id)
                                    .Select(rr => rr.user.Name)
                                    .FirstOrDefault()
                            })
                            .FirstOrDefault()
                    })
                    .Select(x => new TrafficEquipmentGetAll
                    {
                        id = x.id,
                        isError = x.FirstRepair != null,
                        statusError = x.FirstRepair.RepairStatus ?? 0,
                        CategoryCode = x.CategoryCode,
                        IdentificationCode = x.IdentificationCode,
                        ManagementUnit = x.ManagementUnit,
                        JobClassification = x.JobClassification,
                        SignalNumber = x.SignalNumber,
                        TypesOfSignal = x.TypesOfSignal,
                        SignalInstallation = x.SignalInstallation,
                        UseStatus = x.UseStatus,
                        DataStatus = x.DataStatus,
                        Remark = x.Remark,
                        Length = x.Length,
                        Longitude = x.Longitude,
                        Latitude = x.Latitude,
                        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null
                    })
                    .FirstOrDefault(x => x.isError == false);

                if(checkData == null)
                    await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(checkData));
            }
                catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
