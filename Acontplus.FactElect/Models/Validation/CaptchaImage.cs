namespace Acontplus.FactElect.Models.Validation;

public class CaptchaImage
{
    public string imageName { get; set; }
    public string imageFieldName { get; set; }
    public List<string> values { get; set; }
    public string audioFieldName { get; set; }
}