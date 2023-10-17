using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;
using BankDataWebService.Models;

namespace BankServiceBTier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BBankController : ControllerBase
    {
        private readonly string httpURL = "http://localhost:5027";

        // --- BANK CONTROLLER FUNCTIONALITY ---
        // GET: api/bbank
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bank>>> GetBanks()
        {
            List<Bank> banks = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/banks", Method.Get);
            RestResponse response = await client.GetAsync(req);
            banks = JsonConvert.DeserializeObject<List<Bank>>(response.Content);
            if (banks == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(banks);
            }
        }

        // GET: api/bbank/5
        [HttpGet("{email}")]
        public async Task<ActionResult<Bank>> GetBanks(string email)
        {
            List<Bank> banks = new List<Bank>();
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/banks", Method.Get);
            RestResponse response = await client.GetAsync(req);
            banks = JsonConvert.DeserializeObject<List<Bank>>(response.Content);
            if (banks == null)
            {
                return NotFound();
            }
            else
            {
                List<Bank> matchBank = new List<Bank>();
                foreach (Bank bank in banks) 
                { 
                    if (bank.email.Equals(email))
                    {
                        matchBank.Add(bank);
                    }
                }
                return Ok(matchBank);
            }
        }

        // PUT: api/bbank/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBank(uint id, [FromBody] Bank bankData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/banks/" + id, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(bankData);
            RestResponse response = await client.PutAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        // POST: api/bbank
        [HttpPost]
        public async Task<ActionResult<Bank>> PostBank([FromBody] Bank bankData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/banks", Method.Post);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(bankData);
            RestResponse response = await client.PostAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return CreatedAtAction("PostBank", response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        // DELETE: api/bbank/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(uint id)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/banks/" + id, Method.Delete);
            RestResponse response = await client.DeleteAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(response.Content);
            }
        }
    }
}
