namespace Acontplus.TestDomain.Entities;

public class Dia : Entity<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
