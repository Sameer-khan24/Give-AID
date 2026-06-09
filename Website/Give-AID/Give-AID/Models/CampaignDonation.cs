using System.ComponentModel.DataAnnotations.Schema;

namespace Give_AID.Models
{
    public class CampaignDonation
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }

        public int Campaign_ID { get; set; }

        [ForeignKey("Campaign_ID")]
        public Campaign Campaign { get; set; }

        public int Amount { get; set; }

        public string Payment_Mode { get; set; }

        public DateTime Donation_Date { get; set; }
    }
}