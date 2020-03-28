using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace JsonWebToken.Filters
{
    public class AuthorizeApiAttribute : Attribute, IAuthenticationFilter
    {

        public string userRoles
            = string.Empty;


        private static string secretKey
            = ConfigurationManager.AppSettings["secretKey"].ToString();
        public string Realm { get; set; }

        public bool AllowMultiple => false;
        public AuthorizeApiAttribute()
        {

        }

        public AuthorizeApiAttribute(Role roles)
        {
            userRoles = roles.ToString();
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = GetPrincipal(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
            {
                var claims = principal.Claims.ToList();

                var roles = claims.Where(x => x.Type.Trim().Equals("Roles")).Select(x => x.Value).ToString();

                if (roles.Equals(userRoles))
                {
                    context.Principal = principal;
                }
                else
                {
                    context.ErrorResult = new AuthenticationFailureResult("User not authorized to perform following action", request);
                }
            }
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                JwtSecurityToken jwtToken
                    = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (jwtToken == null) return null;

                byte[] key = Convert.FromBase64String(secretKey);

                TokenValidationParameters parameters =
                    new TokenValidationParameters()
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                SecurityToken securityToken;
                ClaimsPrincipal principal
                    = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
        /*
        private static string ValidateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;

            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }

            Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }
        */
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}