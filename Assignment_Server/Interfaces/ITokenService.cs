using Assignment_Server.Models;

namespace Assignment_Server.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(User user,IList<string> roles);
    }
}
