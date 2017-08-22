﻿using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Threading.Tasks;
using MSSQLScreen.Models;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace MSSQLScreen
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private ApplicationDbContext _context;

        public AuthorizationServerProvider()
        {
            _context = new ApplicationDbContext();
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
            var usrInDb = _context.UserAccounts.SingleOrDefault(c => c.Username == client.UserName && c.Password == client.Password);
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
                        "status", usrInDb.Privilege
                    }

                });
                var ticket = new AuthenticationTicket(identity, props);
                identity.AddClaim(new Claim(ClaimTypes.Name, usrInDb.Name));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usrInDb.Username));
                client.Validated(ticket);
            }
        }
    }
}