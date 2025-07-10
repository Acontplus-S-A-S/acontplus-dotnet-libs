namespace Acontplus.Services.Extensions;

/// <summary>
/// Provides access to user claims and identity information from the current HTTP context.
/// </summary>
public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    /// <summary>
    /// Gets the user ID from the current user claims.
    /// </summary>
    public int GetUserId() { return httpContextAccessor.HttpContext!.User.GetUserId(); }

    /// <summary>
    /// Gets a claim value of the specified type from the current user claims.
    /// </summary>
    /// <typeparam name="T">The type of the claim value.</typeparam>
    /// <param name="claimName">The name of the claim.</param>
    public T GetClaimValue<T>(string claimName) { return httpContextAccessor.HttpContext!.User.GetClaimValue<T>(claimName) ?? default; }

    /// <summary>
    /// Gets the user name from the current user claims.
    /// </summary>
    public string GetUserName() { return httpContextAccessor.HttpContext?.User.GetUsername(); }

    /// <summary>
    /// Gets the email from the current user claims.
    /// </summary>
    public string GetEmail() { return httpContextAccessor.HttpContext?.User.GetEmail(); }

    /// <summary>
    /// Gets the role name from the current user claims.
    /// </summary>
    public string GetRoleName() { return httpContextAccessor.HttpContext?.User.GetRoleName(); }
}
