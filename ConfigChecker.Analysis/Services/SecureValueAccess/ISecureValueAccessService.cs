
namespace ConfigChecker.Analysis.Services.SecureValueAccess;

public interface ISecureValueAccessService
{
    Task<bool> DoesConfigValueMatch(string redisKey, dynamic configValue);
}