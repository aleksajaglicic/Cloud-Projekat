using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PortfolioService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace PortfolioService.Repository
{
    public class UserDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public UserDataRepository()
        {
            var connectionString = CloudConfigurationManager.GetSetting("DataConnectionString") ??
                                   ConfigurationManager.ConnectionStrings["DataConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DataConnectionString is not set in the configuration.");
            }

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("UserTable");
            _table.CreateIfNotExists();
        }

        public bool Exists()
        {
            return _table.Exists();
        }

        public List<string> GetFailedHealthCheckEmails()
        {
            var emails = _table.CreateQuery<User>()
                             .Where(u => u.PartitionKey == "User")
                             .Select(u => u.Email)
                             .ToList();

            return emails;
        }

        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }

        public void AddUser(User newUser)
        {
            TableOperation insertOperation = TableOperation.Insert(newUser);
            _table.Execute(insertOperation);
        }

        public User RetrieveUser(string email)
        {
            var result = (from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User" && g.Email == email
                          select g).FirstOrDefault();
            return result;
        }

        public void UpdateUser(User updatedUser)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<User>(updatedUser.PartitionKey, updatedUser.RowKey);
            TableResult retrievedResult = _table.Execute(retrieveOperation);
            User existingUser = retrievedResult.Result as User;

            if (existingUser != null)
            {
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.Email = updatedUser.Email;
                existingUser.Password = updatedUser.Password;
                existingUser.City= updatedUser.City;
                existingUser.Country = updatedUser.Country;
                existingUser.Address = updatedUser.Address;

                TableOperation updateOperation = TableOperation.Replace(existingUser);
                _table.Execute(updateOperation);
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public void DeleteUser(string email)
        {
            var user = RetrieveUser(email);
            if (user != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(user);
                _table.Execute(deleteOperation);
            }
            else
            {
                throw new Exception("User not found");
            }
        }

    }

}
