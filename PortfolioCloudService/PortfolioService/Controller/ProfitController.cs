

using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PortfolioService.Controller
{
    [RoutePrefix("api/Profit")]
    public class ProfitController : ApiController
    {
        private readonly ProfitRepository _profitRepository;
        private readonly TransactionController _transactionController;
        private readonly DifferenceInWorthController _differenceController;
        public TransactionRepository _transactionRepository = new TransactionRepository();
        public DifferenceInWorthRepository _differenceRepository = new DifferenceInWorthRepository();

        [HttpPost]
        [Route("create_new_entry")]
        public IHttpActionResult CreateEntry(Profit profit)
        {
            try
            {
                _profitRepository.CreateOrUpdate(profit);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("calculate_portfolio")]
        public IHttpActionResult CalculatePortfolio()
        {
            try
            {
                var profits = _profitRepository.GetAllProfits();

                foreach (var profit in profits)
                {
                    var userId = profit.User_id;
                    decimal netWorth = 0.0M;

                    //var transactions = _transactionController.GetTransactionByUser(userId);
                    var transactions = _transactionRepository.GetTransactionByUser(userId);

                    if (transactions != null)
                    {
                        foreach (var transaction in transactions)
                        {
                            if (transaction.Type == TypeTransaction.BOUGHT)
                            {
                                netWorth += transaction.Amount_paid_dollars;
                            }
                            else
                            {
                                netWorth -= transaction.Amount_paid_dollars;
                            }
                        }
                    }

                    List<Transaction> userCryptoPortfolioToday = _transactionRepository.GetCryptoCurrenciesByUser(userId, false);
                    List<Transaction> userCryptoPortfolioYesterday = _transactionRepository.GetCryptoCurrenciesByUser(userId, true);

                    double totalValueYesterday = 0.0;
                    double totalValueToday = 0.0;

                    foreach (var todayEntry in userCryptoPortfolioToday)
                    {
                        string currency = todayEntry.Currency;
                        double todayAmount = (double)todayEntry.Amount_paid_dollars;

                        decimal yesterdayAmount = userCryptoPortfolioYesterday.FirstOrDefault(x => x.Currency == currency)?.Amount_paid_dollars ?? 0.0M;
                        double difference = (double)(todayAmount - (double)yesterdayAmount);

                        totalValueYesterday += ConvertCryptoToDollars((double)yesterdayAmount, currency);
                        totalValueToday += ConvertCryptoToDollars(todayAmount, currency);

                        _differenceRepository.CreateDifferenceEntry(new DifferenceInWorth(userId, currency, difference));
                    }

                    double netWorthToday = totalValueToday;
                    double differenceToday = totalValueToday - totalValueYesterday;
                    TypeProfit type = differenceToday < 0.0F ? TypeProfit.LOSS : TypeProfit.PROFIT;

                    var newEntryProfit = new Profit
                    {
                        User_id = userId,
                        Type = type,
                        Net_worth = netWorthToday,
                        Summary = differenceToday
                    };

                    _profitRepository.CreateOrUpdate(newEntryProfit);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("get_summary_by_user_id")]
        public IHttpActionResult GetSummaryByUserId(string userId)
        {
            try
            {
                var summary = _profitRepository.GetProfitByUserId(userId);
                if (summary != null)
                {
                    return Ok(summary);
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

        private double ConvertCryptoToDollars(double amount, string currency)
        {
            // Implement crypto to dollars conversion logic here
            return amount; // Placeholder implementation
        }
    }
}
