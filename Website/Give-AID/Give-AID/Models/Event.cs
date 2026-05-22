namespace Give_AID.Models
{
    public class Event
    {
       public int Id { get; set; }
        public string Event_Name { get; set; }
        public DateTime Event_Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; } 
    }
}
