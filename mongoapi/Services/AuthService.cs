using mongoapi.Models;

namespace mongoapi.Services
{
    public class AuthService
    {
        private readonly MongoDBService _mongoDBService;

        public AuthService(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
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
    }
}
