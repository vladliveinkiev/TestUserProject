using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TestUserProject.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
