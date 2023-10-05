using MongoDB.Driver;

namespace ASP.NET_HW_22.Data; 

public interface IDbService {
    public Task<IMongoCollection<T>> GetCollection<T>(string name);
}