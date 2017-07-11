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

        public List<string> ExcludeRegexList { get; set; } = new List< string >();
        public List<string> IncludeRegexList { get; set; } = new List< string >();

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
