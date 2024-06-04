using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Http;

namespace PortfolioService.Controller
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        UserDataRepository repository = new UserDataRepository();
        //private BlobHelper blobHelper = new BlobHelper();

        [HttpPost]
        [Route("edit")]
        public IHttpActionResult EditUser([FromBody] User user)
        {
            try
            {
                User _user = repository.RetrieveUser(user.Email);
                if(_user != null)
                {
                    repository.UpdateUser(_user);
                }

                return Ok("Successfully changed user.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("delete")]
        public IHttpActionResult DeleteUser([FromBody] string email)
        {
            try
            {
                repository.DeleteUser(email);
                return Ok("Successfully changed user.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddUserAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());
                await Request.Content.ReadAsMultipartAsync(provider);

                string user = provider.FormData["user"];


                var fileContent = provider.FileData.First();
                var tempPath = fileContent.LocalFileName;


                byte[] fileBytes = File.ReadAllBytes(tempPath);

                User tempUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(user);
                tempUser.PartitionKey = "User";
                tempUser.RowKey = tempUser.Email;

                string uniqueBlobName = string.Format("avatar_{0}", tempUser.Email);

                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("images");
                await container.CreateIfNotExistsAsync();

                var blob = container.GetBlockBlobReference(uniqueBlobName);

                await blob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length, cancellationToken);

                repository.AddUser(tempUser);

                return CreatedAtRoute("GetUserById", new { id = tempUser.Email }, "Successfully added user.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("getAvatar/{email}")]
        public async Task<IHttpActionResult> GetAvatarAsync(string email)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("images");
                var blob = container.GetBlockBlobReference($"avatar_{email}");

                if (await blob.ExistsAsync())
                {
                    Stream blobStream = await blob.OpenReadAsync();
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(blobStream)
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    return ResponseMessage(response);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("getUser")]
        public IHttpActionResult GetUser([FromBody] string email)
        {
            try
            {
                User user = repository.RetrieveUser(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("checkUser")]
        public IHttpActionResult CheckUser([FromBody] dynamic data)
        {
            try
            {
                string email = data.email; // Extract email from JSON object
                User user = repository.RetrieveUser(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
