using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioService.Model
{
    public class User : TableEntity
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Country { get; set; }
        public int PhoneNumber { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }
        public String ImgUrl { get; set; }

        public User(string firstName, string lastName,
            string address, string city, string country, 
            int phoneNumber, string email, string password, 
            string imgUrl)
        {
            PartitionKey = "User";
            RowKey = email;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            City = city;
            Country = country;
            PhoneNumber = phoneNumber;
            Email = email;
            Password = password;
            ImgUrl = imgUrl;
        }

        public User() { }

        public override string ToString()
        {
            return $"{FirstName},{LastName},{Email}";
        }
    }

}
