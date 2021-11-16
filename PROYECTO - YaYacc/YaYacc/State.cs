using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    public class State
    {
        public int Id { get; set; }
        public List<Rule> Items { get; set; }
        public bool IsComplete { get; set; }

        public State()
        {
            Id = -1;
            Items = new List<Rule>();
            IsComplete = false;
        }
    }
}
