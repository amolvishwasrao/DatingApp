using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.Dtos
{
    public class UserForRegsiterDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8,MinimumLength=4 ,ErrorMessage="You must Specify Password between 4 and 8 charcters")]
        public string Password { get; set; }
    }
}  