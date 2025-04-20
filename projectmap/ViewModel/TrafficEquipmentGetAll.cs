namespace projectmap.ViewModel
{
    public class TrafficEquipmentGetAll
    {
        public int id { get; set; }
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
        public bool? isErrorUpdate { get; set; }
        public int? statusError { get; set; }
        public int? statusErrorUpdate { get; set; }
        public int? statusErrorFauCode { get; set; }
        public int? totalUpdate { get; set; }
        public string? account_user { get; set; }
        public string? account_userUpdate { get; set; }
        public string? road1 { get; set; }
        public string? road2 { get; set; }
        public DateTimeOffset? date { get; set; }
        public DateTimeOffset? dateUpdate { get; set; }
        public List<string>? images { get; set; }
    }
}
