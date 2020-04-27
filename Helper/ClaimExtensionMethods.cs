using System.Linq;
using System.Security.Claims;

namespace ChatServer.Helper {
    public static class ClaimExtensionMethods {
        public static string GetClaimValue (this ClaimsPrincipal input, string claimName) {
            return input.Claims.Where (c => c.Type == claimName)
                .Select (x => x.Value).FirstOrDefault ();
        }

        public static bool HasClaim (this ClaimsPrincipal input, string claimName) {
            return input.HasClaim (claim => claim.Value == claimName);
        }
    }
}