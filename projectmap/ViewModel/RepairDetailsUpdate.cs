namespace projectmap.ViewModel
{
    public class RepairDetailsUpdate
    {
        public int id { get; set; }
    }

    public class ConfirmData
    {
        public int id { get; set; }
        public int id_user { get; set; }
    }

    public class RepairDetailsUpdateByAccont
    {
        public int id { get; set; }
        public int status { get; set; }
        public int FaultCodes { get; set; }
    }
}
