using System.ComponentModel.DataAnnotations.Schema;

namespace Give_AID.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        public string Message { get; set; }
        public DateTime Feedback_Date { get; set; }

    }
}
