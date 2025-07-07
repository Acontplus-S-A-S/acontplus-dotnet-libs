namespace Acontplus.TestDomain.Entities;

public class Usuario : BaseEntity
{
    [MaxLength(50)]
    public required string Username { get; set; }
    [MaxLength(100)]
    public required string Email { get; set; }
}