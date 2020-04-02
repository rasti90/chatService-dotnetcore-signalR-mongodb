using System.ComponentModel.DataAnnotations;

namespace ChatServer.Model.ViewModels
{
    public class AuthenticateVM
    {
        public string APIKey { get; set; }
        [Required]
        public string UserExternalId { get; set; }

        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}