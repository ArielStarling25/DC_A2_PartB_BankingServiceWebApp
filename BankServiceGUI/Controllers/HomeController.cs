using BankServiceGUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BankDataWebService.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.AspNetCore.Http;

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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            if (email != null && password != null)
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
                            ViewBag.ProfileName = profile.name;
                            ViewBag.ProfileEmail = profile.email;
                            ViewBag.ProfileType = profile.type;
                        }
                        else
                        {
                            ViewBag.ProfileEmail = null;
                            ViewBag.Error = "Password is incorrect!";
                        }
                    }
                    else
                    {
                        ViewBag.ProfileEmail = null;
                        ViewBag.Error = "Email does not exist!";
                    }
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

        [HttpGet("profile/{email}")]
        //returns a profile by email id
        public async Task<IActionResult> getProfile(string email)
        {
            string decodedEmail = decodeString(email);
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
                    ViewBag.ProfileName = profile.name;;
                    ViewBag.ProfilePhone = profile.phone;
                    ViewBag.ProfileEmail = profile.email;
                    ViewBag.ProfileAddr = profile.address;
                    ViewBag.ProfilePassword = profile.password;
                    ViewBag.ProfilePicture = profile.picture;
                    ViewBag.PictureType = "image/jpeg";
                }
            }
            else
            {
                return NotFound();
            }
            return View("Profile");
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

        [HttpPost("transaction")]
        public async Task<IActionResult> postTrans()
        {
            string email = Request.Form["email"].ToString();
            string decodedEmail = decodeString(email);
            int accountNumber = int.Parse(Request.Form["accountNumber"].ToString());
            int toAccountNumber = int.Parse(Request.Form["toAccountNumber"].ToString());
            double amount = double.Parse(Request.Form["amount"].ToString());
            string description = Request.Form["description"].ToString();
            Transaction transaction = new Transaction();
            transaction.accountNumber = accountNumber;
            transaction.toAccountNumber = toAccountNumber;
            transaction.amount = amount;
            transaction.description = description;

            List<Bank> banks = null;
            Bank bankOwned = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bbank/" + decodedEmail, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                banks = JsonConvert.DeserializeObject<List<Bank>>(response.Content);
                foreach (Bank bank in banks)
                {
                    if (bank.accountNumber == accountNumber)
                    {
                        bankOwned = bank;
                        break;
                    }
                }
                ViewBag.Error = banks.Count().ToString();
                if (bankOwned != null)
                {
                    if (bankOwned.balance >= amount)
                    {
                        if (accountNumber != toAccountNumber)
                        {
                            client = new RestClient(httpURL);
                            req = new RestRequest("/api/btransaction", Method.Post);
                            req.RequestFormat = RestSharp.DataFormat.Json;
                            req.AddBody(transaction);
                            response = await client.PostAsync(req);
                            if (response.IsSuccessStatusCode)
                            {
                                ViewBag.Error = response.Content;
                            }
                            else
                            {
                                ViewBag.Error = response.Content;
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Do not transfer to same account!";
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Account balance not enought!";
                    }
                }
                else
                {
                    ViewBag.Error = "This is not your account!";
                }
            }
            else
            {
                ViewBag.Error = response.Content;
            }
            ViewBag.ProfileEmail = decodedEmail;
            return View("Transaction");
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

        public async Task<IActionResult> Profile(string email)
        {
            string decodedEmail = decodeString(email);
            Profile profile = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + decodedEmail, Method.Get);
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
                    ViewBag.ProfileName = profile.name; ;
                    ViewBag.ProfilePhone = profile.phone;
                    ViewBag.ProfileEmail = profile.email;
                    ViewBag.ProfileAddr = profile.address;
                    ViewBag.ProfilePassword = profile.password;
                    ViewBag.ProfilePicture = string.Format("data:image/png;base64,{0}", profile.picture);
                }
            }
            else
            {
                return NotFound();
            }
            return View();
        }

        public async Task <IActionResult> AccountSummary(string email)
        {
            string decodedEmail = decodeString(email);
            List<Bank> banks = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bbank/" + decodedEmail, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                banks = JsonConvert.DeserializeObject<List<Bank>>(response.Content);
                ViewBag.Banks = banks;
                ViewBag.ProfileEmail = decodedEmail;
            }
            else
            {
                return NotFound();
            }
            return View();
        }

        public async Task<IActionResult> TransactionHistory(string email, int accountNumber)
        {
            string decodedEmail = decodeString(email);
            List<Transaction> transactions = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction/" + accountNumber, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                ViewBag.Transactions = transactions;
                ViewBag.ProfileEmail = decodedEmail;
            }
            else
            {
                return NotFound();
            }
            return View();
        }

        public async Task<IActionResult> Transaction(string email)
        {
            ViewBag.ProfileEmail = decodeString(email);
            return View();
        }

        private string decodeString(string encodedString) 
        {
            byte[] data = Convert.FromBase64String(encodedString);
            string decodedString = System.Text.Encoding.UTF8.GetString(data);
            return decodedString;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}