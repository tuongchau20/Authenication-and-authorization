using System.ComponentModel.DataAnnotations;

namespace Authorize.Model
{
    public class LoginModel
    {
        [Key]
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
