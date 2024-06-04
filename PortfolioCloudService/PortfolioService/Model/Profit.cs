using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioService.Model
{
    public enum TypeProfit { PROFIT, LOSS };

    public class Profit : TableEntity
    {
        private int id;
        private string user_id;
        private TypeProfit type;
        private double summary;
        private double net_worth;

        public Profit(int id, string user_id, TypeProfit type, double summary, double net_worth)
        {
            PartitionKey = "Profit";
            RowKey = user_id.ToString();
            this.Id = id;
            this.User_id = user_id;
            this.Type = type;
            this.Summary = summary;
            this.Net_worth = net_worth;
        }

        public Profit()
        {
        }

        public int Id { get => id; set => id = value; }
        public string User_id { get => user_id; set => user_id = value; }
        public TypeProfit Type { get => type; set => type = value; }
        public double Summary { get => summary; set => summary = value; }
        public double Net_worth { get => net_worth; set => net_worth = value; }
    }
}
