using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace AutoDependencyDetector.Logic
{
    public class Options
    {

        [Option('i', "input", Required = true, HelpText = "Directory where binaries with missing dependencies should be searched for.")]
        public string InputDirectory { get; set; }

        [Option('c',"config", DefaultValue = "config.json", HelpText = "Configuration to read from. Will be automatically created if missing")]
        public string Config { get; set; }

        [Option('r', "recurse", DefaultValue = true, HelpText = "Flag determining whether also sub directories should be analyzed and resolved")]
        public bool RecurseInput { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
        
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));;
        }
    }
}
