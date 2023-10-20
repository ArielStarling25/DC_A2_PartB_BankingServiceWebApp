using BankServiceGUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BankDataWebService.Models;
using BankServiceGUI.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.AspNetCore.Http;
using Azure;

namespace BankServiceGUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string httpURL = "http://localhost:5103";
        private string savedProfileType = "";

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
                            savedProfileType = profile.type;
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
                    ViewBag.ProfileName = profile.name; ;
                    ViewBag.ProfilePhone = profile.phone;
                    ViewBag.ProfileEmail = profile.email;
                    ViewBag.ProfileAddr = profile.address;
                    ViewBag.ProfilePassword = profile.password;
                    ViewBag.ProfilePicture = profile.picture;
                    ViewBag.ProfileType = profile.type;
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

        [HttpPost("updateprofile/{email}")]
        public async Task<IActionResult> updateProfile(string email)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + email, Method.Get);
            RestResponse response = await client.GetAsync(req);
            Profile oldProfile = JsonConvert.DeserializeObject<Profile>(response.Content);
            string picture = oldProfile.picture;
            string type = oldProfile.type;
            string oldName = oldProfile.name;
            string oldEmail = oldProfile.email;
            string oldAddress = oldProfile.address;
            string oldPhone = oldProfile.phone;
            string oldPassword = oldProfile.password;

            string newName = Request.Form["name"].ToString();
            string newEmail = Request.Form["email"].ToString();
            string newAddress = Request.Form["address"].ToString();
            string newPhone = Request.Form["phone"].ToString();
            string newPassword = Request.Form["password"].ToString();
            Profile newProfile = new Profile();
            newProfile.picture = picture;
            newProfile.type = type;
            newProfile.name = newName;
            newProfile.email = newEmail;
            newProfile.address = newAddress;
            newProfile.phone = newPhone;
            newProfile.password = newPassword;

            client = new RestClient(httpURL);
            req = new RestRequest("/api/bprofile/" + email, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(newProfile);
            response = await client.PutAsync(req);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Alert = "Profile successfully updated!";
                ViewBag.ProfileName = newName;
                ViewBag.ProfileEmail = newEmail;
                ViewBag.ProfileAddr = newAddress;
                ViewBag.ProfilePhone = newPhone;
                ViewBag.ProfilePassword = newPassword;
                ViewBag.ProfilePicture = string.Format("data:image/png;base64,{0}", picture);
                ViewBag.ProfileType = type;
            }
            else
            {
                ViewBag.Alert = "Profile failed to update!";
                ViewBag.ProfileName = oldName;
                ViewBag.ProfileEmail = oldEmail;
                ViewBag.ProfileAddr = oldAddress;
                ViewBag.ProfilePhone = oldPhone;
                ViewBag.ProfilePassword = oldPassword;
                ViewBag.ProfilePicture = string.Format("data:image/png;base64,{0}", picture);
                ViewBag.ProfileType = type;
            }
            return View("Profile");
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
            ViewBag.ProfileType = "user";
            return View("Transaction");
        }

        private bool profileExists(string email, List<Profile> profiles, out Profile profile)
        {
            bool found = false;
            profile = null;
            foreach (Profile prof in profiles)
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
                    ViewBag.ProfileType = profile.type;
                    if (!String.IsNullOrEmpty(profile.picture))
                    {
                        ViewBag.ProfilePicture = string.Format("data:image/png;base64,{0}", profile.picture);
                    }
                    else
                    {
                        ViewBag.ProfilePicture = "";
                    }
                }
            }
            else
            {
                return NotFound();
            }
            return View();
        }

        public async Task<IActionResult> AccountSummary(string email)
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
                ViewBag.ProfileType = "user";
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
                ViewBag.ProfileType = "user";
            }
            else
            {
                return NotFound();
            }
            return View();
        }

        public async Task<IActionResult> UserManage(string email)
        {
            string decodedEmail = decodeString(email);
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + decodedEmail, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                Profile adminProf = JsonConvert.DeserializeObject<Profile>(response.Content);
                if (adminProf == null)
                {
                    LogClass.LogItem(decodedEmail, "Error", "Admin account does not exist!");
                    return NotFound();
                }
                else
                {
                    List<Profile> profiles = null;
                    RestClient client2 = new RestClient(httpURL);
                    RestRequest req2 = new RestRequest("/api/bprofile/", Method.Get);
                    RestResponse response2 = await client2.GetAsync(req2);
                    if (response2.IsSuccessStatusCode)
                    {
                        if (adminProf.type.Equals("admin"))
                        {
                            profiles = JsonConvert.DeserializeObject<List<Profile>>(response2.Content);
                            ViewBag.ProfileList = profiles;
                            ViewBag.ProfileEmail = decodedEmail;
                            ViewBag.ProfileType = adminProf.type;
                            LogClass.LogItem(decodedEmail, "Info", "Account list access");
                        }
                        else
                        {
                            ViewBag.ProfileEmail = null;
                            LogClass.LogItem(decodedEmail, "Error", "Invalid account access");
                        }
                    }
                    else
                    {
                        LogClass.LogItem(decodedEmail, "Error", "Failed to retrive list: " + response2);
                        return NotFound();
                    }
                }
            }
            else
            {
                LogClass.LogItem(decodedEmail, "Error", "Failed to retrieve admin: " + response);
                return NotFound();
            }
            return View();
        }

        public async Task<IActionResult> UserMenu(string emailAd, string emailUser)
        {
            if (emailUser == null)
            {
                LogClass.LogItem(decodeString(emailAd), "Error", "emailUser is null");
                return BadRequest();
            }
            else
            {
                RestClient client = new RestClient(httpURL);
                RestRequest req = new RestRequest("/api/bprofile/" + decodeString(emailAd), Method.Get);
                RestResponse response = await client.GetAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    Profile adminProf = JsonConvert.DeserializeObject<Profile>(response.Content);
                    if (adminProf == null)
                    {
                        LogClass.LogItem(decodeString(emailAd), "Error", "Admin account does not exist!");
                        return NotFound();
                    }
                    else
                    {
                        if (adminProf.type.Equals("admin"))
                        {
                            if (decodeString(emailUser).Equals("new")) //When creating a new profile
                            {
                                ViewBag.EditState = true;
                                Profile newProf = new Profile();
                                newProf.email = "new";
                                ViewBag.ProfileUser = newProf;
                                ViewBag.ProfileEmail = adminProf.email;
                                ViewBag.ProfileType = adminProf.type;
                            }
                            else //when editing/deleting
                            {
                                string decodedEmail = decodeString(emailUser);
                                Profile profile = null;
                                RestClient client2 = new RestClient(httpURL);
                                RestRequest req2 = new RestRequest("/api/bprofile/" + decodedEmail, Method.Get);
                                RestResponse response2 = await client2.GetAsync(req2);
                                if (response2.IsSuccessStatusCode)
                                {
                                    profile = JsonConvert.DeserializeObject<Profile>(response2.Content);
                                    if (profile != null)
                                    {
                                        ViewBag.EditState = true;
                                        ViewBag.ProfileUser = profile;
                                        ViewBag.ProfileEmail = adminProf.email;
                                        ViewBag.ProfileType = adminProf.type;
                                        LogClass.LogItem(decodeString(emailAd), "Info", "Editing: " + profile.email);
                                    }
                                    else
                                    {
                                        LogClass.LogItem(decodeString(emailAd), "Error", "profile does not exist!");
                                        return NotFound();
                                    }
                                }
                                else
                                {
                                    LogClass.LogItem(decodeString(emailAd), "Error", "Failed to retrieve user: " + response2);
                                    return NotFound();
                                }
                            }
                        }
                        else
                        {
                            LogClass.LogItem(decodeString(emailAd), "Error", "Invalid Access");
                            ViewBag.ProfileEmail = null;
                        }
                    }
                }
                else
                {
                    LogClass.LogItem(decodeString(emailAd), "Error", "Failed to retrieve admin: " + response);
                    return BadRequest();
                }
            }
            return View();
        }

        [HttpPost("updateuser")]
        public async Task<IActionResult> UserMenu(string adminemail, string useremail, string username, string useraddr, string userphone, string userpass, string usertype)
        {
            ViewBag.UserMenuMsg = "";
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
                if (profiles == null)
                {
                    LogClass.LogItem(adminemail, "Error", "profiles is null!");
                    return NotFound();
                }
                else
                {
                    if (profileExists(useremail, profiles, out Profile userProf))
                    {
                        if (verifyAdmin(adminemail))
                        {
                            userProf.name = username;
                            userProf.address = useraddr;
                            userProf.phone = userphone;
                            userProf.password = userpass;
                            userProf.type = usertype;

                            RestClient client2 = new RestClient(httpURL);
                            RestRequest req2 = new RestRequest("/api/bprofile/" + userProf.email, Method.Put);
                            req2.RequestFormat = RestSharp.DataFormat.Json;
                            req2.AddBody(userProf);
                            RestResponse response2 = await client2.PutAsync(req2);
                            if (response2.IsSuccessStatusCode)
                            {
                                ViewBag.ProfileEmail = adminemail;
                                ViewBag.EditState = true;
                                ViewBag.ProfileType = "admin";
                                ViewBag.UserMenuMsg = "Successfully modified profile: " + userProf.email;
                                LogClass.LogItem(adminemail, "Info", "Modded Profile: " + userProf.email);
                            }
                            else
                            {
                                LogClass.LogItem(adminemail, "Error", "Failed PUT request for user");
                                return NotFound();
                            }
                        }
                        else
                        {
                            LogClass.LogItem(adminemail, "Warning", "Unauthorised access");
                            return BadRequest();
                        }
                    }
                    else
                    {
                        LogClass.LogItem(adminemail, "Error", "Profile does not exist!");
                        return BadRequest();
                    }
                }
            }
            else
            {
                LogClass.LogItem(adminemail, "Error", "Failed to retrieve users: " + response);
                return NotFound();
            }
            return View();
        }

        [HttpPost("postuser")]
        public async Task<IActionResult> UserMenu(string adminemail, string useremail, string username, string useraddr, string userphone, string userpass, string usertype, string confirm)
        {
            ViewBag.UserMenuMsg = "";
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
                if (profiles == null)
                {
                    LogClass.LogItem(adminemail, "Error", "profiles is null!");
                    return NotFound();
                }
                else
                {
                    if (profileExists(useremail, profiles, out Profile userProf))
                    {
                        LogClass.LogItem(adminemail, "Error", "Profile exists!");
                        return BadRequest();
                    }
                    else
                    {
                        if (verifyAdmin(adminemail))
                        {
                            if (confirm.Equals(adminemail))
                            {
                                userProf = new Profile();
                                userProf.email = useremail;
                                userProf.name = username;
                                userProf.address = useraddr;
                                userProf.phone = userphone;
                                userProf.picture = "";
                                userProf.password = userpass;
                                userProf.type = usertype;

                                RestClient client2 = new RestClient(httpURL);
                                RestRequest req2 = new RestRequest("/api/bprofile", Method.Post);
                                req2.RequestFormat = RestSharp.DataFormat.Json;
                                req2.AddBody(userProf);
                                RestResponse response2 = await client2.PostAsync(req2);
                                if (response2.IsSuccessStatusCode)
                                {
                                    ViewBag.ProfileEmail = adminemail;
                                    ViewBag.EditState = true;
                                    ViewBag.ProfileType = "admin";
                                    ViewBag.UserMenuMsg = "Successfully added profile: " + userProf.email;
                                    LogClass.LogItem(adminemail, "Info", "Added Profile: " + userProf.email);
                                }
                                else
                                {
                                    LogClass.LogItem(adminemail, "Error", "Failed POST request for user");
                                    return NotFound();
                                }
                            }
                            else
                            {
                                ViewBag.ProfileEmail = adminemail;
                                ViewBag.EditState = true;
                                ViewBag.ProfileType = "admin";
                                ViewBag.UserMenuMsg = "Cancelled adding profile: " + userProf.email;
                                LogClass.LogItem(adminemail, "Info", "Cancelled adding new profile: " + userProf.email);
                            }
                        }
                        else
                        {
                            LogClass.LogItem(adminemail, "Warning", "Unauthorised access");
                            return BadRequest();
                        }
                    }
                }
            }
            else
            {
                LogClass.LogItem(adminemail, "Error", "Failed to retrieve users: " + response);
                return NotFound();
            }
            return View();
        }

        [HttpPost("deleteuser")]
        public async Task<IActionResult> UserMenu(string adminemail, string useremail, string deleteconfirm)
        {
            ViewBag.UserMenuMsg = "";
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
                if (profiles == null)
                {
                    LogClass.LogItem(adminemail, "Error", "profiles is null!");
                    return NotFound();
                }
                else
                {
                    if (profileExists(useremail, profiles, out Profile userProf))
                    {
                        if (verifyAdmin(adminemail))
                        {
                            if (deleteconfirm.Equals(useremail))
                            {
                                RestClient client2 = new RestClient(httpURL);
                                RestRequest req2 = new RestRequest("/api/bprofile/" + userProf.email, Method.Delete);
                                RestResponse response2 = await client2.DeleteAsync(req2);
                                if (response2.IsSuccessStatusCode)
                                {
                                    ViewBag.ProfileEmail = adminemail;
                                    ViewBag.EditState = true;
                                    ViewBag.ProfileType = "admin";
                                    ViewBag.UserMenuMsg = "Successfully deleted profile: " + userProf.email;
                                    LogClass.LogItem(adminemail, "Info", "Deleted Profile: " + userProf.email);
                                }
                                else
                                {
                                    LogClass.LogItem(adminemail, "Error", "Failed PUT request for user");
                                    return NotFound();
                                }
                            }
                        }
                        else
                        {
                            LogClass.LogItem(adminemail, "Warning", "Unauthorised access");
                            return BadRequest();
                        }
                    }
                    else
                    {
                        LogClass.LogItem(adminemail, "Error", "Profile does not exist!");
                        return BadRequest();
                    }
                }
            }
            else
            {
                LogClass.LogItem(adminemail, "Error", "Failed to retrieve users: " + response);
                return NotFound();
            }
            return View();
        }

        private bool verifyAdmin(string adminEmail)
        {
            bool verified = false;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + adminEmail, Method.Get);
            RestResponse response = client.Get(req);
            if (response.IsSuccessStatusCode)
            {
                Profile adminProf = JsonConvert.DeserializeObject<Profile>(response.Content);
                if (adminProf != null)
                {
                    if (adminProf.type.Equals("admin"))
                    {
                        verified = true;
                    }
                }
            }

            return verified;
        }

        // ISSUE: Only displays the transactions as is
        public async Task<IActionResult> TransManage(string email)
        {
            string decodedEmail = decodeString(email);
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + decodedEmail, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                Profile adminProf = JsonConvert.DeserializeObject<Profile>(response.Content);
                if (adminProf == null)
                {
                    LogClass.LogItem(decodedEmail, "Error", "admin does not exist!");
                    return NotFound();
                }
                else
                {
                    List<Transaction> transactions = null;
                    RestClient client2 = new RestClient(httpURL);
                    RestRequest req2 = new RestRequest("/api/btransaction", Method.Get);
                    RestResponse response2 = await client2.GetAsync(req2);
                    if (response2.IsSuccessStatusCode)
                    {
                        if (adminProf.type.Equals("admin"))
                        {
                            transactions = JsonConvert.DeserializeObject<List<Transaction>>(response2.Content);
                            ViewBag.TransactionList = transactions;
                            ViewBag.ProfileEmail = decodedEmail;
                            ViewBag.ProfileType = adminProf.type;
                            ViewBag.SortByAcc = false;
                            LogClass.LogItem(decodedEmail, "Info", "Retrieved Transactions data");
                        }
                        else
                        {
                            ViewBag.ProfileEmail = null;
                            LogClass.LogItem(decodedEmail, "Error", "Invalid Access");
                        }
                    }
                    else
                    {
                        LogClass.LogItem(decodedEmail, "Error", "Failed to retrieve transactions: " + response2);
                        return NotFound();
                    }
                }
            }
            else
            {
                LogClass.LogItem(decodedEmail, "Error", "Failed to retrieve admin: " + response);
                return NotFound();
            }
            return View();
        }

        [HttpPost("sorttrans")]
        public async Task<IActionResult> TransManage(string adminemail, string sorttype, string sortNum)
        {
            List<Transaction> transactions = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction", Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                if (transactions != null)
                {
                    List<Transaction> sortedList = new List<Transaction>();
                    if (sorttype.Equals("id-ascending"))
                    {
                        sortedList = sortTranList(transactions, "ascend", "id");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("id-decending"))
                    {
                        sortedList = sortTranList(transactions, "decend", "id");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("fromacc-ascending"))
                    {
                        sortedList = sortTranList(transactions, "ascend", "fromAcc");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("fromacc-decending"))
                    {
                        sortedList = sortTranList(transactions, "decend", "fromAcc");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("acc-byacc"))
                    {
                        if (sortNum.Equals("empty"))
                        {
                            ViewBag.SortByAcc = true;
                            ViewBag.TransactionList = transactions;
                            LogClass.LogItem(adminemail, "Info", "Transaction Sorting: " + sorttype + " - " + sortNum);
                        }
                        else
                        {
                            ViewBag.SortByAcc = true;
                            if(Int32.TryParse(sortNum, out int findByAcc))
                            {
                                sortedList = sortTranList(findByAcc);
                                ViewBag.TransactionList = sortedList;
                                LogClass.LogItem(adminemail, "Info", "Transaction Sorting: " + sorttype + " - " + sortNum);
                            }
                        }
                    }
                    else if (sorttype.Equals("toacc-ascending"))
                    {
                        sortedList = sortTranList(transactions, "ascend", "toAcc");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("toacc-decending"))
                    {
                        sortedList = sortTranList(transactions, "decend", "toAcc");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("amount-ascending"))
                    {
                        sortedList = sortTranList(transactions, "ascend", "amount");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else if (sorttype.Equals("amount-decending"))
                    {
                        sortedList = sortTranList(transactions, "decend", "amount");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    else
                    {
                        LogClass.LogItem(adminemail, "Warning", "Invalid sort query");
                        ViewBag.SortByAcc = false;
                        ViewBag.TransactionList = sortedList;
                    }
                    LogClass.LogItem(adminemail, "Info", "Transaction Sorting: " + sorttype);

                    ViewBag.ProfileEmail = adminemail;
                    ViewBag.ProfileType = "admin";
                }
            }
            else
            {
                LogClass.LogItem(adminemail, "Error", "Failed to retrieve transactions: " + response);
                return NotFound();
            }
            return View();
        }

        private List<Transaction> sortTranList(List<Transaction> list, string type, string byWhat)
        {
            List<Transaction> sortedList = new List<Transaction>();
            if (byWhat.Equals("id"))
            {
                if (type.Equals("ascend"))
                {
                    sortedList = sortTrans(list, "id");
                }
                else if (type.Equals("decend"))
                {
                    sortedList = sortTrans(list, "id");
                    sortedList.Reverse();
                }
            }
            else if (byWhat.Equals("fromAcc"))
            {
                if (type.Equals("ascend"))
                {
                    sortedList = sortTrans(list, "fromAcc");
                }
                else if (type.Equals("decend"))
                {
                    sortedList = sortTrans(list, "fromAcc");
                    sortedList.Reverse();
                }
            }
            else if (byWhat.Equals("toAcc"))
            {
                if (type.Equals("ascend"))
                {
                    sortedList = sortTrans(list, "toAcc");
                }
                else if (type.Equals("decend"))
                {
                    sortedList = sortTrans(list, "toAcc");
                    sortedList.Reverse();
                }
            }
            else if (byWhat.Equals("amount"))
            {
                if (type.Equals("ascend"))
                {
                    sortedList = sortTrans(list, "amount");
                }
                else if (type.Equals("decend"))
                {
                    sortedList = sortTrans(list, "amount");
                    sortedList.Reverse();
                }
            }
            return sortedList;
        }

        private List<Transaction> sortTranList(int byAcc)
        {
            List<Transaction> transactions = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/btransaction/" + byAcc, Method.Get);
            RestResponse response = client.Get(req);
            if (response.IsSuccessStatusCode)
            {
                transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                if(transactions != null)
                {
                    return transactions;
                }
            }
            return null;
        }

        private List<Transaction> sortTrans(List<Transaction> list, string field)
        {
            List<Transaction> clonedList = new List<Transaction>();

            for (int i = 0; i < list.Count; i++)
            {
                Transaction item = list[i];
                int currentIndex = i;

                if (field.Equals("id"))
                {
                    while (currentIndex > 0 && clonedList[currentIndex - 1].Id > item.Id)
                    {
                        currentIndex--;
                    }
                }
                else if (field.Equals("fromAcc"))
                {
                    while (currentIndex > 0 && clonedList[currentIndex - 1].accountNumber > item.accountNumber)
                    {
                        currentIndex--;
                    }
                }
                else if (field.Equals("toAcc"))
                {
                    while (currentIndex > 0 && clonedList[currentIndex - 1].toAccountNumber > item.toAccountNumber)
                    {
                        currentIndex--;
                    }
                }
                else if (field.Equals("amount"))
                {
                    while (currentIndex > 0 && clonedList[currentIndex - 1].amount > item.amount)
                    {
                        currentIndex--;
                    }
                }

                clonedList.Insert(currentIndex, item);
            }

            return clonedList;
        }

        public async Task<IActionResult> Logs(string email)
        {
            string decodedEmail = decodeString(email);
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/bprofile/" + decodedEmail, Method.Get);
            RestResponse response = await client.GetAsync(req);
            if (response.IsSuccessStatusCode)
            {
                Profile adminProf = JsonConvert.DeserializeObject<Profile>(response.Content);
                if (adminProf == null)
                {
                    LogClass.LogItem(decodedEmail, "Error", "admin does not exist!");
                    return NotFound();
                }
                else
                {
                    if (adminProf.type.Equals("admin"))
                    {
                        ViewBag.LogList = LogClass.getLogs();
                        ViewBag.ProfileEmail = decodedEmail;
                        ViewBag.ProfileType = adminProf.type;
                        LogClass.LogItem(decodedEmail, "Info", "Logs access");
                    }
                }
            }
            else
            {
                LogClass.LogItem(decodedEmail, "Error", "Failed to retrieve admin: " + response);
                return NotFound();
            }
            return View();
        }

        public IActionResult Transaction(string email)
        {
            ViewBag.ProfileEmail = decodeString(email);
            ViewBag.ProfileType = "user";
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