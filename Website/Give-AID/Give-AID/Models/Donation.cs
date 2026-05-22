using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Give_AID.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public int User_ID { get; set; }
        
        [ForeignKey ("User_ID")]
        public User User { get; set; }
        
        public int Amount { get; set; }
        public DateTime Donation_Date { get; set; }
        public string Payment_Mode { get; set; }
        public int Purpose_ID { get; set; }
        [ForeignKey ("Purpose_ID")]
        public Purpose Purpose { get; set; }


    }
}
