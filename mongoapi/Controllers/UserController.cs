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

        [HttpPost("{userId}/moveFromArchived")]
        public async Task<IActionResult> MoveEmailsFromArchived(string userId, [FromBody] MoveFromArchived request)
        {
            var success = await _mongoDBService.MoveEmailsFromArchived(userId, request.EmailIds);
            if (success)
            {
                return Ok();
            }
            return BadRequest("Failed to move emails from archive.");
        }

        [HttpPost("{userId}/moveFromTrash")]
        public async Task<IActionResult> MoveEmailsFromTrash(string userId, [FromBody] MoveFromArchived request)
        {
            var success = await _mongoDBService.MoveEmailsFromTrash(userId, request.EmailIds);
            if (success)
            {
                return Ok();
            }
            return BadRequest("Failed to move emails from trash.");
        }

        [HttpPost("deleteEmails")]
        public async Task<IActionResult> DeleteEmails([FromBody] DeleteEmailsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.EmailIds == null || request.EmailIds.Count == 0)
            {
                return BadRequest("Request inválido.");
            }

            var deleted = await _mongoDBService.DeleteEmailsFromTrashAsync(request.UserId, request.EmailIds);
            if (deleted)
            {
                return Ok();
            }

            return NotFound("Usuário ou emails não encontrados.");
        }

        [HttpPut("users/{id}/theme")]
        public async Task<IActionResult> UpdateTheme(string id, [FromBody] UpdateThemeRequest request)
        {
            var user = await _mongoDBService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            user.Preferences.Theme = request.Theme;
            await _mongoDBService.UpdateAsync(id, user);

            return Ok(user);
        }
    }

    public class UpdateThemeRequest
    {
        public string Theme { get; set; }
    }

    public class MoveFromArchived
    {
        public List<string> EmailIds { get; set; }
    }


    public class MoveEmailsRequest
    {
        public List<string> EmailIds { get; set; }
        public string EmailType { get; set; }
    }

    public class DeleteEmailsRequest
    {
        public string UserId { get; set; }
        public List<string> EmailIds { get; set; }
    }

}
