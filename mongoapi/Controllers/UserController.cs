using Microsoft.AspNetCore.Mvc;
using mongoapi.Models;
using mongoapi.Services;
using System.Security.Claims;

namespace mongoapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public UserController(MongoDBService mongoDBService) { _mongoDBService = mongoDBService; }

        [HttpGet]
        public async Task<List<User>> Get() {
        return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user) { 
            await _mongoDBService.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new {id = user.Id }, user);
        }

    }
}
