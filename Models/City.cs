using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASP.NET_HW_22.Models;

public class City {
    [BsonId]
    public ObjectId Id { get; set; }

    public string? Name { get; set; }

    public ObjectId? CountryId { get; set; }

    [BsonIgnore]
    public Country? Country { get; set; }
}