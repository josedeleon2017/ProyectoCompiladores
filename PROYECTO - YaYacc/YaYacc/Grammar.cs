using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    public class Grammar
    {
        public Rule InitialRule { get; set; }
        public List<Rule> DictRules { get; set; }
        public List<string> Terminals { get; set; }
        public List<string> NonTerminals { get; set; }
    }
}
