using PortfolioService.Model;
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
        [HttpPost]
        [Route("create_difference")]
        public bool CreateDifference(DifferenceInWorth difference)
        {
            return true;
        }

        [HttpPost]
        [Route("get_difference_by_user_id_currency")]
        public bool GetDifferenceByUserIdCurrency(string user_id, string currency)
        {
            return true;
        }
    }
}
