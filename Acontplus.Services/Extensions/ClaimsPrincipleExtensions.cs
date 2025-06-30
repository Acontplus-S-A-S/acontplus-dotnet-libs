namespace Acontplus.Services.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }

    public static string GetRoleName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    }

    // Método genérico para obtener claims
    // Obtener claims personalizados
    //int companyId = user.GetClaimValue<int>("companyId");
    //string idCardCompany = user.GetClaimValue<string>("idCardCompany");
    //bool isActive = user.GetClaimValue<bool>("isActive");
    //Guid tenantId = user.GetClaimValue<Guid>("tenantId");

    // Obtener claims estándar
    //string email = user.GetClaimValue<string>(ClaimTypes.Email);
    //int userId = user.GetClaimValue<int>(ClaimTypes.NameIdentifier);
    public static T GetClaimValue<T>(this ClaimsPrincipal user, string claimName)
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

            // Para otros tipos, intenta una conversión genérica
            return (T)Convert.ChangeType(claim, typeof(T));
        }
        catch
        {
            return default;
        }
    }
}
