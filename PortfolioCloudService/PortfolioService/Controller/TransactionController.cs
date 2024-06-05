using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PortfolioService.Controller
{
    [RoutePrefix("api/Crypto")]
    public class TransactionController : ApiController
    {
        TransactionRepository transactionRepository = new TransactionRepository();

        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateCrypto(Transaction transaction)
        {
            if (transactionRepository.Create(transaction))
            {
                return Ok("Successfully created transaction");
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("get_rates")]
        public async Task<IHttpActionResult> GetCurrencyRates()
        {
            string url = "https://currency-exchange-api-six.vercel.app/api/v2/currencies/today";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var exchangeRates = await response.Content.ReadAsAsync<object>(); // Replace 'object' with a specific model if available
                    return Ok(exchangeRates);
                }
                catch (HttpRequestException httpRequestException)
                {
                    // Log detailed HttpRequestException
                    if (httpRequestException.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {httpRequestException.InnerException.Message}");
                    }
                    System.Diagnostics.Debug.WriteLine($"HttpRequestException: {httpRequestException.Message}");
                    return InternalServerError(httpRequestException);
                }
                catch (Exception ex)
                {
                    // Log detailed Exception
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    return InternalServerError(ex);
                }
            }
        }

        [HttpPost]
        [Route("delete")]
        public IHttpActionResult DeleteCrypto(string user_id, string currency)
        {
            if (transactionRepository.DeleteCrypto(user_id, currency))
            {
                return Ok("Successfully deleted transaction");
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("create_sale")]
        public IHttpActionResult CreateSaleTransaction(Transaction transaction)
        {
            if (transactionRepository.CreateSaleTransaction(transaction))
            {
                return Ok("Successfully created sale for transaction");
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("get_transaction_by_user")]
        public IHttpActionResult GetTransactionByUser([FromBody] dynamic data)
        {
            string user_id = data.user_id;
            List<Transaction> transactions = transactionRepository.GetTransactionByUser(user_id);
            if (transactions != null)
            {
                return Ok(transactions);
            }
            else
            {
                return InternalServerError();
            }
        }


        [HttpPost]
        [Route("get_crypto_currencies_by_user")]
        public IHttpActionResult GetCryptoCurrenciesByUser(string user_id, bool is_yesterday = false)
        {
            List<Transaction> transactions = transactionRepository.GetCryptoCurrenciesByUser(user_id, is_yesterday);
            if (transactions != null)
            {
                return Ok(transactions);
            }
            else
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("delete_by_id")]
        public IHttpActionResult DeleteById([FromBody] dynamic data)
        {
            try
            {
                string transactionIdString = data.transaction_id;
                if (!Guid.TryParse(transactionIdString, out Guid transactionId))
                {
                    return BadRequest("Invalid transaction ID format");
                }

                if (transactionRepository.DeleteTransactionById(transactionId))
                {
                    return Ok("Successfully deleted transaction");
                }
                else
                {
                    return NotFound(); // Return 404 if the transaction with the specified ID was not found
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [HttpPost]
        [Route("find_max_id")]
        public IHttpActionResult FindMaxId()
        {
            int maxId = transactionRepository.FindMaxId();
            if (maxId != 0)
            {
                return Ok(maxId);
            }
            else
            {
                return InternalServerError();
            }
        }
    }
}
