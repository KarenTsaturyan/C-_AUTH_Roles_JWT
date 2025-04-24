using System.ComponentModel.DataAnnotations;

namespace JWTAuthentification_Authorization.Core.Dtos
{
    public class UpdatePermissionDto
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
    }
}
