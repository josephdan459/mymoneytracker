using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker
{
    public class RuleModel
    {
        public string Rule_name { get; set; }
        public string Category { get; set; }
        public string Payee_regex { get; set; }        
        public string Direction { get; set; }
    }
}
