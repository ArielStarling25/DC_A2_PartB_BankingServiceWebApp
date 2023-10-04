using BankDataWebService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace BankServiceBTier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BTransactionController : ControllerBase
    {
        private readonly string httpURL = "http://localhost:5027";

        // --- TRANSACTION CONTROLLER FUNCTIONALITY ---
        // GET: api/Transactions                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            List<Transaction> profiles = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions", Method.Get);
            RestResponse response = await client.GetAsync(req);
            profiles = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
            if (profiles == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(profiles);
            }
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetProfiles(uint id)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions/" + id, Method.Get);
            RestResponse response = await client.GetAsync(req);
            Transaction acc = JsonConvert.DeserializeObject<Transaction>(response.Content);
            if (acc == null)
            {
                return NotFound("Not Found: " + id);
            }
            else
            {
                return Ok(acc);
            }
        }

        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBank(uint id, [FromBody] Transaction transData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions/" + id, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(transData);
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

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostBank([FromBody] Transaction transData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions", Method.Post);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(transData);
            RestResponse response = await client.PutAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return CreatedAtAction("PostTransaction", response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(uint id)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions/" + id, Method.Delete);
            RestResponse response = await client.GetAsync(req);
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
