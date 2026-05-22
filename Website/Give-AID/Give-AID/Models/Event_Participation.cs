using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Give_AID.Models
{
    public class Event_Participation
    {
        public int Id { get; set; }

        public int Volunteer_ID { get; set; }

        [ForeignKey("Volunteer_ID")]
        public Volunteer Volunteer { get; set; }
        public int Event_ID { get; set; }

        [ForeignKey("Event_ID")]
        public Event Event { get; set; }

        [DefaultValue("Approvaal Pending")]
        public string Status { get; set; }


    }
}
