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
        string Log;
        bool Result;
        

        //Cola temporal para los tokens, borrar despues de implementar el Scanner
        Queue<string> Tokens = new Queue<string>();
        string CurrentToken;



        public bool ValidateExpression()
        {
            try
            {
                //Scanner = new Scanner(exp);

                //---------CAMBIAR DESUPES POR GetToken()
                CurrentToken = Tokens.Dequeue();
                bool endValidation = false;
                while (!endValidation)
                {
                    string action = GetAction(StatesStack.Peek(), CurrentToken);
                    if (action == "-1") endValidation = true;
                    if (action[0] == 'S')
                    {                     
                        StatesStack.Push(action.Substring(1));
                        PrincipalStack.Push(CurrentToken);

                        //---------CAMBIAR DESUPES POR GetToken()
                        CurrentToken = Tokens.Dequeue();

                        SaveStep(action);
                    }
                    else if (action[0] == 'R')
                    {
                        SaveStep(action);
                        List<string> rule = GetRule(action.Substring(1));
                        for (int i = rule.Count - 1; i > 0; i--)
                        {
                            if (rule[i] == PrincipalStack.Peek())
                            {
                                PrincipalStack.Pop();
                                StatesStack.Pop();
                            }
                        }
                        PrincipalStack.Push(Convert.ToString(rule[0]));
                        StatesStack.Push(GetAction(StatesStack.Peek(), PrincipalStack.Peek()));
                    }
                    else if (action == "ACCEPTED")
                    {
                        SaveStep(action);
                        Result = true;
                        CreateLog();
                        return true;
                    }
                }
                Result = false;
                CreateLog();
                return false;
            }
            catch (Exception)
            {
                Result = false;
                CreateLog();
                return false;
            }
        }

        public string GetAction(string state, string tokenValue)
        {
            string key = $"{state},{tokenValue}";
            if (!LRTable.ContainsKey(key)) return "-1";
            return LRTable[key];
        }

        public List<string> GetRule(string rule)
        {
            int pos = Convert.ToInt32(rule);
            return GrammarRules[pos];
        }

        private void SaveStep(string action)
        {
            string st_stack = "";
            string[] states = new string[StatesStack.Count];
            StatesStack.CopyTo(states, 0);
            for (int i = states.Length - 1; i >= 0; i--)
            {
                st_stack += states[i].PadRight(3, ' ');
            }

            string p_stack = "";
            string[] tokens = new string[PrincipalStack.Count];
            PrincipalStack.CopyTo(tokens, 0);
            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                p_stack += tokens[i].PadRight(12, ' ');
            }
            Log += st_stack.PadRight(100, ' ') + "|" + p_stack.PadRight(155, ' ') + "|" + action.PadRight(8, ' ') + "\n";
        }

        private void CreateLog()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            int posBinDirectory = CurrentDirectory.IndexOf("bin",0);
            string RelativeDirectory = CurrentDirectory.Substring(0, posBinDirectory);

            string path = $"{RelativeDirectory}\\log.txt";
            using (StreamWriter sw = File.CreateText(path))
            {
                //Print header
                sw.WriteLine("**** PROYECTO YAYACC ****");
                sw.WriteLine("- Jocelyn de León");
                sw.WriteLine("- Kevin Romero");
                sw.WriteLine("- José De León");
                if (Result)
                {
                    sw.WriteLine("\nGRAMÁTICA VÁLIDA");
                }
                else
                {
                    sw.WriteLine("\nGRAMÁTICA INVÁLIDA");
                }
                sw.WriteLine($"{DateTime.Now}\n");


                //Print table
                sw.WriteLine("".PadRight(265, '-'));
                sw.WriteLine("STATES".PadRight(100, ' ') + "|" + "STACK".PadRight(155, ' ') + "|" + "ACTION".PadRight(8, ' '));
                sw.WriteLine("".PadRight(265, '-'));
                sw.WriteLine(Log);
            }
        }

        public Parser(string regexp)
        {
            /*
             * Cola temporal para pruebas, la gramatica 1 deberia ser reconocida en este orden de tokens
             * 
             */
            /*Tokens.Enqueue("T_NONT");
            Tokens.Enqueue("T_SEPARATOR");
            Tokens.Enqueue("T_NONT");
            Tokens.Enqueue("T_ENDLINE");
            Tokens.Enqueue("T_NEWLINE");
            Tokens.Enqueue("T_NONT");
            Tokens.Enqueue("T_SEPARATOR");
            Tokens.Enqueue("T_TERMINAL"); 
            Tokens.Enqueue("T_NONT");
            Tokens.Enqueue("T_TERMINAL");
            Tokens.Enqueue("T_OR");
            Tokens.Enqueue("T_TERMINAL");
            Tokens.Enqueue("T_ENDLINE");
            Tokens.Enqueue("T_EOF");*/

            /* Initiate stacks */
            Scanner scanner = new Scanner(regexp);
            Token nextToken;

            do
            {
                nextToken = scanner.GetToken();
                Tokens.Enqueue(nextToken.Tag.ToString());
            } while (nextToken.Tag != TokenType.T_EOF);

            PrincipalStack.Push("#");
            StatesStack.Push("0");
            Log = "";


            /*  Rules of principal grammar
                0.E' -> S'
                1.S' -> S "T_ENDLINE" "T_NEWLINE" S'
                2.S' -> S "T_ENDLINE"	
                3.S->"T_NONT" "T_SEPARATOR" RULE
                4.S->"T_NONT" "T_SEPARATOR" RULE "T_OR" D
                5.D-> RULE "T_OR" D
                6.D-> RULE
                7.RULE-> RULE RULE
                8.RULE-> "T_TERMINAL"
                9.RULE-> "T_NONT"
            */
            GrammarRules.Add(new List<string>() { "E'", "S'" });
            GrammarRules.Add(new List<string>() { "S'", "S", "T_ENDLINE", "S'" });
            GrammarRules.Add(new List<string>() { "S'", "S", "T_ENDLINE"});
            GrammarRules.Add(new List<string>() { "S", "T_NONT", "T_SEPARATOR", "RULE" });
            GrammarRules.Add(new List<string>() { "S", "T_NONT", "T_SEPARATOR", "RULE", "T_OR", "D" });
            GrammarRules.Add(new List<string>() { "D", "RULE", "T_OR", "D" });
            GrammarRules.Add(new List<string>() { "D", "RULE"});
            GrammarRules.Add(new List<string>() { "RULE", "RULE", "RULE" });
            GrammarRules.Add(new List<string>() { "RULE", "T_TERMINAL" });
            GrammarRules.Add(new List<string>() { "RULE", "T_NONT" });

            /*
                Actions in LR table
             */

            //state 0
            LRTable.Add("0,T_NONT", "S3");
            LRTable.Add("0,S'", "1");
            LRTable.Add("0,S", "2");

            //state 1
            LRTable.Add("1,T_EOF", "ACCEPTED");

            //state 2
            LRTable.Add("2,T_ENDLINE", "S4");

            //state 3
            LRTable.Add("3,T_SEPARATOR", "S5");

            //state 4
            LRTable.Add("4,T_NONT", "S3");
            LRTable.Add("4,T_EOF", "R2");
            LRTable.Add("4,S'", "6");
            LRTable.Add("4,S", "2");

            //state 5
            LRTable.Add("5,T_NONT", "S9");
            LRTable.Add("5,T_TERMINAL", "S8");
            LRTable.Add("5,RULE", "7");

            //state 6
            LRTable.Add("6,T_EOF", "R1");


            //state 7
            LRTable.Add("7,T_ENDLINE", "R3");
            LRTable.Add("7,T_NONT", "S9");
            LRTable.Add("7,T_OR", "S10");
            LRTable.Add("7,T_TERMINAL", "S8");
            LRTable.Add("7,RULE", "11");

            //state 8
            LRTable.Add("8,T_ENDLINE", "R8");
            LRTable.Add("8,T_NONT", "R8");
            LRTable.Add("8,T_OR", "R8");
            LRTable.Add("8,T_TERMINAL", "R8");

            //state 9
            LRTable.Add("9,T_ENDLINE", "R9");
            LRTable.Add("9,T_NONT", "R9");
            LRTable.Add("9,T_OR", "R9");
            LRTable.Add("9,T_TERMINAL", "R9");

            //state 10
            LRTable.Add("10,T_NONT", "S9");
            LRTable.Add("10,T_TERMINAL", "S8");
            LRTable.Add("10,D", "12");
            LRTable.Add("10,RULE", "13");

            //state 11
            LRTable.Add("11,T_NONT", "S9");
            LRTable.Add("11,T_TERMINAL", "S8");
            LRTable.Add("11,RULE", "11");
            LRTable.Add("11,T_ENDLINE", "R7");
            LRTable.Add("11,T_OR", "R7");

            //state 12
            LRTable.Add("12,T_ENDLINE", "R4");


            //state 13
            LRTable.Add("13,T_ENDLINE", "R6");
            LRTable.Add("13,T_NONT", "S9");
            LRTable.Add("13,T_OR", "S14");
            LRTable.Add("13,T_TERMINAL", "S8");
            LRTable.Add("13,RULE", "11");


            //state 14
            LRTable.Add("14,T_NONT", "S9");
            LRTable.Add("14,T_TERMINAL", "S8");
            LRTable.Add("14,D", "15");
            LRTable.Add("14,RULE", "13");

            //state 15
            LRTable.Add("15,T_ENDLINE", "R5");
        }
    }
}
