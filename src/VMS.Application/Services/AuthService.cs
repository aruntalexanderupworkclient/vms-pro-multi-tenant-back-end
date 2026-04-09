using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Auth;
using VMS.Application.DTOs.Common;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;
using VMS.Shared.Helpers;

namespace VMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthService(IUnitOfWork uow, ITokenService tokenService, IGoogleAuthService googleAuthService)
    {
        _uow = uow;
        _tokenService = tokenService;
        _googleAuthService = googleAuthService;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Repository<User>().Query()
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return ApiResponse<LoginResponseDto>.FailResponse("Invalid email or password.", "AUTH_INVALID_CREDENTIALS");

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            return ApiResponse<LoginResponseDto>.FailResponse("Invalid email or password.", "AUTH_INVALID_CREDENTIALS");

        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.FailResponse("Account is deactivated.", "AUTH_ACCOUNT_INACTIVE");

        var response = await GenerateTokenResponseAsync(user, cancellationToken);
        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful.");
    }

    public async Task<ApiResponse<LoginResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _uow.Repository<User>().Query()
            .IgnoreQueryFilters()
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser)
            return ApiResponse<LoginResponseDto>.FailResponse("Email already registered.", "AUTH_EMAIL_EXISTS");

        var tenant = new Tenant
        {
            Name = request.TenantName ?? $"{request.FullName}'s Organization",
            IsActive = true
        };

        var adminRole = new Role
        {
            Name = "Admin",
            IsAdmin = true,
            TenantId = tenant.Id
        };
        await _uow.Repository<Role>().AddAsync(adminRole, cancellationToken);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Phone = request.Phone,
            TenantId = tenant.Id,
            RoleId = adminRole.Id,
            IsActive = true
        };
        await _uow.Repository<User>().AddAsync(user, cancellationToken);

        var refreshTokenStr = _tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            Token = refreshTokenStr,
            ExpiresAt = _tokenService.GetRefreshTokenExpiry()
        };
        await _uow.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user, adminRole);

        return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiry(),
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            User = MapUserInfo(user, adminRole)
        }, "Registration successful.");
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingToken = await _uow.Repository<RefreshToken>().Query()
            .IgnoreQueryFilters()
            .Include(rt => rt.User!)
                .ThenInclude(u => u.Role)
            .Where(rt => !rt.IsDeleted)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked, cancellationToken);

        if (existingToken == null || existingToken.ExpiresAt < DateTime.UtcNow)
            return ApiResponse<LoginResponseDto>.FailResponse("Invalid or expired refresh token.", "AUTH_INVALID_REFRESH_TOKEN");

        var user = existingToken.User!;
        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.FailResponse("Account is deactivated.", "AUTH_ACCOUNT_INACTIVE");

        existingToken.IsRevoked = true;
        var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
        existingToken.ReplacedByToken = newRefreshTokenStr;
        _uow.Repository<RefreshToken>().Update(existingToken);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            Token = newRefreshTokenStr,
            ExpiresAt = _tokenService.GetRefreshTokenExpiry()
        };
        await _uow.Repository<RefreshToken>().AddAsync(newRefreshToken, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user, user.Role);

        return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshTokenStr,
            AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiry(),
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
            User = MapUserInfo(user, user.Role)
        }, "Token refreshed successfully.");
    }

    public async Task<ApiResponse<LoginResponseDto>> GoogleLoginAsync(GoogleLoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(request.IdToken);
        if (googleUser == null)
            return ApiResponse<LoginResponseDto>.FailResponse("Invalid Google token.", "AUTH_INVALID_GOOGLE_TOKEN");

        var user = await _uow.Repository<User>().Query()
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Email == googleUser.Email, cancellationToken);

        if (user == null)
        {
            var tenant = new Tenant
            {
                Name = $"{googleUser.Name}'s Organization",
                IsActive = true
            };

            user = new User
            {
                FullName = googleUser.Name,
                Email = googleUser.Email,
                GoogleId = googleUser.GoogleId,
                ProfilePictureUrl = googleUser.Picture,
                TenantId = tenant.Id,
                IsActive = true
            };
            await _uow.Repository<User>().AddAsync(user, cancellationToken);
        }
        else
        {
            if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = googleUser.GoogleId;
                user.ProfilePictureUrl ??= googleUser.Picture;
                _uow.Repository<User>().Update(user);
            }
        }

        var response = await GenerateTokenResponseAsync(user, cancellationToken);
        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Google login successful.");
    }

    private async Task<LoginResponseDto> GenerateTokenResponseAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user, user.Role);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            Token = refreshTokenStr,
            ExpiresAt = _tokenService.GetRefreshTokenExpiry()
        };

        await _uow.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiry(),
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            User = MapUserInfo(user, user.Role)
        };
    }

    private static UserInfoDto MapUserInfo(User user, Role? role) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        TenantId = user.TenantId,
        RoleId = user.RoleId,
        RoleName = role?.Name,
        IsAdmin = role?.IsAdmin ?? false,
        ProfilePictureUrl = user.ProfilePictureUrl
    };
}
