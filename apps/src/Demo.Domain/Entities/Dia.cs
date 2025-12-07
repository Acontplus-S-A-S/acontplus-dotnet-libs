namespace Demo.Domain.Entities;

public class Dia
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
