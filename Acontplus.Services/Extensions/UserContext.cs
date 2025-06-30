namespace Acontplus.Services.Extensions;

public interface IUserContext
{
    int GetUserId();
    T GetClaimValue<T>(string claimName);
    string GetUserName();
    string GetEmail();
    string GetRoleName();
}

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public int GetUserId() { return httpContextAccessor.HttpContext!.User.GetUserId(); }
    public T GetClaimValue<T>(string claimName) { return httpContextAccessor.HttpContext!.User.GetClaimValue<T>(claimName) ?? default; }
    public string GetUserName() { return httpContextAccessor.HttpContext?.User.GetUsername(); }
    public string GetEmail() { return httpContextAccessor.HttpContext?.User.GetEmail(); }
    public string GetRoleName() { return httpContextAccessor.HttpContext?.User.GetRoleName(); }
}
