using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc
{
    public class Parser
    {
        private Dictionary<string, string> LRTable = new Dictionary<string, string>();
        private Stack<string> StatesStack = new Stack<string>();
        private Stack<string> PrincipalStack = new Stack<string>();
        private List<List<string>> GrammarRules = new List<List<string>>();
        public bool ValidateExpression(string exp)
        {
            try
            {
                return true;
                /*
                 * Aqui el Parser toma los Tokens del Scanner uno por uno
                 * 
                 * Scanner = new Scanner(exp);
                 * CurrentToken = Scanner.GetToken();
                 * 
                 * Con base al estado actual de la pila y token obtenido hace las derivaciones
                */

               
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetAction(string state, string tokenValue)
        {
            string key = $"{state},{tokenValue}";
            return LRTable[key];
        }

        public void CreateLog()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            int posBinDirectory = CurrentDirectory.IndexOf("bin",0);
            string RelativeDirectory = CurrentDirectory.Substring(0, posBinDirectory);

            string path = $"{RelativeDirectory}\\log.txt";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine($"Log: {DateTime.Now}");

                //Print header
                sw.WriteLine("".PadRight(265, '-'));
                sw.WriteLine("STATES".PadRight(120, ' ') + "|" + "STACK".PadRight(120, ' ') + "|" + "ACTION".PadRight(10, ' '));
                sw.WriteLine("".PadRight(265, '-'));
            }
        }

        public Parser()
        {
            //Initiate stacks
            PrincipalStack.Push("#");
            StatesStack.Push("0");


            /*  Rules of principal grammar
            
                0.E' -> S'
                1.S' -> S ; \n S'
                2.S' -> S ;	
                3.S->non_t : rule
                4.S->non_t : rule | D
                5.D->rule | D
                6.D->rule
                7.rule->rule rule
                8.rule->terminal
                9.rule->non_t
            */
            GrammarRules.Add(new List<string>() { "E'", "S'" });
            GrammarRules.Add(new List<string>() { "S'", "S", ";", "\n", "S'" });
            GrammarRules.Add(new List<string>() { "S'", "S", ";"});
            GrammarRules.Add(new List<string>() { "S", "non_t", ":", "rule" });
            GrammarRules.Add(new List<string>() { "S", "non_t", ":", "rule", "|", "D" });
            GrammarRules.Add(new List<string>() { "D", "rule", "|", "D" });
            GrammarRules.Add(new List<string>() { "D", "rule"});
            GrammarRules.Add(new List<string>() { "rule", "rule", "rule" });
            GrammarRules.Add(new List<string>() { "rule", "terminal" });
            GrammarRules.Add(new List<string>() { "rule", "non_t" });

            /*
                Actions in LR table
             */

            //state 0
            LRTable.Add("0,non_t", "S3");
            LRTable.Add("0,S'", "1");
            LRTable.Add("0,S", "2");

            //state 1
            LRTable.Add("0,$", "ACCEPTED");

            //....

        }
    }
}
