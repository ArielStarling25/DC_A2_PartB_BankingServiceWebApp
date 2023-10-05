using BankServiceGUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BankDataWebService.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestSharp;

namespace BankServiceGUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string httpURL = "http://localhost:5103";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index(int sessionId)
        {
            if(sessionId == 12345)
            {
                ViewBag.Message = "LoggedIn";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            List<Profile> profiles = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile", Method.Get);
            RestResponse response = await client.GetAsync(req);
            profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
            if (profiles == null)
            {
                ViewBag.Error = "Error in obtaining profile information";
            }
            else
            {
                Profile profile = new Profile();
                if(profileExists(email, profiles, out profile))
                {
                    if (password.Equals(profile.password))
                    {
                        ViewBag.Message = "LoggedIn";
                        ViewBag.ProfileName = profile.name;
                        ViewBag.ProfileEmail = profile.email;
                        ViewBag.ProfileAddr = profile.address;
                        ViewBag.ProfilePhone = profile.phone;
                        ViewBag.ProfilePassword = profile.password;
                        ViewBag.ProfilePictureData = profile.picture;
                    }
                    else
                    {
                        ViewBag.Error = "Password is incorrect!";
                    }
                }
                else
                {
                    ViewBag.Error = "Email does not exist!";
                }
            }
            return View();
        }

        private bool profileExists(string email, List<Profile> profiles, out Profile profile)
        {
            bool found = false;
            profile = null;
            foreach(Profile prof in profiles)
            {
                if (email.Equals(prof.email))
                {
                    found = true;
                    profile = prof;
                    break;
                }
            }
            return found;
        }

        [HttpGet]
        public IActionResult Profile(int sessionId)
        {
            if(sessionId == 12345)
            {
                ViewBag.Message = "LoggedIn";
            }
            return View();
        }

        [HttpGet]
        public IActionResult Transactions(int sessionId)
        {
            if(sessionId == 12345)
            {
                ViewBag.Message = "LoggedIn";
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}