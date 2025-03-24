namespace projectmap.ViewModel
{
    public class TrafficEquipmentGetAll
    {
        public int? CategoryCode { get; set; }
        public double? IdentificationCode { get; set; }
        public string? ManagementUnit { get; set; }
        public int? JobClassification { get; set; }
        public string? SignalNumber { get; set; }
        public string? TypesOfSignal { get; set; }
        public int? SignalInstallation { get; set; }
        public int? UseStatus { get; set; }
        public int? DataStatus { get; set; }
        public string? Remark { get; set; }
        public double? Length { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public bool? isError { get; set; }
        public int? statusError { get; set; }
    }
}
