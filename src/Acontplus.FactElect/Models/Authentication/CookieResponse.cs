namespace Acontplus.FactElect.Models.Authentication;

public class CookieResponse
{
    public required CookieContainer Cookie { get; set; }
    public required string Captcha { get; set; }
}