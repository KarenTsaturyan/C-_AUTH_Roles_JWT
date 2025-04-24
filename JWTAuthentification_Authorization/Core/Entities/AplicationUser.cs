using Microsoft.AspNetCore.Identity;

namespace JWTAuthentification_Authorization.Core.Entities
{
    public class AplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
