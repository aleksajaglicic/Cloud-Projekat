using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace HealthMonitoringService
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public IHttpActionResult GetFailedHealthCheckEmails()
        {
            try
            {

                UserDataRepository userDataRepository = new UserDataRepository(); 
                List<string> failedHealthCheckEmails = userDataRepository.GetFailedHealthCheckEmails();

                return Ok(failedHealthCheckEmails);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
