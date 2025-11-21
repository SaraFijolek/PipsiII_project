namespace Schematics.API.Service.Infrastructure;

public interface IJwtTokenService
{
    string CreateToken(string userId);
}
