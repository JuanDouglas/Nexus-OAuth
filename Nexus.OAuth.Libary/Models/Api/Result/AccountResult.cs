using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Libary.Models.Api.Result
{
    internal class AccountResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string Phone { get; set; }
        public string ConfirmationStatus { get; set; }
    }
}
