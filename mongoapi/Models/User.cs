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
        public string Nome { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("preferences")]
        public Preferences Preferences { get; set; }

        [BsonElement("emails")]
        public Emails Emails { get; set; }
    }

    public class Preferences
    {
        [BsonElement("theme")]
        public string Theme { get; set; }
    }

    public class Emails
    {
        [BsonElement("sent")]
        public List<Email> Sent { get; set; }

        [BsonElement("received")]
        public List<ReceivedEmail> Received { get; set; }
    }

    public class Email
    {
        [BsonElement("emailId")]
        public string EmailId { get; set; }

        [BsonElement("recipient")]
        public string Recipient { get; set; }

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
        public string EmailId { get; set; }

        [BsonElement("sender")]
        public string Sender { get; set; }

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
