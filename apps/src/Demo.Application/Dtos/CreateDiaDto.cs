namespace Demo.Application.Dtos
{
    public record CreateDiaDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}

