using Give_AID.Models;
using Microsoft.EntityFrameworkCore;

namespace Give_AID
{
    public class Bridge : DbContext
    {
        public Bridge(DbContextOptions<Bridge> options)
            : base(options)
        {
        }
        public DbSet<Admin> admin {  get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Campaign> campaign { get; set; }
        public DbSet<Category> category { get; set; }
        public DbSet<Donation> donation { get; set; }
        public DbSet<Event> events { get; set; }
        public DbSet<Event_Participation> event_participation { get; set; }
        public DbSet<Feedback> feedback { get; set; }
        public DbSet<Purpose> purpose { get; set; }
        public DbSet<Volunteer> volunteer { get; set; }
        public DbSet<CampaignDonation> campaignDonation { get; set; }
    }
}
