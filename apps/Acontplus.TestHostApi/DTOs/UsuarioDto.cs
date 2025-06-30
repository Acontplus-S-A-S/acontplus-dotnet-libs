namespace Acontplus.TestHostApi.DTOs
{
    public class UsuarioDto
    {
        public int? Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
