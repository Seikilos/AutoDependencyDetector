using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Data
{
    public class Config
    {
        public HashSet<string> AnalyzedExtensions { get; set; } = new HashSet< string >();

        public uint HowManyIterations { get; set; } 

        public static Config CreateDefaultConfig()
        {
            return new Config
            {
                AnalyzedExtensions = new HashSet< string > { ".exe", ".dll" },
                HowManyIterations = 3,
            };
        }
    }
}
