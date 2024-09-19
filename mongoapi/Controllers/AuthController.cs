using Bogus;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using mongoapi.Models;
using mongoapi.Services;
using MongoDB.Bson.Serialization.Attributes;
using System;

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
                return BadRequest("Usuario já existente!");
            }

            var user = new User
            {
                Nome = request.Nome,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Preferences = new Preferences(),
                Emails = new Emails()
            };

            await _authService.RegisterAsync(user);

            return Ok(new { message = "Usuario cadastrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email e senha são obrigatorios!.");
            }

            var user = await _authService.LoginAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized("Email ou senha invalidos.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token, user, message = "Login realizado!" });
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
                ReceivedAt = request.ReceivedAt 
            };

            newEmail.IsSpam = await _authService.IsSpam(newEmail);

            var success = await _authService.AddReceivedEmailAsync(userId, newEmail);

            if (!success)
            {
                return NotFound("Usuario não encontrado.");
            }

            return Ok(new { message = "Email recebido com sucesso!" });
        }

        [HttpPost("users/{userId}/sendEmail")]
        public async Task<IActionResult> SendEmail(string userId, [FromBody] SendEmailRequest request)
        {
            var faker = new Faker("en");

            var sentNome = string.IsNullOrEmpty(request.SentNome) ? faker.Name.FullName() : request.SentNome;

            var emails = await _authService.GetUserEmails(userId);

            var newSentEmail = new Email
            {
                EmailId = Guid.NewGuid().ToString(),
                SentEmail = request.SentEmail,
                SentNome = sentNome,
                Subject = request.Subject,
                Body = request.Body,
                SentAt = request.SentAt,
            };
            var i = 0;

            DateTime? sentDataHoraCorrespondente = null;

            while (i == 0)
            {
                foreach (var email in emails.Sent)
                {
                    if (email.SentEmail == newSentEmail.SentEmail && email.Body == newSentEmail.Body)
                    {
                        sentDataHoraCorrespondente = email.SentAt;

                        i = 1;
                    }
                }
            };

            TimeSpan toleranciaTempo = TimeSpan.FromMinutes(1);

            if (sentDataHoraCorrespondente.HasValue)
            {
                TimeSpan diferencaTempo = DateTime.Now - sentDataHoraCorrespondente.Value;
                if (diferencaTempo > toleranciaTempo)
                {
                    var success = await _authService.AddSentEmailAsync(userId, newSentEmail);

                    if (!success)
                    {
                        return NotFound("Usuario não encontrado");
                    }
                    return Ok(new { message = "Email enviado com sucesso!" });
                }
                else
                {
                    return BadRequest(new { message = "Email não enviado, possível SPAM!" });
                }
            }
            else
            {
                return Ok(new { message = "Email enviado com sucesso!" });
            }
        }           
            
        [HttpGet("users/{userId}/emails")]
        public async Task<IActionResult> GetUserEmails(string userId)
        {
            var emails = await _authService.GetUserEmails(userId);

            if (emails == null)
            {
                return NotFound("Usuario não encontrado");
            }

            return Ok(emails);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            var result = await _authService.ResetPasswordByEmailAsync(dto.Email, dto.NewPassword);
            if (result)
            {
                return Ok("Senha redefinida com sucesso.");
            }

            return BadRequest("Falha ao redefinir a senha. Verifique o e-mail e tente novamente.");
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
        public string SentNome { get; set; }
        public string SentEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class ArchiveEmailsRequest
    {
        public List<string> EmailIds { get; set; }
        public string EmailType { get; set; } 
    }

    public class TrashEmailsRequest
    {
        public List<string> EmailIds { get; set; }
        public string EmailType { get; set; } 
    }

    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
