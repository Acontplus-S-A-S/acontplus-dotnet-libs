namespace Acontplus.TestApplication.Dtos
{
    public class UsuarioDto
    {
        public int? Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
    public record UserDto(int? Id, string Username, string Email)
    {
        public static UserDto FromUsuarioDto(UsuarioDto dto) =>
            new(dto.Id, dto.Username, dto.Email);
    }
}

