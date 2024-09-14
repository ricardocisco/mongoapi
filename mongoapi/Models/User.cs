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

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("preferences")]
        public Preferences Preferences { get; set; }

        [BsonElement("emails")]
        public Emails Emails { get; set; } = new Emails();
    }

    public class Preferences
    {
        [BsonElement("theme")]
        public string Theme { get; set; } = "light";

        [BsonElement("fontsize")]
        public float FontSize { get; set; } = 16f;

        [BsonElement("emailsortorder")]
        public string EmailSortOrder { get; set; } = "date";

        [BsonElement("language")]
        public string Language { get; set; } = "br";
    }

    public class Emails
    {
        [BsonElement("sent")]
        public List<Email> Sent { get; set; } = new List<Email>();

        [BsonElement("received")]
        public List<ReceivedEmail> Received { get; set; } = new List<ReceivedEmail>();

        [BsonElement("archived")]
        public List<ArchivedEmail> Archived { get; set; } = new List<ArchivedEmail>();

        [BsonElement("trash")]
        public List<TrashEmail> Trash { get; set; } = new List<TrashEmail>();
    }

    public class Email
    {
        [BsonElement("emailId")]
        public string? EmailId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("sent")]
        public string? SentEmail { get; set; }
        [BsonElement("sentNome")]
        public string? SentNome { get; set; }

        [BsonElement("subject")]
        public string? Subject { get; set; }

        [BsonElement("body")]
        public string? Body { get; set; }

        [BsonElement("sentAt")]
        public DateTime? SentAt { get; set; }
    }

    public class ReceivedEmail
    {
        [BsonElement("emailId")]
        public string? EmailId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("receivedEmail")]
        public string? ReceiveEmail { get; set; }

        [BsonElement("receivedNome")]
        public string? ReceiveNome { get; set; }

        [BsonElement("subject")]
        public string? Subject { get; set; }

        [BsonElement("body")]
        public string? Body { get; set; }

        [BsonElement("receivedAt")]
        public DateTime? ReceivedAt { get; set; }

        [BsonElement("isSpam")]
        public bool? IsSpam { get; set; }
    }

    public class EmailDataBase
    {
        public string EmailId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        [BsonElement("sent")]
        public string? SentEmail { get; set; }
        [BsonElement("sentNome")]
        public string? SentNome { get; set; }
        [BsonElement("sentAt")]
        public DateTime? SentAt { get; set; }


        [BsonElement("receivedEmail")]
        public string? ReceiveEmail { get; set; }
        [BsonElement("receivedNome")]
        public string? ReceiveNome { get; set; }
        [BsonElement("receivedAt")]
        public DateTime? ReceivedAt { get; set; }
        [BsonElement("isSpam")]
        public bool? IsSpam { get; set; }
    }

    public class ArchivedEmail
    {
        [BsonElement("emailId")]
        public string EmailId { get; set; }

        [BsonElement("emailType")]
        public string EmailType { get; set; }

        [BsonElement("emailData")]
        public EmailDataBase EmailDataBase { get; set; }
    }

    public class TrashEmail
    {
        [BsonElement("emailId")]
        public string EmailId { get; set; }

        [BsonElement("emailType")]
        public string EmailType { get; set; }

        [BsonElement("emailData")]
        public EmailDataBase EmailDataBase { get; set; }
    }
}
