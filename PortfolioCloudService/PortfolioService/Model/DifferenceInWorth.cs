using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioService.Model
{
    public class DifferenceInWorth : TableEntity
    {
        private string user_id;
        private string currency;
        private double difference;

        public DifferenceInWorth(string user_id, string currency, double difference)
        {
            PartitionKey = "DifferenceInWorth";
            RowKey = user_id.ToString();
            this.User_id = user_id;
            this.Currency = currency;
            this.Difference = difference;
        }

        public string User_id { get => user_id; set => user_id = value; }
        public string Currency { get => currency; set => currency = value; }
        public double Difference { get => difference; set => difference = value; }
    }
}
