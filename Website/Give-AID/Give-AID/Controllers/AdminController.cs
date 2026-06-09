using Give_AID.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Give_AID.Controllers
{
    public class AdminController : Controller
    {
        private readonly Bridge db;

        public AdminController(Bridge _db)
        {
            db = _db;
        }

        public override void OnActionExecuting(
          Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"]?.ToString();

            if (action != "Login" &&
                action != "Logout" &&
                HttpContext.Session.GetString("AdminEmail") == null)
            {
                context.Result = RedirectToAction("Login");
            }

            base.OnActionExecuting(context);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = db.admin.FirstOrDefault(x =>
                x.Admin_Email == email &&
                x.Admin_Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("AdminEmail", admin.Admin_Email);
                HttpContext.Session.SetString("AdminName", admin.Admin_Name);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalUsers = db.User.Count();
            ViewBag.TotalDonations = db.donation.Count();
            ViewBag.TotalCampaigns = db.campaign.Count();
            ViewBag.TotalVolunteers = db.volunteer.Count();

            ViewBag.TotalCampaignDonations =
                db.campaignDonation.Count();

            ViewBag.TotalEvents =
                db.events.Count();

            ViewBag.GeneralDonationAmount =
                db.donation.Sum(x => (int?)x.Amount) ?? 0;

            ViewBag.CampaignDonationAmount =
                db.campaignDonation.Sum(x => (int?)x.Amount) ?? 0;

            ViewBag.TotalMoneyRaised =
                ViewBag.GeneralDonationAmount +
                ViewBag.CampaignDonationAmount;

            ViewBag.CampaignList = db.campaign.ToList();
            ViewBag.CampaignDonations = db.campaignDonation.ToList();

            return View();
        }
        public IActionResult Logout()
        {
         HttpContext.Session.Clear();
         return RedirectToAction("Login");
        }
        public IActionResult Campaigns()
        {
            var data = db.campaign.ToList();
            return View(data);
        }
        [HttpGet]
        public IActionResult CreateCampaign()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCampaign(Campaign c)
        {
            db.campaign.Add(c);
            db.SaveChanges();

            return RedirectToAction("Campaigns");
        }
        [HttpGet]
        public IActionResult EditCampaign(int id)
        {
            var data = db.campaign.Find(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult EditCampaign(Campaign c)
        {
            db.campaign.Update(c);
            db.SaveChanges();

            return RedirectToAction("Campaigns");
        }
        public IActionResult DeleteCampaign(int id)
        {
            var data = db.campaign.Find(id);

            if (data != null)
            {
                db.campaign.Remove(data);
                db.SaveChanges();
            }

            return RedirectToAction("Campaigns");
        }
        public IActionResult Purposes()
        {
            var data = db.purpose.ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult CreatePurpose()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePurpose(Purpose p)
        {
            db.purpose.Add(p);
            db.SaveChanges();

            return RedirectToAction("Purposes");
        }
        [HttpGet]
        public IActionResult EditPurpose(int id)
        {
            var data = db.purpose.Find(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult EditPurpose(Purpose p)
        {
            db.purpose.Update(p);
            db.SaveChanges();

            return RedirectToAction("Purposes");
        }

        public IActionResult DeletePurpose(int id)
        {
            var data = db.purpose.Find(id);

            if (data != null)
            {
                db.purpose.Remove(data);
                db.SaveChanges();
            }

            return RedirectToAction("Purposes");
        }
        public IActionResult Users()
        {
            var data = db.User.ToList();
            return View(data);
        }
        public IActionResult Volunteers()
        {
            var data = db.volunteer.ToList();
            return View(data);
        }
        public IActionResult ApproveVolunteer(int id)
        {
            var volunteer = db.volunteer.Find(id);

            if (volunteer != null)
            {
                volunteer.Status = "Approved";
                db.SaveChanges();
            }

            return RedirectToAction("Volunteers");
        }

        public IActionResult RejectVolunteer(int id)
        {
            var volunteer = db.volunteer.Find(id);

            if (volunteer != null)
            {
                volunteer.Status = "Rejected";
                db.SaveChanges();
            }

            return RedirectToAction("Volunteers");
        }
        public IActionResult Donations()
        {
            var data = db.donation
                         .Include(x => x.User)
                         .Include(x => x.Purpose)
                         .ToList();

            return View(data);
        }
        public IActionResult Events()
        {
            var data = db.events.ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateEvent(Event e)
        {
            db.events.Add(e);
            db.SaveChanges();

            return RedirectToAction("Events");
        }
        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            var data = db.events.Find(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult EditEvent(Event e)
        {
            db.events.Update(e);
            db.SaveChanges();

            return RedirectToAction("Events");
        }

        public IActionResult DeleteEvent(int id)
        {
            var data = db.events.Find(id);

            if (data != null)
            {
                db.events.Remove(data);
                db.SaveChanges();
            }

            return RedirectToAction("Events");
        }
        public IActionResult EventParticipants()
        {
            var data = db.event_participation
                         .Include(x => x.Volunteer)
                         .Include(x => x.Event)
                         .ToList();

            return View(data);
        }
        public IActionResult ApproveEventParticipant(int id)
        {
            var data = db.event_participation.Find(id);

            if (data != null)
            {
                data.Status = "Approved";
                db.SaveChanges();
            }

            return RedirectToAction("EventParticipants");
        }

        public IActionResult RejectEventParticipant(int id)
        {
            var data = db.event_participation.Find(id);

            if (data != null)
            {
                data.Status = "Rejected";
                db.SaveChanges();
            }

            return RedirectToAction("EventParticipants");
        }
        public IActionResult CampaignDonations()
        {
            var data = db.campaignDonation
                         .Include(x => x.User)
                         .Include(x => x.Campaign)
                         .OrderByDescending(x => x.Id)
                         .ToList();

            return View(data);
        }
        public IActionResult Feedbacks()
        {
            var data = db.feedback
            .Include(x => x.User)
            .OrderByDescending(x => x.Id)
            .ToList();


        return View(data);
}

    }
}