using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] //specifies that when the class is called it needs to provide this field with a value.It cant't be null
        public string UserName{get; set;}
        
        [Required]
        [StringLength(8,MinimumLength =4)]
        public string Password{get; set;}
    }
}