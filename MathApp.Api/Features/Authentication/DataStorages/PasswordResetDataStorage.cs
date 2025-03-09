using Microsoft.Extensions.Caching.Memory;

namespace MathAppApi.Features.Authentication.DataStorages;

public class PasswordResetDataStorage : IPasswordResetDataStorage
{
    private readonly IMemoryCache _memoryCache;

    public PasswordResetDataStorage()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }
    
    public string RegisterPasswordReset(string userId)
    {
        var passwordResetId = Guid.NewGuid().ToString("N");
        _memoryCache.Set(passwordResetId, userId, TimeSpan.FromMinutes(15));//TODO: move to settings
        return passwordResetId;
    }

    public string? GetUserId(string resetId)
    {
        return _memoryCache.Get(resetId) as string;
    }
}