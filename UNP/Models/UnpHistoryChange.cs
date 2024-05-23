namespace UNP.Models
{
    public class UnpHistoryChange
    {
        public int Id { get; set; }
        public string Unp { get; set; }
        public string Email { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangeType { get; set; }
    }
}