using Microsoft.Extensions.Options;
using mongoapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mongoapi.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _usercollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _usercollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAsync(User user) { 
            await _usercollection.InsertOneAsync(user);
            return;
        }

        public async Task<List<User>> GetAsync()
        {
            return await _usercollection.Find(new BsonDocument()).ToListAsync();
        }

    }
}
