using VMS.Core.Entities;

namespace VMS.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, Role? role);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiry();
    DateTime GetRefreshTokenExpiry();
}
