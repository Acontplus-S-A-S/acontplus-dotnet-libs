namespace Acontplus.TestApplication.DTOs
{
    public record CreateDiaDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}

