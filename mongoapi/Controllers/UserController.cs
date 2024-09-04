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
        public async Task<List<User>> Get()
        {
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            await _mongoDBService.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPost("{userId}/moveToArchived")]
        public async Task<IActionResult> MoveEmailsToArchived(string userId, [FromBody] MoveEmailsRequest request)
        {
            var success = await _mongoDBService.MoveEmailsToArchived(userId, request.EmailIds, request.EmailType);
            if (success)
            {
                return Ok();
            }
            return BadRequest("Failed to move emails.");
        }

        [HttpPost("{userId}/moveToTrash")]
        public async Task<IActionResult> MoveEmailsToTrash(string userId, [FromBody] MoveEmailsRequest request)
        {
            var success = await _mongoDBService.MoveEmailsToTrash(userId, request.EmailIds, request.EmailType);
            if (success)
            {
                return Ok();
            }
            return BadRequest("Failed to move emails.");

        }

    }


    public class MoveEmailsRequest
    {
        public List<string> EmailIds { get; set; }
        public string EmailType { get; set; }
    }
}
