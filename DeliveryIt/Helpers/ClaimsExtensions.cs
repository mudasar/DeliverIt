using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DeliverIt.Helpers
{
    public static class ClaimsExtensions
    {
        public static bool HasRole(this ClaimsPrincipal principal, string role)
        {
            return principal.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == role);
        }

        public static bool IsPartner(this ClaimsPrincipal principal)
        {
            return principal.HasRole("partner");
        }

        public static bool IsUser(this ClaimsPrincipal principal)
        {
            return principal.HasRole("user");
        }

        public static string GetId(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
