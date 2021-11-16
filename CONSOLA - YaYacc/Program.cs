using PROYECTO___YaYacc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PROYECTO___YaYacc.YaYacc;


namespace CONSOLA___YaYacc
{
    class Program
    {
        static void Main(string[] args)
        {
            //Entrega 3
            //Con la entidad gramatica validada hacer un LALR que valide el ingreso de palabras en consola
            Console.WriteLine("YAYACC");


            Rule r0 = new Rule() { Id = "S'", Elements = new List<string>() { "S" } };
            Rule r1 = new Rule() { Id = "S", Elements = new List<string>() { "S", "+", "T" } };
            Rule r2 = new Rule() { Id = "S", Elements = new List<string>() { "T" } };
            Rule r3 = new Rule() { Id = "T", Elements = new List<string>() { "T", "*", "F" } };
            Rule r4 = new Rule() { Id = "T", Elements = new List<string>() { "F" } };
            Rule r5 = new Rule() { Id = "F", Elements = new List<string>() { "(", "S", ")" } };
            Rule r6 = new Rule() { Id = "F", Elements = new List<string>() { "num" } };



            Grammar g = new Grammar() { InitialRule = r0, DictRules = new List<Rule>() { r1, r2, r3, r4, r5, r6 } };
            g.Terminals = new List<string>() { "+", "*", "(", ")", "num" };
            g.NonTerminals = new List<string>() { "S'", "S", "T", "F" };

            /* 
            S' -> S
            S -> S + T
            S -> T
            T -> T * F
            T -> F
            F -> ( S )
            F -> num      
             */

            LALR p = new LALR(g);
            p.GenerateTable();

            Console.ReadLine();
        }
    }
}
