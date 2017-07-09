using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Data;
using Newtonsoft.Json;

namespace AutoDependencyDetector.Logic
{
    /// <summary>
    /// Does the entire pipeline of operations
    /// </summary>
    public class ProcessPipeline
    {
        public ConsoleLogger Logger { get; private set; } = new ConsoleLogger();


        private Options _options;

        public void ExecutePipeline(Options options)
        {
            _options = options;

            var l = new ConsoleLogger();


            l.Info( "Starting dependency detection" );

            l.Info( "Analyzing {0}{1}", options.InputDirectory, options.RecurseInput?" and children" : "" );
            _verifyInputDir(options.InputDirectory);


            var config = _readConfig( options.Config );
            var dependencies = _gatherDependencies( options.InputDirectory, options.RecurseInput , config);


        }

        private static Config _readConfig( string optionsConfig )
        {
            if ( File.Exists( optionsConfig ) == false )
            {
                // Create default one
                var c = Config.CreateDefaultConfig();
                File.WriteAllText( optionsConfig, JsonConvert.SerializeObject( c ,Formatting.Indented) );
            }

            return JsonConvert.DeserializeObject< Config >( File.ReadAllText( optionsConfig ));
        }

        private static List<string> _gatherDependencies( string dir, bool recurse , Config config)
        {
            var allFiles = Directory.GetFiles( dir, "*", recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );

            // Filter data by extension
            var filtered = allFiles.Where( f => config.AnalyzedExtensions.Contains( Path.GetExtension( f ) ) );

            foreach ( var file in filtered )
            {
                
                //var supportedExtensions = new HashSet<string>{".exe", ".dll", };
            }

            return null;
        }

        private static void _verifyInputDir(string dir)
        {
            if ( Directory.Exists( dir ) == false )
            {
                throw new DirectoryNotFoundException(dir);
            }
            
        }
    }
}
