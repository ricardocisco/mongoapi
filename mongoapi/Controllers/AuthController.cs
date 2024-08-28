﻿using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using mongoapi.Models;
using mongoapi.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace mongoapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _authService.LoginAsync(request.Email, request.Password) != null)
            {
                return BadRequest("User already exists.");
            }

            var user = new User
            {
                Nome = request.Nome,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Preferences = new Preferences(),
                Emails = new Emails()
            };

            await _authService.RegisterAsync(user);

            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token, user, message = "Login successful!" });
        }

        [HttpPost("users/{userId}/receivedEmails")]
        public async Task<IActionResult> AddReceivedEmail(string userId, [FromBody] ReceivedEmailRequest request)
        {
            var newEmail = new ReceivedEmail
            {
                ReceiveEmail = request.ReceiveEmail,
                ReceiveNome = request.ReceiveNome,
                Subject = request.Subject,
                Body = request.Body,
                ReceivedAt = request.ReceivedAt,
                IsSpam = request.IsSpam
            };

            var success = await _authService.AddReceivedEmailAsync(userId, newEmail);

            if (!success)
            {
                return NotFound("User not found.");
            }

            return Ok(new { message = "Received email added successfully!" });
        }

        [HttpPost("users/{userId}/sent-email")]
        public async Task<IActionResult> SendEmail(string userId, [FromBody] SendEmailRequest request)
        {
            var newSentEmail = new Email
            {
                EmailId = Guid.NewGuid().ToString(),
                SentEmail = request.SentEmail,
                SentNome = request.SentNome,
                Subject = request.Subject,
                Body = request.Body,
                SentAt = request.SentAt,
            };

            var success = await _authService.AddSentEmailAsync(userId, newSentEmail);

            if (!success)
            {
                return NotFound("User not found");
            }

            return Ok(new { message = "Email sent successfully and recorded!"});
        }

    }

    public class RegisterRequest
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ReceivedEmailRequest
    {
        public string ReceiveEmail { get; set; }
        public string ReceiveNome { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime ReceivedAt { get; set; }
        public bool IsSpam { get; set; }
    }

    public class SendEmailRequest
    {
        public string EmailId { get; set; } = Guid.NewGuid().ToString();
        public string SentEmail { get; set; }
        public string SentNome { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }
}
