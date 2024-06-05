using PortfolioService.Repository;
using System.Collections.Generic;
using System.Web.Http;

namespace HealthMonitoringService
{
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("api/users/failedHealthCheckEmails")]
        public IHttpActionResult GetFailedHealthCheckEmails()
        {
            // Retrieve user email addresses from the repository
            UserDataRepository userDataRepository = new UserDataRepository(); 
            List<string> failedHealthCheckEmails = userDataRepository.GetFailedHealthCheckEmails();

            return Ok(failedHealthCheckEmails);
        }
    }
}
