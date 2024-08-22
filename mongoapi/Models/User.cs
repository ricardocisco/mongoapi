using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mongoapi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nome")]
        public string Nome { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;
    }
}
