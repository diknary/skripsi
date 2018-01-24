using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Threading.Tasks;
using MSSQLScreen.Models;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

namespace MSSQLScreen
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private ApplicationDbContext _context;

        public AuthorizationServerProvider()
        {
            _context = new ApplicationDbContext();
        }

        private int ValidateUser(string username, string password)
        {
            var usr = _context.UserAccounts.SingleOrDefault(c => c.Username == username);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Convert.FromBase64String(usr.Salt));

            tdes.Key = pdb.CryptDeriveKey("TripleDES", "SHA1", 192, tdes.IV);

            if (Convert.ToBase64String(tdes.Key) == usr.Password)
                return usr.Id;
            else
                return 0;
        }

        private void CreateLoginHistory(int id)
        {
            var addloginhis = new AdminLog
            {
                LoginDate = DateTime.Now,
                AdminAccountId = id
            };
            _context.AdminLogs.Add(addloginhis);
            _context.SaveChanges();

        }

        private void GetLastLogin(string username)
        {
            var getUser = _context.UserAccounts.Single(c => c.Username == username);
            var getAdmin = _context.AdminAccounts.Single(c => c.UserAccountId == getUser.Id);
            var getLastLogin = _context.AdminLogs.Where(c => c.AdminAccountId == getAdmin.Id).OrderByDescending(c => c.Id).FirstOrDefault();
            getAdmin.LastLogin = getLastLogin.LoginDate;
            _context.SaveChanges();
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext client)
        {
            client.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext client)
        {
            int userId = ValidateUser(client.UserName, client.Password);
            if (userId ==  0)
            {
                client.SetError("Login failed", "Username/password is wrong");
                return;
            }
            else
            {
                var admInDb = _context.AdminAccounts.SingleOrDefault(c => c.UserAccountId == userId);
                var identity = new ClaimsIdentity(client.Options.AuthenticationType);

                if (admInDb == null)
                {
                    client.SetError("Not Found", "Admin is not exist");
                    return;
                }
                else
                {
                    var usr = _context.UserAccounts.Single(c => c.Id == userId);
                    var props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        {
                            "name", usr.Name
                        },
                        {
                            "admin_id", usr.Id.ToString()
                        },
                        {
                            "status", admInDb.Privilege
                        }

                    });
                    identity.AddClaim(new Claim(ClaimTypes.Role, admInDb.Privilege));
                    var ticket = new AuthenticationTicket(identity, props);

                    CreateLoginHistory(admInDb.Id);
                    GetLastLogin(client.UserName);
                    client.Validated(ticket);
                }
            }
        }
    }
}