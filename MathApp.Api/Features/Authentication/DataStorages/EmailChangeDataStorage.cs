using Microsoft.Extensions.Caching.Memory;

namespace MathAppApi.Features.Authentication.DataStorages;

public class EmailChangeDataStorage : IEmailChangeDataStorage
{
    private readonly IMemoryCache _memoryCache;

    public EmailChangeDataStorage()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }
    
    public string RegisterEmailUpdate(string userId, string newMail)
    {
        var emailUpdateId = Guid.NewGuid().ToString();
        _memoryCache.Set(emailUpdateId, (userId, newMail), TimeSpan.FromMinutes(15));
        return emailUpdateId;
    }

    public (string userId, string newMail)? GetEmailUpdateRequest(string requestId)
    {
        return _memoryCache.Get<(string, string)>(requestId);
    }
}