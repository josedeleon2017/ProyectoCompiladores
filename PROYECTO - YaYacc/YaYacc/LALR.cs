using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    public class LALR
    {
        private Dictionary<string, string> LRTable = new Dictionary<string, string>();
        public List<State> _LALR = new List<State>();
        public Grammar Grammar { get; set; }
        public int NextId;
        public bool IsComplete;



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
                        if (pointPosition != currentState.Items[j].Elements.Count)
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
                                Rule ruleFormat = AddPoint(rulesToAdd[k]);
                                currentNewState.Items.Add(ruleFormat);
                            }
                        }
                    }


                    //Evaluar si el estado temporal general ya existe

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
        }


        public List<string> ElementsToRecognize(State s)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < s.Items.Count; i++)
            {
                Rule r = s.Items[i];
                int tempPointPosition = r.Elements.IndexOf(".");
                if(tempPointPosition != r.Elements.Count)
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
            S0.Items.Add(AddPoint(InitialRule));

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
                        Rule ruleFormat = AddPoint(rulesToAdd[j]);
                        S0.Items.Add(ruleFormat);
                    }
                }
            }
            _LALR.Add(S0);
        }

        public Rule GenerateState(Rule r)
        {
            return r;
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
        public List<Rule> SearchDeriviedRules(Rule r)
        {
            if (!r.IsAnalyzed)
            {
                int pointPosition = r.Elements.IndexOf(".");
                if (pointPosition == r.Elements.Count || r.Id == r.Elements[pointPosition + 1])
                {
                    return null;
                }
                else
                {
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
            r.Elements.Insert(0,".");
            return r;
        }
        public Rule MovePoint(Rule r)
        {
            Rule result = new Rule();
            result.Id = r.Id;
            result.Elements = r.Elements;
            result.IsAnalyzed = r.IsAnalyzed;


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

        public LALR(Grammar g)
        {
            Grammar = g;
            NextId = 0;
        }
    }
}
