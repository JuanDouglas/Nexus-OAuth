using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Libary.Models.Api.Result
{
    internal class FirsStepResult
    {
        public int Id { get; set; }
        public string UserAgent { get; set; }
        public string Token { get; set; }
        public double? ExpiresIn { get; set; }
        public DateTime Date { get; set; }
    }
}
