﻿using Microsoft.AspNetCore.Http;
using RestSharp;
using Newtonsoft.Json;
using BankDataWebService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankServiceBTier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BProfileController : ControllerBase
    {
        private readonly string httpURL = "http://localhost:5027";

        // --- PROFILE CONTROLLER FUNCTIONALITY ---
        // GET: api/bprofile                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            List<Profile> profiles = null;
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/profiles", Method.Get);
            RestResponse response = await client.GetAsync(req);
            profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
            if (profiles == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(profiles);
            }
        }

        // GET: api/bprofile/{email}
        [HttpGet("{email}")]
        public async Task<ActionResult<Profile>> GetProfiles(string email)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/profiles/" + email, Method.Get);
            RestResponse response = await client.GetAsync(req);
            Profile acc = JsonConvert.DeserializeObject<Profile>(response.Content);
            if (acc == null)
            {
                return NotFound("Not Found: " + email);
            }
            else
            {
                return Ok(acc);
            }
        }

        // PUT: api/bprofile/{email}
        [HttpPut("{email}")]
        public async Task<IActionResult> PutProfile(string email, [FromBody] Profile profileData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/profiles/" + email, Method.Put);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(profileData);
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

        // POST: api/bprofile
        [HttpPost]
        public async Task<IActionResult> PostProfile( [FromBody] Profile profileData)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/profiles", Method.Post);
            req.RequestFormat = RestSharp.DataFormat.Json;
            req.AddBody(profileData);
            RestResponse response = await client.PostAsync(req);
            if (response.IsSuccessStatusCode)
            {
                return CreatedAtAction("PostProfile", response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }

        // DELETE: api/bprofile/5
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteProfile(string email)
        {
            RestClient client = new RestClient(httpURL);
            RestRequest req = new RestRequest("/api/profiles/" + email, Method.Delete);
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
