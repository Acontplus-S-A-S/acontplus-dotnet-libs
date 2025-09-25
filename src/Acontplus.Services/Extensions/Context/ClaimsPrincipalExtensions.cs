namespace Acontplus.Services.Extensions.Context;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetRoleName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Retrieves the value of a specific claim from the ClaimsPrincipal and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the claim value will be converted.</typeparam>
    /// <param name="user">The ClaimsPrincipal instance from which the claim will be retrieved.</param>
    /// <param name="claimName">The name of the claim to retrieve.</param>
    /// <returns>The value of the claim converted to the specified type, or the default value of the type if the claim does not exist or cannot be converted.</returns>
    public static T? GetClaimValue<T>(this ClaimsPrincipal user, string claimName)
    {
        var claim = user.FindFirst(claimName)?.Value;
        if (claim == null)
            return default;

        try
        {
            // Manejo de tipos comunes
            if (typeof(T) == typeof(string))
                return (T)(object)claim;

            if (typeof(T) == typeof(int))
                return (T)(object)Convert.ToInt32(claim);

            if (typeof(T) == typeof(long))
                return (T)(object)Convert.ToInt64(claim);

            if (typeof(T) == typeof(bool))
                return (T)(object)Convert.ToBoolean(claim);

            if (typeof(T) == typeof(Guid))
                return (T)(object)Guid.Parse(claim);

            return (T)Convert.ChangeType(claim, typeof(T));
        }
        catch
        {
            return default;
        }
    }
}
