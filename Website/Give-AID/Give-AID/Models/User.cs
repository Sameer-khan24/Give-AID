using Microsoft.AspNetCore.Identity;

namespace Give_AID.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Contact_No { get; set; }
        public string address { get; set; }
        
    }
}
