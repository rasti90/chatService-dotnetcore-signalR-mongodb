using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Helper
{
    public static class ClaimExtensionMethods
    {
        public static string GetClaimValue(this ClaimsPrincipal input, string claimName) {
            return input.Claims.Where(c => c.Type == claimName)
                    .Select(x => x.Value).FirstOrDefault();
        }
    }
}