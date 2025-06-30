namespace Acontplus.Core.DTOs;

public class ApiRequest
{
    public int UserRoleId { get; set; }
    public bool FromMobile { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
