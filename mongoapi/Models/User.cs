using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography.Xml;

namespace mongoapi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nome")]
        public string Nome { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("preferences")]
        public Preferences Preferences { get; set; }

        [BsonElement("emails")]
        public Emails Emails { get; set; } = new Emails();
    }

    public class Preferences
    {
        [BsonElement("theme")]
        public string Theme { get; set; }
    }

    public class Emails
    {
        [BsonElement("sent")]
        public List<Email> Sent { get; set; } = new List<Email>();

        [BsonElement("received")]
        public List<ReceivedEmail> Received { get; set; } = new List<ReceivedEmail>();
    }

    public class Email
    {
        [BsonElement("emailId")]
        public string EmailId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("sent")]
        public string SentEmail { get; set; }
        [BsonElement("sentNome")]
        public string SentNome { get; set; }

        [BsonElement("subject")]
        public string Subject { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }

        [BsonElement("sentAt")]
        public DateTime SentAt { get; set; }
    }

    public class ReceivedEmail
    {
        [BsonElement("emailId")]
        public string EmailId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("receivedEmail")]
        public string ReceiveEmail { get; set; }

        [BsonElement("receivedNome")]
        public string ReceiveNome { get; set; }

        [BsonElement("subject")]
        public string Subject { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }

        [BsonElement("receivedAt")]
        public DateTime ReceivedAt { get; set; }

        [BsonElement("isSpam")]
        public bool IsSpam { get; set; }
    }
}
