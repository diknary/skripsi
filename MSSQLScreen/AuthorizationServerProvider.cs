using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Threading.Tasks;
using MSSQLScreen.Models;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System;

namespace MSSQLScreen
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private ApplicationDbContext _context;

        public AuthorizationServerProvider()
        {
            _context = new ApplicationDbContext();
        }

        private void SetIsConnected(int id)
        {
            var admin = _context.AdminAccounts.First(c => c.Id == id);
            admin.IsConnected = true;
            _context.SaveChanges();
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
            var getUser = _context.AdminAccounts.Single(c => c.Username == username);
            var getLastLogin = _context.AdminLogs.Where(c => c.AdminAccountId == getUser.Id).OrderByDescending(c => c.Id).FirstOrDefault();
            getUser.LastLogin = getLastLogin.LoginDate;
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
            var usrInDb = _context.AdminAccounts.SingleOrDefault(c => c.Username == client.UserName && c.Password == client.Password);
            var identity = new ClaimsIdentity(client.Options.AuthenticationType);
            
            if (usrInDb == null)
            {
                client.SetError("access_token", "null");
                return;
            }
            else
            {
                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "name", usrInDb.Name
                    },
                    {
                        "admin_id", usrInDb.Id.ToString()
                    },
                    {
                        "status", usrInDb.Privilege
                    }

                });
                var ticket = new AuthenticationTicket(identity, props);
                
                identity.AddClaim(new Claim(ClaimTypes.Name, usrInDb.Name));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usrInDb.Username));
                identity.AddClaim(new Claim(ClaimTypes.Role, usrInDb.Privilege));
                client.Validated(ticket);
                SetIsConnected(usrInDb.Id);
                CreateLoginHistory(usrInDb.Id);
                GetLastLogin(client.UserName);
            }
        }
    }
}