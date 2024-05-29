using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        public IHttpActionResult EditUser([FromBody] string email)
        {
            try
            {
                repository.RetrieveUser(email);
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
                // Get the multipart form data from the request
                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());
                await Request.Content.ReadAsMultipartAsync(provider);

                // Get user data from form data
                string user = provider.FormData["user"];

                // Get the file content from form data
                HttpContent fileContent = null;
                if (provider.FileData.Count > 0)
                {
                    fileContent = provider.Contents[1];
                }

                // Handle file upload if provided
                if (fileContent != null)
                {
                    // Read the file content into a memory stream
                    var memoryStream = new MemoryStream();
                    using (var stream = await fileContent.ReadAsStreamAsync())
                    {
                        await stream.CopyToAsync(memoryStream);
                    }

                    // Convert memory stream to byte array
                    byte[] fileBytes = memoryStream.ToArray();

                    // Pass the byte array to your upload method
                    await UploadImageToBlobStorageAsync(fileBytes, user, cancellationToken);

                    // Dispose the memory stream
                    memoryStream.Dispose();
                }

                // Deserialize user data from the form data
                User tempUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(user);
                User newUser = new User()
                {
                    FirstName = tempUser.FirstName,
                    LastName = tempUser.LastName,
                    Password = tempUser.Password,
                    Country = tempUser.Country,
                    City = tempUser.City,
                    Address = tempUser.Address,
                    Email = tempUser.Email,
                    ImgUrl = "", // Set this later if necessary
                    PhoneNumber = 123, // Update with actual phone number
                    Timestamp = DateTime.Now // Add timestamp
                };

                // Add user to repository
                repository.AddUser(newUser);

                return Ok("Successfully added user.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Method to upload the byte array to Azure Blob Storage
        async Task UploadImageToBlobStorageAsync(byte[] fileBytes, string user, CancellationToken cancellationToken)
        {
            try
            {
                // Create a unique blob name using the user's email
                string uniqueBlobName = string.Format("image_{0}", user);

                // Get storage account and blob client
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Get reference to the container
                var container = blobClient.GetContainerReference("images");

                // Get reference to the blob
                var blob = container.GetBlockBlobReference(uniqueBlobName);

                // Upload the byte array to blob storage
                await blob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length, cancellationToken);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error uploading image to blob storage: {ex.Message}");
                throw; // Re-throw the exception to be handled in the calling method
            }
        }

        [HttpGet]
        [Route("getUser")]
        public IHttpActionResult GetUser([FromBody] string email)
        {
            try
            {
                repository.RetrieveUser(email);
                return Ok("Successfully changed user.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
