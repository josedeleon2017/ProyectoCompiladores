using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    public class LALR
    {
        //Generacion del parser
        private Dictionary<string, string> LRTable = new Dictionary<string, string>();
        public List<State> _LALR = new List<State>();
        public Grammar Grammar { get; set; }
        public int NextId;
        public bool IsComplete;
        public int DuplicateStateId;
        private Dictionary<string, List<string>> FirstTable = new Dictionary<string, List<string>>();

        //Validacion de expresion
        private Stack<string> StatesStack = new Stack<string>();
        private Stack<string> PrincipalStack = new Stack<string>();
        private List<List<string>> GrammarRules = new List<List<string>>();
        string Log;
        bool Result;

        public bool ValidateExpression(Queue<string> Tokens)
        {
            try
            {
                RestartConfiguration();
                string CurrentToken = Tokens.Dequeue();
                bool endValidation = false;
                while (!endValidation)
                {
                    string action = GetAction(StatesStack.Peek(), CurrentToken);
                    if (action == "-1") endValidation = true;
                    if (action[0] == 'S')
                    {
                        StatesStack.Push(action.Substring(1));
                        PrincipalStack.Push(CurrentToken);

                        //Obtiene el nuevo token
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

        public void RestartConfiguration()
        {
            PrincipalStack = new Stack<string>();
            StatesStack = new Stack<string>();

            PrincipalStack.Push("#");
            StatesStack.Push("0");
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
            int posBinDirectory = CurrentDirectory.IndexOf("bin", 0);
            string RelativeDirectory = CurrentDirectory.Substring(0, posBinDirectory);

            string path = $"{RelativeDirectory}\\Logs\\ExpressionLog.txt";
            using (StreamWriter sw = File.CreateText(path))
            {
                //Print header
                sw.WriteLine("**** PROYECTO YAYACC ****");
                sw.WriteLine("- Jocelyn de León");
                sw.WriteLine("- Kevin Romero");
                sw.WriteLine("- José De León");
                if (Result)
                {
                    sw.WriteLine("\nEXPRESIÓN VÁLIDA");
                }
                else
                {
                    sw.WriteLine("\nEXPRESIÓN INVÁLIDA");
                }
                sw.WriteLine($"{DateTime.Now}\n");


                //Print table
                sw.WriteLine("".PadRight(265, '-'));
                sw.WriteLine("STATES".PadRight(100, ' ') + "|" + "STACK".PadRight(155, ' ') + "|" + "ACTION".PadRight(8, ' '));
                sw.WriteLine("".PadRight(265, '-'));
                sw.WriteLine(Log);
            }
        }

        public void GenerateTable()
        {
            GenerateInitialState();

            //while que valida estado por estado empenzando de 0 pa arriba
            while (ExistIncompleteStates())
            {
                //Obtiene el primer estado a completar
                int currentStatePosition = GetCurrentState();
                State currentState = _LALR[currentStatePosition];

                //Obtiene los elementos a reconocer en el estado
                var elements = ElementsToRecognize(currentState);

                for (int i = 0; i < elements.Count; i++)
                {
                    //Crea un nuevo estado temporal con los elementos reconocidos
                    State currentNewState = new State();
                    currentNewState.Id = GetId();

                    //Agregar al nuevo estado las reglas con el punto movido
                    for (int j = 0; j < currentState.Items.Count; j++)
                    {
                        string ruleNext = "";
                        int pointPosition = currentState.Items[j].Elements.IndexOf(".");
                        if (pointPosition != currentState.Items[j].Elements.Count - 1)
                        {
                            ruleNext = currentState.Items[j].Elements[pointPosition + 1];
                        }
                        if (currentState.Items[j].Elements.Contains(elements[i]) && ruleNext == elements[i])
                        {
                            Rule ruleToMove = currentState.Items[j];
                            Rule copyRuleToMove = ruleToMove.DeepClone(ruleToMove);
                            Rule r = MovePoint(copyRuleToMove);
                            currentNewState.Items.Add(r);
                        }
                    }

                    //Expandir las reglas del nuevo estado si es necesario
                    //Valida si el punto genera mas reglas
                    for (int j = 0; j < currentNewState.Items.Count; j++)
                    {
                        List<Rule> rulesToAdd = SearchDeriviedRules(currentNewState.Items[j]);
                        currentNewState.Items[j].IsAnalyzed = true;
                        if (rulesToAdd != null)
                        {
                            //Normaliza todas las nuevas reglas a agregar
                            for (int k = 0; k < rulesToAdd.Count; k++)
                            {
                                Rule ruleToAddCopy = rulesToAdd[k].DeepClone(rulesToAdd[k]);
                                Rule ruleFormat = AddPoint(ruleToAddCopy);

                                //Sacar el LookAHead de las reglas generadas

                                currentNewState.Items.Add(ruleFormat);
                            }
                        }
                    }


                    //Evaluar si el estado temporal general ya existe
                    if (!ExistState(currentNewState))
                    {
                        _LALR.Add(currentNewState);
                    }
                    else
                    {
                        ReturnId();
                        //Busco el estado ya existente para no repertir su creacion
                        currentNewState = SearchState(DuplicateStateId);
                    }

                    //Si ya existe mando a traer el id de ese estado, su id sirve para redireccionar la accion

                    //Por cada elemento reconocido se agrega una accion

                    //Agrega los GOTO
                    if (Grammar.NonTerminals.Contains(elements[i]))
                    {
                        LRTable.Add($"{currentState.Id},{elements[i]}", Convert.ToString(currentNewState.Id));
                    }
                    //Agrega los SHIFT
                    else if (Grammar.Terminals.Contains(elements[i]))
                    {
                        LRTable.Add($"{currentState.Id},{elements[i]}", $"S{currentNewState.Id}");
                    }

                }
                _LALR[currentStatePosition].IsComplete = true;
            }

            //Calcular los reduces y ahi termina
            for (int i = 0; i < _LALR.Count; i++)
            {
                State currentState = _LALR[i];
                for (int j = 0; j < currentState.Items.Count; j++)
                {
                    Rule currentRule = currentState.Items[j];
                    if (currentRule.Elements[currentRule.Elements.Count - 1] == ".")
                    {
                        AddAsociateReduce(currentRule, currentState.Id);
                    }
                }
            }
        }

        public void AddAsociateReduce(Rule r, int stateNumber)
        {
            //Recorre todos los no terminales -> Despues cambiar por la list del LookAHead
            for (int i = 0; i < Grammar.Terminals.Count; i++)
            {
                string indexRule = GetIndexRule(r);
                if (!LRTable.ContainsKey($"{stateNumber},{Grammar.Terminals[i]}"))
                {
                    if (indexRule == "ACCEPTED" && Grammar.Terminals[i] == "$")
                    {
                        LRTable.Add($"{stateNumber},{Grammar.Terminals[i]}", $"{indexRule}");
                    }
                    else if (indexRule != "ACCEPTED")
                    {
                        LRTable.Add($"{stateNumber},{Grammar.Terminals[i]}", $"R{indexRule}");
                    }
                }
            }
        }

        public string GetIndexRule(Rule r)
        {
            //Evalua si es la regla inicial
            if (r.Elements.Count - 1 == Grammar.InitialRule.Elements.Count)
            {
                bool isEquals = true;
                for (int i = 0; i < r.Elements.Count - 1; i++)
                {
                    if (r.Elements[i] != Grammar.InitialRule.Elements[i])
                    {
                        isEquals = false;
                    }
                }
                if (isEquals)
                {
                    return "ACCEPTED";
                }
            }

            //Evalua si es alguna de los otras reglas
            for (int i = 0; i < Grammar.DictRules.Count; i++)
            {
                if (r.Elements.Count - 1 == Grammar.DictRules[i].Elements.Count)
                {
                    bool isEquals = true;
                    for (int j = 0; j < r.Elements.Count - 1; j++)
                    {
                        if (r.Elements[j] != Grammar.DictRules[i].Elements[j])
                        {
                            isEquals = false;
                        }
                    }
                    if (isEquals)
                    {
                        return Convert.ToString(i + 1);
                    }
                }
            }
            return "";
        }

        public bool ExistState(State s)
        {
            for (int i = 0; i < _LALR.Count; i++)
            {
                State currentState = _LALR[i];
                bool CompareS = CompareStates(s, currentState);
                if (CompareS) return true;
            }
            return false;
        }

        public bool CompareStates(State s1, State s2)
        {
            //Evaluo si ambos estados tiene  la misma cantidad de reglas
            if (s1.Items.Count == s2.Items.Count)
            {
                //Evalua si las reglas tienen la misma longitud
                int rulesQty = s1.Items.Count;
                for (int i = 0; i < rulesQty; i++)
                {
                    if (s1.Items[i].Elements.Count == s2.Items[i].Elements.Count)
                    {
                        int ruleLong = s1.Items[i].Elements.Count;
                        //Recorre todos los elementos de la misma regla
                        for (int j = 0; j < ruleLong; j++)
                        {
                            if (s1.Items[i].Elements[j] != s2.Items[i].Elements[j])
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Si alguna de las reglas no es del mismo tamaño, la regla no es la misma y por lo tanto tampoco el estado
                        return false;
                    }
                }
                //Aqui tomar el id del estado
                DuplicateStateId = s2.Id;
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<string> ElementsToRecognize(State s)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < s.Items.Count; i++)
            {
                Rule r = s.Items[i];
                int tempPointPosition = r.Elements.IndexOf(".");
                if (tempPointPosition != r.Elements.Count - 1)
                {
                    string elementToAdd = r.Elements[tempPointPosition + 1];
                    if (!result.Contains(elementToAdd))
                    {
                        result.Add(r.Elements[tempPointPosition + 1]);
                    }
                }
            }
            return result;
        }

        public void GenerateInitialState()
        {
            State S0 = new State();
            S0.Id = GetId();

            //Agrega la primera regla con punto al inicio
            Rule InitialRule = Grammar.InitialRule;
            Rule ruleToAdd = InitialRule.DeepClone(InitialRule);
            S0.Items.Add(AddPoint(ruleToAdd));

            //Sacar el LookAHead -> $

            //Valida si el punto genera mas reglas
            for (int i = 0; i < S0.Items.Count; i++)
            {
                List<Rule> rulesToAdd = SearchDeriviedRules(S0.Items[i]);
                S0.Items[i].IsAnalyzed = true;
                if (rulesToAdd != null)
                {
                    //Normaliza todas las nuevas reglas a agregar
                    for (int j = 0; j < rulesToAdd.Count; j++)
                    {
                        Rule ruleToAddCopy = rulesToAdd[j].DeepClone(rulesToAdd[j]);
                        Rule ruleFormat = AddPoint(ruleToAddCopy);
                        S0.Items.Add(ruleFormat);
                    }
                }
            }
            _LALR.Add(S0);
        }

        public int GetCurrentState()
        {
            for (int i = 0; i < _LALR.Count; i++)
            {
                if (!_LALR[i].IsComplete)
                {
                    _LALR[i].IsComplete = true;
                    return i;
                }
            }
            return -1;
        }

        public State SearchState(int id)
        {
            for (int i = 0; i < _LALR.Count; i++)
            {
                if (_LALR[i].Id == id) return _LALR[i];
            }
            return null;
        }
        public List<Rule> SearchDeriviedRules(Rule r)
        {
            if (!r.IsAnalyzed)
            {
                int pointPosition = r.Elements.IndexOf(".");
                if (pointPosition == r.Elements.Count - 1)
                {
                    return null;
                }
                else
                {
                    if (r.Id == r.Elements[pointPosition + 1]) return null;
                    List<Rule> result = new List<Rule>();
                    string ruleToExpand = r.Elements[pointPosition + 1];
                    for (int i = 0; i < Grammar.DictRules.Count; i++)
                    {
                        if (Grammar.DictRules[i].Id == ruleToExpand)
                        {
                            result.Add(Grammar.DictRules[i]);
                        }
                    }
                    return result;
                }
            }
            return null;
        }

        public bool ExistIncompleteStates()
        {
            for (int i = 0; i < _LALR.Count; i++)
            {
                if (!_LALR[i].IsComplete) return true;
            }
            return false;
        }
        public Rule AddPoint(Rule r)
        {
            r.Elements.Insert(0, ".");
            return r;
        }
        public Rule MovePoint(Rule r)
        {
            Rule result = new Rule();
            result.Id = r.Id;
            result.Elements = r.Elements;
            result.IsAnalyzed = false;


            int pointPosition = result.Elements.IndexOf(".");
            string pivot = result.Elements[pointPosition + 1];
            result.Elements[pointPosition] = pivot;
            result.Elements[pointPosition + 1] = ".";
            return result;
        }

        public int GetId()
        {
            int id = NextId;
            NextId++;
            return id;
        }

        public void ReturnId()
        {
            NextId--;
            return;
        }

        public LALR(Grammar g)
        {
            Grammar = g;
            Grammar.Terminals.Add("$");
            NextId = 0;

            SetGrammarRule();
            CreateTableFirst();
        }

        public void SetGrammarRule()
        {
            //Agrega en el formato la primera regla
            List<string> initialRule = new List<string>();
            initialRule.Add(Grammar.InitialRule.Id);
            for (int i = 0; i < Grammar.InitialRule.Elements.Count; i++)
            {
                initialRule.Add(Grammar.InitialRule.Elements[i]);
            }
            GrammarRules.Add(initialRule);
            //Agrega en el formato todas las demas reglas
            for (int i = 0; i < Grammar.DictRules.Count; i++)
            {
                List<string> currentRule = new List<string>();
                currentRule.Add(Grammar.DictRules[i].Id);
                for (int j = 0; j < Grammar.DictRules[i].Elements.Count; j++)
                {
                    currentRule.Add(Grammar.DictRules[i].Elements[j]);
                }
                GrammarRules.Add(currentRule);
            }
        }
        public void CreateTableFirst()
        {
            string id = Grammar.InitialRule.Id;
            First(Grammar.InitialRule.Elements[0]);
            var values = FirstTable[Grammar.InitialRule.Elements[0]];
            FirstTable.Add(id, values);
            foreach (var item in Grammar.DictRules)
            {
                First(item.Id);
            }
        }

        public void First(string ruleID)
        {
            List<Rule> temporalRule = new List<Rule>();
            foreach (var rule in Grammar.DictRules)
            {
                if (rule.Id == ruleID)
                {
                    temporalRule.Add(rule);
                }
            }
            foreach (var rule in temporalRule)
            {
                if (Grammar.Terminals.Contains(rule.Elements[0]))
                {
                    //ID, lista de strings
                    if (FirstTable.ContainsKey(ruleID))
                    {
                        var values = FirstTable[ruleID];
                        if (!values.Contains(rule.Elements[0]))
                        {
                            values.Add(rule.Elements[0]);
                        }
                        FirstTable[ruleID] = values;
                    }
                    else
                    {
                        List<string> Values = new List<string>();
                        if (!Values.Contains(rule.Elements[0]))
                        {
                            Values.Add(rule.Elements[0]);
                        }
                        FirstTable.Add(ruleID, Values);
                    }
                }
                else if (Grammar.NonTerminals.Contains(rule.Elements[0]))
                {
                    if (ruleID != rule.Elements[0])
                    {
                        First(rule.Elements[0]);
                        if (!FirstTable.ContainsKey(ruleID))
                        {
                            List<string> Values = new List<string>();
                            FirstTable.Add(ruleID, Values);
                        }
                        var values = FirstTable[ruleID];
                        foreach (var item in FirstTable[rule.Elements[0]])
                        {
                            if (!values.Contains(item))
                            {
                                values.Add(item);
                            }
                        }
                        FirstTable[ruleID] = values;
                    }
                }
            }
        }
        public bool LookaHead(Rule ParentRule, List<Rule> RulesDerived)
        {
            //S' → .S

            //     01null
            int pointPosition = ParentRule.Elements.IndexOf(".");
            //ponintposition = 0
            string derivated = ParentRule.Elements[pointPosition + 1];
            //derivated = 1
            if (Grammar.Terminals.Contains(derivated))
            {
                return false;
            }
            else if (Grammar.NonTerminals.Contains(derivated))
            {
                //evelauar la possion de derived + 1
                if ((ParentRule.Elements.IndexOf(derivated) + 1) == ParentRule.Elements.Count)
                {
                    //El siguiente es el final de la regla y propaga a los reglas derivadas todos sus lookahead
                    foreach (var Rules in RulesDerived)
                    {
                        foreach (var LookaHead in ParentRule.LookAHead)
                        {
                            Rules.LookAHead.Add(LookaHead);
                        }
                    }
                }
                else
                {
                    List<string> FirstNext = new List<string>();
                    string keyNext = ParentRule.Elements[ParentRule.Elements.IndexOf(derivated) + 1];
                    if (derivated == ParentRule.Id)
                    {
                        if (!isTerminal(keyNext))
                        {
                            FirstNext = FirstTable[keyNext];
                            foreach (var LookaHead in FirstNext)
                            {
                                if (ParentRule.LookAHead.Contains(LookaHead))
                                {
                                    ParentRule.LookAHead.Add(LookaHead);
                                }

                            }
                            foreach (var RuleDerived in RulesDerived)
                            {
                                foreach (var LookaHead in ParentRule.LookAHead)
                                {
                                    if (!RuleDerived.LookAHead.Contains(LookaHead))
                                    {
                                        RuleDerived.LookAHead.Add(LookaHead);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var RuleDerived in RulesDerived)
                            {
                                if (!RuleDerived.LookAHead.Contains(keyNext))
                                {
                                    RuleDerived.LookAHead.Add(keyNext);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isTerminal(keyNext))
                        {
                            foreach (var RuleDerived in RulesDerived)
                            {
                                if (!RuleDerived.LookAHead.Contains(keyNext))
                                {
                                    RuleDerived.LookAHead.Add(keyNext);
                                }
                            }
                        }
                        else
                        {
                            FirstNext = FirstTable[keyNext];
                            foreach (var RuleDerived in RulesDerived)
                            {
                                foreach (var LookaHead in FirstNext)
                                {
                                    if (!RuleDerived.LookAHead.Contains(keyNext))
                                    {
                                        RuleDerived.LookAHead.Add(LookaHead);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            List<string> newLookaHead = new List<string>();
            foreach (var item in RulesDerived)
            {
                pointPosition = item.Elements.IndexOf(".");
                if (pointPosition + 1 == item.Elements.Count())
                {
                    return true;
                }

                if (pointPosition + 2 < item.Elements.Count())
                {
                    string addLookaHead = item.Elements[pointPosition + 2];
                    if (isTerminal(addLookaHead))
                    {
                        newLookaHead.Add(addLookaHead);
                    }
                }
            }
            foreach (var ruleEdit in RulesDerived)
            {
                foreach (var newlook in newLookaHead)
                {
                    if (!ruleEdit.LookAHead.Contains(newlook))
                    {
                        ruleEdit.LookAHead.Add(newlook);
                    }
                }
            }

            return true;
        }
        private bool isTerminal(string value)
        {
            if (Grammar.Terminals.Contains(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<Rule> toAddLookaHead(State S1, Rule r)
        {
            List<Rule> result;
            int pointPosition = r.Elements.IndexOf(".");
            if (pointPosition == r.Elements.Count - 1)
            {
                return null;
            }
            else
            {
                //S' → .S'
                if (r.Id == r.Elements[pointPosition + 1])
                    return null;
                result = new List<Rule>();
                string ruleToExpand = r.Elements[pointPosition + 1];
                foreach (var item in S1.Items)
                {
                    if (item.Id == ruleToExpand)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
        }
    }
}