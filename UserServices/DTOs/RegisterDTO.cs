using System.ComponentModel.DataAnnotations;

namespace UserServices.DTOs
{
    public class RegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "The password must be at least 8 characters long and include uppercase letters, lowercase letters, and numbers.")]
        public string Password { get; set; }
    }
}
