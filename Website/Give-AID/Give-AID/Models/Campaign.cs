namespace Give_AID.Models
{
    public class Campaign
    {
        public int Id { get; set; }
        public string Campaign_Name { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public int Goal_Amount { get; set; }
        public string Description { get; set; }
    }
}
