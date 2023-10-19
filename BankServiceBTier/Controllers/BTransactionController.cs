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
        // GET: api/btransaction                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            List<Transaction> trans = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions", Method.Get);
            RestResponse response = await client.GetAsync(req);
            trans = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
            if (trans == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(trans);
            }
        }

        // GET: api/btransaction/accno/5
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<Transaction>> GetTransactions(int accountNumber)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions/" + accountNumber, Method.Get);
            RestResponse response = await client.GetAsync(req);
            List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
            if (response.IsSuccessStatusCode)
            {
                if (transactions == null)
                {
                    transactions = new List<Transaction>();
                }
                return Ok(transactions);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        // GET: api/btransaction/accno/5
        [HttpGet("accNo/{accNo}")]
        public async Task<IActionResult> GetTransaction(int accNo)
        {
            List<Transaction> trans = null;
            List<Transaction> transactions = new List<Transaction>();
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions", Method.Get);
            RestResponse response = await client.GetAsync(req);
            trans = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
            if (trans == null)
            {
                return NotFound();
            }
            else
            {
                for(int i = 0; i < trans.Count; i++)
                {
                    if (trans[i].accountNumber == accNo)
                    {
                        transactions.Add(trans[i]);
                    }
                }

                if(transactions.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(transactions);
                }
            }
        }

        // PUT: api/btransaction/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrans(uint id, [FromBody] Transaction transData)
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

        // POST: api/btransaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTrans(Transaction transData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions", Method.Post);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(transData);
            RestResponse response = await client.PostAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return CreatedAtAction("PostTransaction", "Transaction successfully made.");
            }
            else
            {
                return CreatedAtAction("FailTransaction", "Transaction failed.");
            }
        }

        // DELETE: api/btransaction/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrans(uint id)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/transactions/" + id, Method.Delete);
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
