using JWTAuthentification_Authorization.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentification_Authorization.Core.DbContext
{
    public class ApplicationDbContext :  IdentityDbContext<AplicationUser> // IdentityDbContext default identity user
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}
