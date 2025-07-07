namespace Acontplus.FactElect.Dtos.Validation;

public record CaptchaImageDto
{
    public required string ImageName { get; set; }
    public required string ImageFieldName { get; set; }
    public required List<string> Values { get; set; }
    public required string AudioFieldName { get; set; }
}