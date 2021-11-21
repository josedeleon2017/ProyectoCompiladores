using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc.YaYacc
{
    [Serializable]
    public class Rule
    {
        public string Id { get; set; }
        public List<string>  Elements { get; set; }
        public bool IsAnalyzed { get; set; }
        public List<string> LookAHead { get; set; }

        public Rule()
        {
            Id = "";
            Elements = new List<string>();
            IsAnalyzed = false;
        }

        public Rule DeepClone(Rule obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (Rule)formatter.Deserialize(ms);
            }
        }

    }
}
