using System;
using System.Collections.Generic;
using System.IO;
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
         
        public Grammar()
        {

        }
        public Grammar(string path)
        {
            Terminals = new List<string>();
            NonTerminals = new List<string>();
            DictRules = new List<Rule>();
            Deserialize(path);
        }

        public void Deserialize(string path)
        {
             

            var ruleLines = File.ReadAllLines(path);
          
            foreach (var ruleLine in ruleLines)
            {
                var ruleParts = ruleLine.Split(':');

                if (!ruleParts[1].Contains("|"))
                {
                    var elements = ruleParts[1].Split(' ');
                    addElements(elements, ruleParts[0]);
                }
                else
                {
                    var tempRules = ruleParts[1].Split('|');
                    foreach (var rule in tempRules)
                    {
                        var elements = rule.Split(' ');
                        addElements(elements, ruleParts[0]);
                    }
                }
            }
        }

        private void addElements(string[] elements, string idRule)
        {
            Rule newRule = new Rule();

            Scanner scanner = new Scanner("");
            Token nextToken = new Token();

            newRule.Id = idRule.Trim();

            foreach (var element in elements)
            {
                if (element != "" && element != ";")
                {
                    nextToken = scanner.identifyToken(element);
                    newRule.Elements.Add(nextToken.Value);

                    if (nextToken.Tag == TokenType.T_TERMINAL && !Terminals.Contains(nextToken.Value))
                    {
                        Terminals.Add(nextToken.Value);
                    }
                    else if (nextToken.Tag == TokenType.T_NONT && !NonTerminals.Contains(nextToken.Value))
                    {
                        if (NonTerminals.Count > 0)
                        {
                            NonTerminals.Add(nextToken.Value);
                        }
                        else
                        {
                            NonTerminals.Add(newRule.Id);
                        }
                    }
                }

            }
            if (InitialRule == null)
            {
                InitialRule = newRule;
            }
            else
            {
                DictRules.Add(newRule);
            }
        }
    }
}
