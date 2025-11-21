using Microsoft.AspNetCore.Identity;

namespace Schematics.API.Data.Entities;

public class User : IdentityUser
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}
