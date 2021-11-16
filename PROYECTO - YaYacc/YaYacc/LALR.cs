using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    public class LALR
    {
        public List<State> Lalr { get; set; }
        public Grammar Grammar { get; set; }
        public int NextId;
        public bool IsComplete;



        public void GenerateTable()
        {
            GenerateInitialState();

            //while que valida estado por estado empenzando de 0 pa arriba
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
                if (rulesToAdd == null)
                {
                    //no hace nada
                }
                else
                {
                    //Normaliza todas las nuevas reglas a agregar
                    for (int j = 0; j < rulesToAdd.Count; j++)
                    {
                        Rule ruleFormat = AddPoint(rulesToAdd[j]);
                        S0.Items.Add(ruleFormat);
                    }
                }
            }



            Lalr.Add(S0);
        }

        public int ContarPuntos(Rule r)
        {
            int count = 0;
            for (int i = 0; i < r.Elements.Count; i++)
            {
                if (r.Elements[i] == ".") count++;
            }
            return count;
        }
        public List<Rule> SearchDeriviedRules(Rule r)
        {
            if (!r.IsAnalyzed)
            {
                int pointPosition = r.Elements.IndexOf(".");
                if (pointPosition == r.Elements.Count)
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
            for (int i = 0; i < Lalr.Count; i++)
            {
                if (!Lalr[i].IsComplete) return false;
            }
            return true;
        }
        public Rule AddPoint(Rule r)
        {
            r.Elements.Insert(0,".");
            return r;
        }
        public Rule MovePoint(Rule r)
        {
            return r;
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
