using System.ComponentModel.DataAnnotations;

namespace ChatServer.Model.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
    }
}