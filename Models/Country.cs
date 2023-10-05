using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ASP.NET_HW_22.Models;

public class Country {
    [BsonId]
    public ObjectId Id { get; set; }

    public string? Name { get; set; }

    public int Population { get; set; }

    public double Area { get; set; }

    public ObjectId? CapitalObjectId { get; set; }

    [BsonIgnore]
    public City? Capital { get; set; }

    public ObjectId? ContinentObjectId { get; set; }

    [BsonIgnore]
    public Continent? Continent { get; set; }
}