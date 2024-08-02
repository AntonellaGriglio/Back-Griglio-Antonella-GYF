using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Api.Comandos.Usuarios;

public class LoginCmd
{
    public string? Usuario { get; set; }
    public string? Password { get; set; }
}
