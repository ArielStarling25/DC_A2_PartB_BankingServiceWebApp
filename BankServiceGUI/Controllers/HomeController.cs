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
            if (sessionId == 12345)
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
                if (profileExists(email, profiles, out profile))
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

        //Profile Controllers

        //Returns all profiles
        [HttpGet("profile")]
        public async Task<IActionResult> getProfiles()
        {
            List<Profile> profiles = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
                if (profiles == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(profiles));
                }
            }
            else
            {
                return NotFound();
            }
        }

        //returns a profile by email id
        [HttpGet("profile/{email}")]
        public async Task<IActionResult> getProfileByEmail(string email)
        {
            Profile profile = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + email, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                profile = JsonConvert.DeserializeObject<Profile>(response.Content);
                if (profile == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(profile));
                }
            }
            else
            {
                return NotFound();
            }
        }

        //Updating User Data
        [HttpPut("profile/{email}")]
        public async Task<IActionResult> putProfileData(string email, [FromBody] Profile data)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + email, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(data);
            RestResponse response = await client.PutAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        //Bank Controllers

        //Returns all bank data
        [HttpGet("bank")]
        public async Task<IActionResult> getBanks()
        {
            List<Bank> banks = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bbanks", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                banks = JsonConvert.DeserializeObject<List<Bank>>(response.Content);
                if (banks == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(banks));
                }
            }
            else
            {
                return NotFound();
            }
        }

        //Gets Bank Data by id
        [HttpGet("bank/{id}")]
        public async Task<IActionResult> getBankById(int id)
        {
            Bank bank = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bbanks/" + id, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                bank = JsonConvert.DeserializeObject<Bank>(response.Content);
                if (bank == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(bank));
                }
            }
            else
            {
                return NotFound();
            }
        }

        //Updating Bank Data
        [HttpPut("bank/{id}")]
        public async Task<IActionResult> putBankData(int id, [FromBody] Bank bankData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bbanks/" + id, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(bankData);
            RestResponse response = await client.PutAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        //Transaction Controllers
        
        //Returns all transaction data
        [HttpGet("transaction")]
        public async Task<IActionResult> getTrans()
        {
            List<Transaction> trans = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                trans = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                if (trans == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(trans));
                }
            }
            else
            {
                return NotFound();
            }
        }

        //returns id specific transaction
        [HttpGet("transaction/id/{id}")]
        public async Task<IActionResult> getTransById(int id)
        {
            Transaction trans = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction/id/" + id, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                trans = JsonConvert.DeserializeObject<Transaction>(response.Content);
                if (trans == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(trans));
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("transaction/accno/{accNo}")]
        public async Task<IActionResult> getTransByAccNo(int accNo)
        {
            Transaction trans = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction/accno/" + accNo, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                trans = JsonConvert.DeserializeObject<Transaction>(response.Content);
                if (trans == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(JsonConvert.SerializeObject(trans));
                }
            }
            else
            {
                return NotFound();
            }
        }

        //updates a specific transaction by id
        [HttpPut("transaction/{id}")]
        public async Task<IActionResult> putTrans(int id, [FromBody] Transaction transData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction/" + id, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(transData);
            RestResponse response = await client.PutAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
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