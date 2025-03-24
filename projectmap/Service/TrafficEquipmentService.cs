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
                var checkData = _context.trafficEquipments.FirstOrDefault(x => x.Longitude == data.Longitude && x.Latitude == data.Latitude);
                if(checkData != null)
                    return await Task.FromResult(PayLoad<TrafficEquipmentDTO>.CreatedFail(Status.DATATONTAI));

                var mapData = _mapper.Map<TrafficEquipment>(data);
                //mapData.CategoryCode = RanDomCode.geneActionInteGer(7);
                //mapData.IdentificationCode = Convert.ToDouble(RanDomCode.geneActionInteGer(15));
                _context.trafficEquipments.Add(mapData);
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

                _context.trafficEquipments.Add(equipment);
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
                var checkData = _context.trafficEquipments
                     .Include(x => x.RepairDetails)
                     .Select(x => new
                     {
                         Equipment = x,
                         FirstRepair = x.RepairDetails.FirstOrDefault(x1 => x1.TE_id == x.id)
                     })
                     .Select(x => new TrafficEquipmentGetAll
                     {
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
                         Latitude = x.Equipment.Latitude
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
    }
}
