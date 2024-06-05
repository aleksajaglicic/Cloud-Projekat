using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PortfolioService.Controller
{
    [RoutePrefix("api/DiffInWorth")]
    public class DifferenceInWorthController : ApiController
    {
        private DifferenceInWorthRepository _repository = new DifferenceInWorthRepository();

        [HttpPost]
        [Route("create_difference")]
        public IHttpActionResult CreateDifference(DifferenceInWorth difference)
        {
            bool success = _repository.CreateDifferenceEntry(difference);
            if (success)
            {
                return Ok("Difference entry created successfully");
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("get_difference_by_user_id_currency")]
        public IHttpActionResult GetDifferenceByUserIdCurrency(string user_id, string currency)
        {
            decimal difference = _repository.GetDifferenceByUserIdCurrency(user_id, currency);
            return Ok(difference);
        }
    }
}
