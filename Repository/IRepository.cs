using MongoDB.Bson;

namespace ASP.NET_HW_22.Repository;

public interface IRepository<T> {
    public Task<IEnumerable<T?>> SelectAsync();
    public Task<T?> SelectAsync(ObjectId id);
    public Task InsertAsync(T obj);
    public Task UpdateAsync(ObjectId id, T obj);
    public Task DeleteAsync(ObjectId id);
    public Task<bool> AnyAsync(ObjectId id);
}