using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    public class Rule
    {
        public string Id { get; set; }
        public List<string>  Elements { get; set; }
        public bool IsAnalyzed { get; set; }
        //Agregar aqui la parte adicional que llevamos para el first

    }
}
