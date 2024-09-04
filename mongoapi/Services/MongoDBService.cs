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

        public async Task CreateAsync(User user)
        {
            await _usercollection.InsertOneAsync(user);
            return;
        }

        public async Task<List<User>> GetAsync()
        {
            return await _usercollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _usercollection.Find(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string userId, User updatedUser)
        {
            await _usercollection.ReplaceOneAsync(user => user.Id == userId, updatedUser);
        }

        public async Task<bool> MoveEmailsToArchived(string userId, List<string> emailIds, string emailType)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var user = await _usercollection.Find(filter).FirstOrDefaultAsync();

            if (user == null) return false;

            foreach (var emailId in emailIds)
            {
                ArchivedEmail archivedEmail = null;

                if (emailType == "sent")
                {
                    var sentEmail = user.Emails.Sent.FirstOrDefault(e => e.EmailId == emailId);
                    if (sentEmail != null)
                    {
                        archivedEmail = new ArchivedEmail
                        {
                            EmailId = sentEmail.EmailId,
                            EmailType = "sent",
                            EmailDataBase = new EmailDataBase
                            {
                                EmailId = sentEmail.EmailId,
                                Subject = sentEmail.Subject,
                                Body = sentEmail.Body,
                                SentAt = sentEmail.SentAt,
                                SentEmail = sentEmail.SentEmail,
                                SentNome = sentEmail.SentNome
                            }
                        };
                        user.Emails.Sent.Remove(sentEmail);
                    }
                }
                else if (emailType == "received")
                {
                    var receivedEmail = user.Emails.Received.FirstOrDefault(e => e.EmailId == emailId);
                    if (receivedEmail != null)
                    {
                        archivedEmail = new ArchivedEmail
                        {
                            EmailId = receivedEmail.EmailId,
                            EmailType = "received",
                            EmailDataBase = new EmailDataBase
                            {
                                EmailId = receivedEmail.EmailId,
                                Subject = receivedEmail.Subject,
                                Body = receivedEmail.Body,
                                ReceivedAt = receivedEmail.ReceivedAt,
                                ReceiveEmail = receivedEmail.ReceiveEmail,
                                ReceiveNome = receivedEmail.ReceiveNome,
                                IsSpam = receivedEmail.IsSpam
                            }
                        };
                        user.Emails.Received.Remove(receivedEmail);
                    }
                }

                if (archivedEmail != null)
                {
                    user.Emails.Archived.Add(archivedEmail);
                }
            }

            var update = Builders<User>.Update.Set(u => u.Emails, user.Emails);
            var result = await _usercollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> MoveEmailsToTrash(string userId, List<string> emailIds, string emailType)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var user = await _usercollection.Find(filter).FirstOrDefaultAsync();

            if (user == null) return false;

            foreach (var emailId in emailIds)
            {
                TrashEmail trashEmail = null;
                if (emailType == "sent")
                {
                    var sentEmail = user.Emails.Sent.FirstOrDefault(e => e.EmailId == emailId);
                    if (sentEmail != null)
                    {
                        trashEmail = new TrashEmail
                        {
                            EmailId = sentEmail.EmailId,
                            EmailType = "sent",
                            EmailDataBase = new EmailDataBase
                            {
                                EmailId = sentEmail.EmailId,
                                Subject = sentEmail.Subject,
                                Body = sentEmail.Body,
                                SentAt = sentEmail.SentAt,
                                SentEmail = sentEmail.SentEmail,
                                SentNome = sentEmail.SentNome
                            }
                        };
                        user.Emails.Sent.Remove(sentEmail);
                    }
                }
                else if (emailType == "received")
                {
                    var receivedEmail = user.Emails.Received.FirstOrDefault(e => e.EmailId == emailId);
                    if (receivedEmail != null)
                    {
                        trashEmail = new TrashEmail
                        {
                            EmailId = receivedEmail.EmailId,
                            EmailType = "received",
                            EmailDataBase = new EmailDataBase
                            {
                                EmailId = receivedEmail.EmailId,
                                Subject = receivedEmail.Subject,
                                Body = receivedEmail.Body,
                                ReceivedAt = receivedEmail.ReceivedAt,
                                ReceiveEmail = receivedEmail.ReceiveEmail,
                                IsSpam = receivedEmail.IsSpam
                            }
                        };
                        user.Emails.Received.Remove(receivedEmail);
                    }
                }

                if (trashEmail != null)
                {
                    user.Emails.Trash.Add(trashEmail);
                }
            }

            var update = Builders<User>.Update
                .Set(u => u.Emails, user.Emails)
                .Set(u => u.Emails.Trash, user.Emails.Trash);

            var result = await _usercollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
