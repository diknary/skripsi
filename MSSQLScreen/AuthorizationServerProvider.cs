using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Threading.Tasks;
using MSSQLScreen.Models;
using System.Security.Claims;

namespace MSSQLScreen
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private ApplicationDbContext _context;

        public AuthorizationServerProvider()
        {
            _context = new ApplicationDbContext();
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext client)
        {
            client.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext client)
        {
            var usrInDb = _context.UserAccounts.SingleOrDefault(c => c.Username == client.UserName && c.Password == client.Password);
            var identity = new ClaimsIdentity(client.Options.AuthenticationType);
            if (usrInDb == null)
            {
                client.SetError("invalid_grant", "Invalid username or password");
                return;
            }
            else
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, usrInDb.Privilege));
                identity.AddClaim(new Claim(ClaimTypes.Name, usrInDb.Username));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usrInDb.Username));
                client.Validated(identity);
            }
        }
    }
}