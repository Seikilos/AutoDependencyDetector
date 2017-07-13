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

        [Option('d', "dependencies", HelpText = "Directory of dependencies", Required = true)]
        public string DependencyDirectory { get; set; }

        [Option('s', "configsetname", DefaultValue = Data.Config.DefaultSetName, HelpText = "Name of configuration set to use. Allows to handle multiple configurations in one file")]
        public string ConfigurationSetName { get; set; }

        [Option('u', "user", HelpText = "For initially obtaining dependency walker behind a proxy")]
        public string ProxyUser { get; set; }

        [Option('p', "password", HelpText = "For initially obtaining dependency walker behind a proxy")]
        public string ProxyPassword { get; set; }

        [Option('f', "filelist", HelpText = "If set, dependencies additionally written to a file instead. Must be a valid filename. This will still copy files, which is necessary since there might be nested dependencies.")]
        public string CreateFileList { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            const int firstLine = -25;

            sb.AppendLine( $"{"Input:",firstLine} {InputDirectory}" );
            sb.AppendLine( $"{"Recurse into sub dirs:",firstLine} {RecurseInput}" );
            sb.AppendLine( $"{"DependencyDirectory:",firstLine} {DependencyDirectory}" );
            sb.AppendLine( $"{"Configuration:",firstLine} {Config??"not set"}" );
            sb.AppendLine( $"{"Configuration Set Name:",firstLine} {ConfigurationSetName}" );
            sb.AppendLine( $"{"Write file list:",firstLine} {CreateFileList??"no"}" );
            
            return sb.ToString().Trim();
        }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));;
        }
    }
}
