using API.Models.Base;

namespace API.Models
{
    public class User : ModelBase
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string QuickSearch { get; set; }
    }
}