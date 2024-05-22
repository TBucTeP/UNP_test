namespace UNP.Models
{
    public class UnpModel
    {
        public int Id { get; set; }
        public string Unp { get; set; }
        public string Email { get; set; }
        public bool IsInLocalDb { get; set; }
        public bool IsInExternalDb { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
