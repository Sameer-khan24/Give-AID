using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Give_AID.Models
{
    public class Volunteer
    {
        public int Id { get; set; }
        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }
        public string Skills { get; set; }

        public DateOnly AvailableFrom{ get; set; }
        public DateOnly AvailableTill { get; set; }
        [DefaultValue("Pending")]

        public string Status { get; set; }

    }
}
