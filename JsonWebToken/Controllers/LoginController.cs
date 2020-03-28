using JsonWebToken.Core;
using JsonWebToken.Filters;
using System;
using System.Net;
using System.Web.Http;

namespace JsonWebToken.Controllers
{

    [RoutePrefix("api")]
    public class LoginController : ApiController
    {

        [Route("ValidateUser")]
        [HttpGet]
        [AllowAnonymous]
        public string ValidateUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("UserName and Password is mandatory");
            }

            UserManager manager
                = new UserManager();
            var userDetails
                = manager.ValidateUser(userName, password);
            if (!string.IsNullOrWhiteSpace(userDetails))
            {
                return userDetails;
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }


        [Route("FindUserById")]
        [HttpGet]
        [AuthorizeApi(Role.Admin)]
        public IHttpActionResult FindUserById(int employeeId)
        {
            if (employeeId > 0)
            {
                UserManager userManager
                    = new UserManager();

                return Ok(userManager.FindByName(employeeId));
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

    }
}
