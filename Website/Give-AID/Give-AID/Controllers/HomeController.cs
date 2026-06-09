using Give_AID.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Give_AID.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Bridge db;

        public HomeController(ILogger<HomeController> logger, Bridge _db)
        {
            _logger = logger;
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User u)
        {
            db.User.Add(u);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = db.User.FirstOrDefault(x =>
                x.Email == email &&
                x.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction("UserDashboard");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }
        public IActionResult UserDashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.TotalDonations =
                db.donation.Count(x => x.User_ID == userId);

            var volunteer = db.volunteer
                              .FirstOrDefault(x => x.User_ID == userId);

            ViewBag.VolunteerStatus =
                volunteer != null ? volunteer.Status : "Not Applied";

            ViewBag.ActiveCampaigns =
                db.campaign.Count();

            if (volunteer != null)
            {
                var eventStatus = db.event_participation
                                    .Where(x => x.Volunteer_ID == volunteer.Id)
                                    .OrderByDescending(x => x.Id)
                                    .Select(x => x.Status)
                                    .FirstOrDefault();

                ViewBag.EventStatus = eventStatus ?? "Not Joined";
            }
            else
            {
                ViewBag.EventStatus = "Not Joined";
            }

            return View();
        }
        [HttpGet]
        public IActionResult Volunteer()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Volunteer(Volunteer v)
        {
            v.User_ID = HttpContext.Session.GetInt32("UserId").Value;
            v.Status = "Pending";

            db.volunteer.Add(v);
            db.SaveChanges();

            return RedirectToAction("UserDashboard");
        }
        [HttpGet]
        public IActionResult Donate()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Purposes = db.purpose.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Donate(Donation d)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            d.User_ID = HttpContext.Session.GetInt32("UserId").Value;
            d.Donation_Date = DateTime.Now;

            db.donation.Add(d);
            db.SaveChanges();

            return RedirectToAction("UserDashboard");
        }
        public IActionResult DonationHistory()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var data = db.donation
                         .Include(x => x.Purpose)
                         .Where(x => x.User_ID == userId)
                         .ToList();

            return View(data);
        }
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = db.User.Find(userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User u)
        {
            db.User.Update(u);
            db.SaveChanges();

            return RedirectToAction("UserDashboard");
        }
        public IActionResult Events()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var data = db.events.ToList();

            var userId = HttpContext.Session.GetInt32("UserId");

            var volunteer = db.volunteer
                              .FirstOrDefault(x => x.User_ID == userId);

            if (volunteer != null)
            {
                ViewBag.JoinedEvents = db.event_participation
                                         .Where(x => x.Volunteer_ID == volunteer.Id)
                                         .Select(x => x.Event_ID)
                                         .ToList();
            }

            return View(data);
        }
        public IActionResult JoinEvent(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var volunteer = db.volunteer
                              .FirstOrDefault(x => x.User_ID == userId);

            if (volunteer == null)
            {
                TempData["Error"] = "Please apply as volunteer first.";
                return RedirectToAction("Volunteer");
            }

            var alreadyJoined = db.event_participation
                                  .FirstOrDefault(x =>
                                      x.Volunteer_ID == volunteer.Id &&
                                      x.Event_ID == id);

            if (alreadyJoined != null)
            {
                TempData["Error"] = "You have already joined this event.";
                return RedirectToAction("Events");
            }

            Event_Participation ep = new Event_Participation();

            ep.Volunteer_ID = volunteer.Id;
            ep.Event_ID = id;
            ep.Status = "Approval Pending";

            db.event_participation.Add(ep);
            db.SaveChanges();

            TempData["Success"] = "Event participation request submitted successfully.";

            return RedirectToAction("Events");
        }
        public IActionResult Campaigns()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var data = db.campaign.ToList();

            foreach (var item in data)
            {
                ViewData[$"Collected_{item.Id}"] =
                    db.campaignDonation
                      .Where(x => x.Campaign_ID == item.Id)
                      .Sum(x => (int?)x.Amount) ?? 0;
            }

            return View(data);
        }
        public IActionResult MyEvents()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var volunteer = db.volunteer
                              .FirstOrDefault(x => x.User_ID == userId);

            if (volunteer == null)
            {
                return RedirectToAction("Volunteer");
            }

            var data = db.event_participation
                         .Include(x => x.Event)
                         .Where(x => x.Volunteer_ID == volunteer.Id)
                         .ToList();

            return View(data);
        }
        [HttpGet]
        public IActionResult CampaignDonate(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var user = db.User.Find(userId);

            var campaign = db.campaign.Find(id);

            ViewBag.User = user;
            ViewBag.Campaign = campaign;

            return View();
        }

        [HttpPost]
        public IActionResult CampaignDonate(int campaignId, int amount, string paymentMode)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            CampaignDonation d = new CampaignDonation();

            d.User_ID = userId.Value;
            d.Campaign_ID = campaignId;
            d.Amount = amount;
            d.Payment_Mode = paymentMode;
            d.Donation_Date = DateTime.Now;

            db.campaignDonation.Add(d);
            db.SaveChanges();

            TempData["Success"] = "Campaign donation submitted successfully.";

            return RedirectToAction("CampaignDonationHistory");
        }
        public IActionResult CampaignDonationHistory()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            var userId = HttpContext.Session.GetInt32("UserId");

            var data = db.campaignDonation
                         .Include(x => x.Campaign)
                         .Where(x => x.User_ID == userId)
                         .ToList();

            return View(data);
        }
        [HttpGet]
        public IActionResult Feedback()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login");
            }


            return View();


}

        [HttpPost]
        public IActionResult Feedback(string message)
        {
            var userId = HttpContext.Session.GetInt32("UserId");


          Feedback f = new Feedback();

            f.User_ID = userId.Value;
            f.Message = message;
            f.Feedback_Date = DateTime.Now;

            db.feedback.Add(f);
            db.SaveChanges();

            TempData["Success"] = "Feedback submitted successfully.";

            return RedirectToAction("UserDashboard");


}

    }
}