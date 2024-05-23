namespace UNP.Models
{
    public class UnpEntryResponseModel
    {
        public string Unp { get; set; }
        public string LastChecked { get; set; }
        public string IsInLocalDb { get; set; }
        public string IsInExternalDb { get; set; }
    }
}
