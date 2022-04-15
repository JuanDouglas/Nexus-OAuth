using Nexus.OAuth.Libary.Models.Api.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Libary.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string Phone { get; set; }
        public string ValidationStatus { get; set; }

        internal Account(AccountResult account)
        {
            Id = account.Id;
            Name = account.Name;    
            Email = account.Email;
            Created = account.Created;
            Phone = account.Phone;
            ValidationStatus = account.ConfirmationStatus;
        }
    }
}
