﻿using JWTAuthentification_Authorization.Core.Dtos;
using JWTAuthentification_Authorization.Core.Entities;
using JWTAuthentification_Authorization.Core.Interfaces;
using JWTAuthentification_Authorization.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthentification_Authorization.Core.Services
{
    public class AuthService : IAuthSevice
    {
        //public readonly UserManager<IdentityUser> _userManager; // we changed it in program.cs add Identity
        public readonly UserManager<AplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IConfiguration _configuration;

        public AuthService(UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthServiceResponceDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponceDto()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };
                //return Unauthorized("Invalid Credentials");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
            {
                return new AuthServiceResponceDto()
                {
                    IsSucceed = false,
                    Message = "Invalid Credentials"
                };
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("JWTID", Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJsonWebToken(authClaims);

            return new AuthServiceResponceDto()
            {
                IsSucceed = true,
                Message = token
            };
        }

        public async Task<AuthServiceResponceDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponceDto()
                {
                    IsSucceed = false,
                    Message = "Invalid User Name"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return new AuthServiceResponceDto()
            {
                IsSucceed = true,
                Message = "User is now ADMIN"
            };
        }

        public async Task<AuthServiceResponceDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponceDto()
                {
                    IsSucceed = false,
                    Message = "Invalid User Name"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return new AuthServiceResponceDto()
            {
                IsSucceed = true,
                Message = "User is now OWNER"
            };
        }

        public async Task<AuthServiceResponceDto> RegisterAsync(RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);

            if (isExistsUser != null)
            {
                return new AuthServiceResponceDto()
                {
                    IsSucceed = false,
                    Message = "UserName Already Exists"
                };
            }

            AplicationUser newUser = new AplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            // password should be checked by configs in Program.cs
            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User Creation Failed Because: ";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }

                return new AuthServiceResponceDto()
                {
                    IsSucceed = false,
                    Message = errorString
                };
            }
            // Add a Default role to all users
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            return new AuthServiceResponceDto()
            {
                IsSucceed = true,
                Message = "User Created Successfully"
            };
        }

        public async Task<AuthServiceResponceDto> SeedRolesAsync()
        {
            bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);

            if (isOwnerRoleExists && isAdminRoleExists && isUserRoleExists)
            {
                return new AuthServiceResponceDto()
                {
                    IsSucceed = true,
                    Message = "Role Seeding is Already Done"
                };
            }


            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            return new AuthServiceResponceDto()
            {
                IsSucceed = true,
                Message = "Role Seeding Done Successful"
            };
        }

        private string GenerateNewJsonWebToken(List<Claim> authClaims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
             );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}
