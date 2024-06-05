using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PortfolioService.Controller
{
    [RoutePrefix("api/check")]
    public class CheckController : ApiController
    {
        [HttpGet]
        [Route("isAlive")]
        public IHttpActionResult isAlive()
        {
            return Ok("Ok");
        }
    }
}
