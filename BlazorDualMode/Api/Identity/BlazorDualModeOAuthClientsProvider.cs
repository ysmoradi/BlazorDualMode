using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;

namespace BlazorDualMode.Api.Identity
{
    public class BlazorDualModeOAuthClientsProvider : OAuthClientsProvider
    {
        public override IEnumerable<Client> GetClients()
        {
            return new[]
            {
                GetResourceOwnerFlowClient(new BitResourceOwnerFlowClient
                {
                    ClientName = "BlazorDualModeResOwner",
                    ClientId = "BlazorDualModeResOwner",
                    Secret = "secret",
                    TokensLifetime = TimeSpan.FromDays(7),
                    Enabled = true
                })
            };
        }
    }
}
