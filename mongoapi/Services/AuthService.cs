using mongoapi.Models;
using System.Security.Claims;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace mongoapi.Services
{
    public class AuthService
    {
        private readonly MongoDBService _mongoDBService;
        private readonly IConfiguration _configuration;

        public AuthService(MongoDBService mongoDBService, IConfiguration configuration)
        {
            _mongoDBService = mongoDBService;
            _configuration = configuration;
        }

        public async Task<User?> RegisterAsync(User user)
        {
            await _mongoDBService.CreateAsync(user);
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var users = await _mongoDBService.GetAsync();
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> AddReceivedEmailAsync(string userId, ReceivedEmail newEmail)
        {
            var user = await _mongoDBService.GetUserByIdAsync(userId);
            if (user == null) return false;

            newEmail.EmailId = Guid.NewGuid().ToString();
            user.Emails.Received.Add(newEmail);

            await _mongoDBService.UpdateAsync(userId, user);

            return true;
        }

        public async Task<bool> AddSentEmailAsync(string userId, Email newSentEmail)
        {
            var user = await _mongoDBService.GetUserByIdAsync(userId);
            if(user == null) return false;

            user.Emails.Sent.Add(newSentEmail);

            await _mongoDBService.UpdateAsync(userId, user);


            return true;
        }

    }
}
