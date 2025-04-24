using JWTAuthentification_Authorization.Core.Dtos;
using JWTAuthentification_Authorization.Core.Interfaces;
using JWTAuthentification_Authorization.Core.OtherObjects;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentification_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthSevice _authService;

        public AuthController(IAuthSevice authService)
        {
            _authService = authService;
        }

        // Route for seeding my roles to DB
        [HttpPost]
        [Route("seed-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedRoles = await _authService.SeedRolesAsync();
            return Ok(seedRoles);
        }

        // Route -> Register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResult = await _authService.RegisterAsync(registerDto);
            if (registerResult.IsSucceed)
            {
                return Ok(registerResult);
            }

            return BadRequest(registerResult);
        }

        // Route -> Login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);

            if (loginResult.IsSucceed)
            {
                return Ok(loginResult);
            }

            return Unauthorized(loginResult);
        }



        //Route -> make user -> admin
        [HttpPost]
        [Route("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
           var opperationResult = await _authService.MakeAdminAsync(updatePermissionDto);

            if (opperationResult.IsSucceed)
            {
                return Ok(opperationResult);
            }

            return BadRequest(opperationResult);
        }

        //Route -> make user -> owner
        [HttpPost]
        [Route("make-owner")]
        public async Task<IActionResult> MakeOwner([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var opperationResult = await _authService.MakeOwnerAsync(updatePermissionDto);

            if (opperationResult.IsSucceed)
            {
                return Ok(opperationResult);
            }

            return BadRequest(opperationResult);
        }

    }
}