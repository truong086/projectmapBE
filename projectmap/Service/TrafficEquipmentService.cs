using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace projectmap.Service
{
    public class TrafficEquipmentService : ITrafficEquipmentService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly string _apiKey = "AIzaSyBkgBNM7Mtgg6I3SvhOlwZCgqp7vFAPrS8";
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

        public async Task<PayLoad<object>> AddTest()
        {
            var datas = _context.trafficequipments.Where(x => x.Area_Level3_1 == null).ToList();

            foreach(var data in datas)
            {

            
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={data.Latitude},{data.Longitude}&key={_apiKey}&language=zh-TW";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var json = JObject.Parse(response);

                    var status = json["status"]?.ToString();
                    if (status == "OK")
                    {
                        var list = new List<TrafficEquipment>();
                        var results = json["results"];
                        if (results != null)
                        {
                            foreach (var result in results)
                            {
                                //var components = json["results"]?[0]?["address_components"];
                                var components = result["address_components"] as JArray;
                                if (HasRequiredComponents(components))
                                {
                                    foreach (var component in components)
                                    {
                                        var types = component["types"]?.ToObject<List<string>>();
                                        if (types != null)
                                        {
                                            if (data.District_1 == null || data.Area_Level3_1 == null || data.Road_1 == null)
                                            {
                                                if (data.District_1 == null && types.Contains("administrative_area_level_2"))
                                                {
                                                    data.District_1 = component["long_name"]?.ToString();
                                                }

                                                if (data.Area_Level3_1 == null && types.Contains("administrative_area_level_3"))
                                                {
                                                    data.Area_Level3_1 = component["long_name"]?.ToString();
                                                }

                                                if (data.Road_1 == null && types.Contains("route"))
                                                {
                                                    data.Road_1 = component["long_name"]?.ToString();
                                                }
                                            }
                                            else if (data.District_1 != null && data.Area_Level3_1 != null && data.Road_1 != null)
                                            {

                                                if (types.Contains("route") && data.Road_1 == component["long_name"]?.ToString() && data.Road_2 == null)
                                                {
                                                    data.District_2 = null;
                                                    data.Area_Level3_2 = null;
                                                    data.Road_2 = null;

                                                    continue;
                                                }
                                                if (data.District_2 == null && types.Contains("administrative_area_level_2"))
                                                {
                                                    data.District_2 = component["long_name"]?.ToString();
                                                }

                                                if (data.Area_Level3_2 == null && types.Contains("administrative_area_level_3"))
                                                {
                                                    data.Area_Level3_2 = component["long_name"]?.ToString();
                                                }

                                                if (data.Road_2 == null && types.Contains("route") && data.Road_1 != component["long_name"]?.ToString())
                                                {
                                                    data.Road_2 = component["long_name"]?.ToString();
                                                }
                                            }


                                            //string streetName = component["long_name"]?.ToString();
                                            //string shortName = component["short_name"]?.ToString();
                                            if (data.District_1 != null 
                                                && data.District_2 != null 
                                                && data.Area_Level3_1 != null 
                                                && data.Area_Level3_2 != null 
                                                && data.Road_1 != null 
                                                && data.Road_2 != null)
                                            {
                                                list.Add(data);
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (data.District_1 != null 
                                    && data.District_2 != null 
                                    && data.Area_Level3_1 != null 
                                    && data.Area_Level3_2 != null 
                                    && data.Road_1 != null 
                                    && data.Road_2 != null)
                                {
                                    break;
                                }

                            }

                        }

                        //var address = json["results"]?[0]?["formatted_address"]?.ToString();
                        
                    }
                    
                }

                if(data.Road_2 == null)
                {
                    data.Road_2 = data.Road_1;
                    data.District_2 = data.District_1;
                    data.Area_Level3_2 = data.Area_Level3_1;
                }
                _context.trafficequipments.Update(data);
                _context.SaveChanges();
            }
            return await Task.FromResult(PayLoad<object>.Successfully(new
            {
                data = "Success"
            }));

        }

        // Hàm kiểm tra xem components có đủ các loại yêu cầu không
        private bool HasRequiredComponents(JArray components)
        {
            var requiredTypes = new HashSet<string>
    {
        //"route"
        //"administrative_area_level_2",
        "administrative_area_level_3"
    };

            var foundTypes = new HashSet<string>();

            foreach (var component in components)
            {
                var types = component["types"]?.ToObject<List<string>>();
                if (types == null) continue;

                foreach (var type in types)
                {
                    if (requiredTypes.Contains(type))
                        foundTypes.Add(type);
                }
            }

            return requiredTypes.All(type => foundTypes.Contains(type));
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus != 3)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                    date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1
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
                        checkData = checkData.Where(x => x.SignalNumber.Contains(name) || x.ManagementUnit.Contains(name) || x.road1.Contains(name) || x.road2.Contains(name) || x.districs.Contains(name)).ToList();
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
    })
    .Where(x => x.isError == true && x.statusError == 4)
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
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
                        x.Road_1,
                        x.Road_2,
                        x.District_1,
                        FirstRepair = x.RepairDetails
                            .Where(r => r.TE_id == x.id && !r.deleted)
                            .OrderBy(r => r.id)
                            .Select(r => new
                            {
                                r.id,
                                r.RepairStatus,
                                r.FaultCodes,
                                RepairRecord = r.RepairRecords
                                    .OrderBy(rr => rr.id)
                                    .Select(rr => rr.user.Name)
                                    .FirstOrDefault(),
                                date = r.FaultReportingTime
                            })
                            .FirstOrDefault(),
                        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
                        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
                        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
                        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
                        images = x.imageData.ToList(),
                        totalUpdate = x.totalEdit,
                        date = x.FirstRepair.date,
                        dateUpdate = x.FirstRepairUpdate.date,
                        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
                        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
                        isErrorUpdate = x.FirstRepairUpdate != null,
                        road1 = x.Road_1,
                        road2 = x.Road_2,
                        statusErrorFauCode = x.FirstRepair.FaultCodes,
                        districs = x.District_1,
                        repaiDetail_id = x.FirstRepair.id
                    })
                    .FirstOrDefault(x => x.isError == false || (x.isError == true && x.statusError == 4));

                if(checkData == null)
                    await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                return await Task.FromResult(PayLoad<object>.Successfully(checkData));
            }
                catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllErrorCode0ByAccount(int id, int page = 1, int pageSize = 20)
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => new
                    {
                        rr.user.Name,
                        rr.user.id
                    })
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
    })
    .Select(x => new TrafficEquipmentGetAll
    {
        id = x.id,
        id_Engineer = x.FirstRepair.RepairRecord.id,
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord.Name : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
    })
    .Where(x => x.isError == true && x.statusError != 4 && x.id_Engineer == id)
                .ToList();

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

        public async Task<PayLoad<object>> FindAllErrorCode321(string? name, int page = 1, int pageSize = 20)
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.id,
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1,
        repaiDetail_id = x.FirstRepair.id
    })
    .Where(x => x.isError == true && (x.statusError == 3 || x.statusError == 2 || x.statusError == 1))
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

        public async Task<PayLoad<object>> GetNormal()
        {
            try
            {
                int total = _context.trafficequipments.Where(x => !x.deleted).Count();

                // Số đèn bị lỗi(distinct để tránh đếm trùng nếu có nhiều lỗi)
                int errorLights = _context.repairdetails.Where(x => x.RepairStatus != 4 && x.RepairStatus != 5 && !x.deleted).Select(x => x.id).Distinct().Count();

                if(total == 0)
                    return await Task.FromResult(PayLoad<object>.Successfully(0));

                double percentage = ((double)(total - errorLights) / total) * 100;

                return await Task.FromResult(PayLoad<object>.Successfully(Math.Round(percentage, 2))); // Làm tròn 2 chữ số sau dấu phẩy
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetNormalDistrict()
        {
            try
            {
                var list = Status.listDataDisrtric;
                if (list.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var listData = new Dictionary<string, int>();
                foreach(var item in list)
                {
                    var dataCount = _context.repairdetails.Where(x => x.RepairStatus != 4 && x.RepairStatus != 5 && !x.deleted).AsNoTracking()
                        .Select(x => new
                        {
                            name1 = x.trafficEquipment.District_1,
                            name2 = x.trafficEquipment.District_2
                        }).Select(x => new
                        {
                            district1 = x.name1,
                            district2 = x.name2
                        }).Where(x2 => x2.district1 == item || x2.district2 == item).Count();

                    listData.Add(item, dataCount);
                }

                return await Task.FromResult(PayLoad<object>.Successfully(listData));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            } 
        }

        public async Task<PayLoad<object>> GetNormalDistrictUpdate()
        {
            try
            {
                var list = Status.listDataDisrtric;
                if (list.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var listData = new Dictionary<string, int>();
                foreach (var item in list)
                {
                    var dataCount = _context.repairdetails.Where(x => x.RepairStatus == 3 && !x.deleted).AsNoTracking()
                        .Select(x => new
                        {
                            name1 = x.trafficEquipment.District_1,
                            name2 = x.trafficEquipment.District_2
                        }).Select(x => new
                        {
                            district1 = x.name1,
                            district2 = x.name2
                        }).Where(x2 => x2.district1 == item || x2.district2 == item).Count();

                    listData.Add(item, dataCount);
                }

                return await Task.FromResult(PayLoad<object>.Successfully(listData));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetNormalDistrict2()
        {
            try
            {
                var list = Status.listDataDisrtric;
                if (list.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var listData = new Dictionary<string, double>();
                var total = _context.trafficequipments.Where(x => !x.deleted).Count();
                foreach (var item in list)
                {
                    var dataCount = _context.repairdetails.Where(x => x.RepairStatus != 4 && x.RepairStatus != 5 && !x.deleted).AsNoTracking()
                        .Select(x => new
                        {
                            name1 = x.trafficEquipment.District_1,
                            name2 = x.trafficEquipment.District_2
                        }).Select(x => new
                        {
                            district1 = x.name1,
                            district2 = x.name2
                        }).Where(x2 => x2.district1 == item || x2.district2 == item).Count();

                    //double totalNumber = total == 0 ? 0 : (dataCount * 100.0) / total; // Tính phần trăm đèn bị hỏng trong tổng số đèn
                    var totalNumber = ((double)(total - dataCount) / total) * 100; // Tính phần số đèn chưa hỏng
                    listData.Add(item, Math.Round(totalNumber, 2));
                }

                return await Task.FromResult(PayLoad<object>.Successfully(listData));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetNormalDistrictUpdate2()
        {
            try
            {
                var list = Status.listDataDisrtric;
                if (list.Count <= 0)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var listData = new Dictionary<string, int>();
                var total = _context.trafficequipments.Where(x => !x.deleted).Count();
                foreach (var item in list)
                {
                    var dataCount = _context.repairdetails.Where(x => x.RepairStatus == 3 && !x.deleted).AsNoTracking()
                        .Select(x => new
                        {
                            name1 = x.trafficEquipment.District_1,
                            name2 = x.trafficEquipment.District_2
                        }).Select(x => new
                        {
                            district1 = x.name1,
                            district2 = x.name2
                        }).Where(x2 => x2.district1 == item || x2.district2 == item).Count();

                    //double totalNumber = total == 0 ? 0 : (dataCount * 100.0) / total; // Tính phần trăm đèn bị hỏng trong tổng số đèn
                    //var totalNumber = ((double)(total - dataCount) / total) * 100; // Tính phần số đèn chưa hỏng
                    listData.Add(item, dataCount);
                }

                return await Task.FromResult(PayLoad<object>.Successfully(listData));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GetNormalError()
        {
            try
            {
                int total = _context.trafficequipments.Where(x => !x.deleted).Count();

                // Số đèn bị lỗi(distinct để tránh đếm trùng nếu có nhiều lỗi)
                int errorLights = _context.repairdetails.Where(x => x.RepairStatus != 4 && x.RepairStatus != 5 && !x.deleted).Select(x => x.id).Distinct().Count();

                if (total == 0)
                    return await Task.FromResult(PayLoad<object>.Successfully(0));

                double percentage = (errorLights * 100.0) / total;

                return await Task.FromResult(PayLoad<object>.Successfully(Math.Round(percentage, 2))); // Làm tròn 2 chữ số sau dấu phẩy
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> FindAllNameDistric(string? name, int page = 1, int pageSize = 20)
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
        x.Road_1,
        x.Road_2,
        x.District_1,
        FirstRepair = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus != 3)
            .OrderBy(r => r.id)
            .Select(r => new
            {
                r.RepairStatus,
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        FirstRepairUpdate = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4)
            .OrderByDescending(r => r.FaultReportingTime)
            .Select(r => new
            {
                r.FaultCodes,
                RepairRecord = r.RepairRecords
                    .OrderBy(rr => rr.id)
                    .Select(rr => rr.user.Name)
                    .FirstOrDefault(),
                date = r.FaultReportingTime
            })
            .FirstOrDefault(),
        imageData = x.RepairDetails
        .SelectMany(x => x.RepairRecords).Select(x2 => x2.Picture),
        totalEdit = x.RepairDetails
            .Where(r => r.TE_id == x.id && !r.deleted && r.RepairStatus == 4).Count()
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
        account_user = x.FirstRepair != null ? x.FirstRepair.RepairRecord : null,
        images = x.imageData.ToList(),
        totalUpdate = x.totalEdit,
        date = x.FirstRepair.date,
        dateUpdate = x.FirstRepairUpdate.date,
        account_userUpdate = x.FirstRepairUpdate.RepairRecord,
        statusErrorUpdate = x.FirstRepairUpdate.FaultCodes ?? 0,
        isErrorUpdate = x.FirstRepairUpdate != null,
        road1 = x.Road_1,
        road2 = x.Road_2,
        statusErrorFauCode = x.FirstRepair.FaultCodes,
        districs = x.District_1
    })
    .ToList();
               
                if (!string.IsNullOrEmpty(name))
                {
                    checkData = checkData.Where(x => x.districs.Contains(name)).ToList();
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
    }
}
