using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tic_Tac_Toe.Server.Models
{
    public sealed class UserAccount
    {
        [StringLength(maximumLength: 25, MinimumLength = 6)]
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [StringLength(maximumLength: 25, MinimumLength = 6)]
        [JsonPropertyName("password")]
        public string Password { get; set; }


        public UserAccount(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public static bool operator ==(UserAccount a, UserAccount b)
            => a.Login == b.Login && a.Password == b.Password;
        public static bool operator !=(UserAccount a, UserAccount b)
            => a != b;

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is not null && (UserAccount)obj == this);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
