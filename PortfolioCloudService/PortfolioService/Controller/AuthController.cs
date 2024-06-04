using Microsoft.Owin.Security;
using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace PortfolioService.Controller
{
    [RoutePrefix("api/Auth")]
    public class AuthController : ApiController
    {
        UserDataRepository repository = new UserDataRepository();

        [HttpPost]
        [Route("register")]
        public IHttpActionResult RegisterUser([FromBody] User user)
        {
            try
            {
                var newUser = new User(user.FirstName, user.LastName, user.Address, 
                    user.City, user.Country, user.PhoneNumber, user.Email, user.Password, user.ImgUrl);
                repository.AddUser(newUser);
                return Ok($"Successfully registered user: {user.ToString()}");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult LoginUser([FromBody] User user)
        {
            try
            {
                foreach (User userLogin in repository.RetrieveAllUsers())
                {
                    if(userLogin.Email.Equals(user.Email) && userLogin.Password.Equals(user.Password))
                    {
                        return Ok($"Successfully logged in user: {user.ToString()}");
                    }
                    else
                    {
                        return Ok($"Unsuccessfully logged in user: {user.ToString()}");
                    }
                }
                return Ok($"Successfully logged in user: {user.ToString()}");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
