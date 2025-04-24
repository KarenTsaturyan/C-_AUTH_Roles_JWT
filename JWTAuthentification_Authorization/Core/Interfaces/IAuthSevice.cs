using JWTAuthentification_Authorization.Core.Dtos;

namespace JWTAuthentification_Authorization.Core.Interfaces
{
    public interface IAuthSevice
    {
        Task<AuthServiceResponceDto> SeedRolesAsync();
        Task<AuthServiceResponceDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthServiceResponceDto> LoginAsync(LoginDto loginDto);
        Task<AuthServiceResponceDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto);
        Task<AuthServiceResponceDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto);
    }
}
