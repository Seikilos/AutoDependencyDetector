using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Exceptions;

namespace AutoDependencyDetector.Data
{
    public class Config
    {

        public const string DefaultSetName = "Default";
        public HashSet<string> AnalyzedExtensions { get; set; } = new HashSet< string >();

        public uint HowManyIterations { get; set; }

        public class ConfigurationSet
        {
           
            public List<string> ExcludeRegexList { get; set; } = new List< string >();
            public List<string> IncludeRegexList { get; set; } = new List< string >();

            public List<string> AdditionalFiles { get; set; } = new List< string >();
 
        }

        public Dictionary<string,ConfigurationSet> ConfigurationSets { get; set; } = new Dictionary< string, ConfigurationSet >();
      
        public static Config CreateDefaultConfig()
        {
            return new Config
            {
                AnalyzedExtensions = new HashSet< string > { ".exe", ".dll" },
                HowManyIterations = 3,
                ConfigurationSets = new Dictionary< string, ConfigurationSet >{{DefaultSetName,new ConfigurationSet()}},
            };
        }

        public ConfigurationSet GetConfigurationSet( string name )
        {
            if ( ConfigurationSets.ContainsKey( name ) == false )
            {
                throw new ConfigurationException( $"Could not find '{name}' configuration set name in config." );
            }

            return ConfigurationSets[ name ];
        }

    }
}
