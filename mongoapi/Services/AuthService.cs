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
            // Você pode adicionar validações ou lógica de negócios aqui
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
    }
}
