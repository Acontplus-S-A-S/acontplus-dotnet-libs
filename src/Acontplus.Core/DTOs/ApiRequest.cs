namespace Acontplus.Core.DTOs;

//Deprecated: Use own DTOs for API requests instead of this generic one
public class ApiRequest
{
    public int UserRoleId { get; set; }
    public bool FromMobile { get; set; }
    public required Dictionary<string, object> Data { get; set; }
}
