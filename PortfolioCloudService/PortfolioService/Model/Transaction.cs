using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioService.Model
{
    public enum TypeTransaction { BOUGHT, SOLD }

    public class Transaction : TableEntity
    {
        private int id;
        private string user_id;
        private string date_and_time;
        private TypeTransaction type;
        private string currency;
        private decimal amount_paid_dollars;

        public int Id { get => id; set => id = value; }
        public string User_id { get => user_id; set => user_id = value; }
        public string Date_and_time { get => date_and_time; set => date_and_time = value; }
        public TypeTransaction Type { get => type; set => type = value; }
        public string Currency { get => currency; set => currency = value; }
        public decimal Amount_paid_dollars { get => amount_paid_dollars; set => amount_paid_dollars = value; }

        public Transaction(int id, string user_id, string date_and_time, TypeTransaction type, string currency, decimal amount_paid_dollars)
        {
            PartitionKey = "Transaction";
            RowKey = user_id.ToString();
            this.Id = id;
            this.User_id = user_id;
            this.Date_and_time = date_and_time;
            this.Type = type;
            this.Currency = currency;
            this.Amount_paid_dollars = amount_paid_dollars;
        }

        public Transaction()
        {
        }
    }
}
