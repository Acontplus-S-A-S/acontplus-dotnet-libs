namespace Acontplus.Core.Abstractions.Context;

public interface IUserContext
{
    int GetUserId();
    T GetClaimValue<T>(string claimName);
    string GetUserName();
    string GetEmail();
    string GetRoleName();
}