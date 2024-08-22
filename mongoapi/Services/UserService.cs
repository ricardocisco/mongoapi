using mongoapi.Models;
using MongoDB.Driver;

namespace mongoapi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _users = database.GetCollection<User>(config["MongoDB:UserCollectionName"]);
        }

        public async Task<List<User>> GetAsync() => await _users.Find(user => true).ToListAsync();

        public async Task<User?> GetAsync(string id) => await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) => await _users.InsertOneAsync(user);
    }
}
