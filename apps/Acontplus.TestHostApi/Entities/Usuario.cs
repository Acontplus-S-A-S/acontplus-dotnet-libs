namespace Acontplus.TestHostApi.Entities
{
    //[Table("usuario", Schema = "seguridad")]
    public class Usuario : BaseEntity
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
