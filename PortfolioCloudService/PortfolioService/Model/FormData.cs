using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PortfolioService.Model
{
    public class FormData
    {
        public string User { get; set; }
        public HttpPostedFileBase File { get; set; }
    }

}
