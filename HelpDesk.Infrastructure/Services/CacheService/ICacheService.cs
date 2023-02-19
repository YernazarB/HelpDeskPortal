namespace HelpDesk.Infrastructure.Services.CacheService
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key, CancellationToken token) where T : class;
        Task SetData<T>(string key, T value, CancellationToken token) where T : class;
        Task RemoveData(string key, CancellationToken token);
    }
}
