namespace Demo.Application.Dtos
{
    public class UpdateDiaDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}

