using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesar_consol
{
    public class Result
    {
        public List<SimpleResult> Results {  get; set; }
        public string Type { get; set; }
        public Result(List<SimpleResult> results, string type) { 
            Results = results;
            Type = type;
        }

    }
}
