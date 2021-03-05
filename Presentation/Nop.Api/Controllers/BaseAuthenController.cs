using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Nop.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseAuthenController : ControllerBase
    {
        private System.Collections.Generic.IEnumerable<System.Security.Claims.Claim> claims
        {
            get
            {
                string jwt = Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization")).Value.ToString().Split(" ").LastOrDefault();
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(jwt).Claims;
            }
        }

        protected int currentMemberId
        {
            get
            {
                return int.Parse(claims.SingleOrDefault(n => n.Type == JwtRegisteredClaimNames.NameId).Value);
            }
        }
    }
}