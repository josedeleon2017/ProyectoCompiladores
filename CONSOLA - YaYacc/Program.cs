using PROYECTO___YaYacc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PROYECTO___YaYacc.YaYacc;
using Newtonsoft.Json;

namespace CONSOLA___YaYacc
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Entrega 3
            //Con la entidad gramatica validada hacer un LALR que valide el ingreso de palabras en consola
            Console.WriteLine("YAYACC");
            Console.ReadLine();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            int posBinDirectory = CurrentDirectory.IndexOf("CONSOLA - YaYacc", 0);
            string RelativeDirectory = CurrentDirectory.Substring(0, posBinDirectory);
            RelativeDirectory += "PROYECTO - YaYacc";
            string jsonPath = $"{RelativeDirectory}\\grammar.json";

            string jsonGrammar = File.ReadAllText(jsonPath);
            Grammar deserializedGrammar = JsonConvert.DeserializeObject<Grammar>(jsonGrammar);
            /* 
            S' -> S
            S -> S + T
            S -> T
            T -> T * F
            T -> F
            F -> ( S )
            F -> num      
             */

            LALR p = new LALR(deserializedGrammar);
            p.GenerateTable();

            Console.ReadLine();
        }
    }
}
