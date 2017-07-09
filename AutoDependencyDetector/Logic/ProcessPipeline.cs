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
        private readonly DependencyDetector _detector;
        public ILogger Logger { get; private set; }


        private Options _options;

        private Config _config;

        public ProcessPipeline( ILogger logger, DependencyDetector detector )
        {
            _detector = detector;
            Logger = logger;
        }

        public void ExecutePipeline( Options options)
        {
            
            _options = options;

           
            Logger.Info( "Starting dependency detection" );

            Logger.Info( "Analyzing {0}{1}", options.InputDirectory, options.RecurseInput?" and children" : "" );

            _verifyDir(options.InputDirectory, "Input directory");
            _verifyDir( options.DependencyDirectory, "Dependency directory" );


            // --------- Obtain configuration
            _config = _readConfig( options.Config );

            // --------- Get all directories
            var directories = _forEveryDirectory( options.InputDirectory, options.RecurseInput );
            
            // --------- Read all dependencies
            foreach ( var directory in directories )
            {
                _handleDependenciesForDirectory( directory );
            }
            


        }

        private void _handleDependenciesForDirectory( string directory )
        {
            Logger.Info( "Analyzing dependencies in {0}", directory );

            var inputs = _gatherInputFiles( directory );
            Logger.Info( "Found {0} input files", inputs.Count );
            foreach ( var input in inputs )
            {
                _handleDependenciesForFile( input );
            }
        }

        private void _handleDependenciesForFile( string file )
        {
            Logger.Info( "Looking for missing dependencies for {0}", file );

            var bitness = new BitnessDetector().BitnessOf( file );

            var missingDependencies = _detector.GetMissingDependencies( file, bitness );

            Logger.Info( "Found {0} missing dependencies. Resolving", missingDependencies.Count );


            var dl = new DependencyLocator( _options.DependencyDirectory );

            var locatedDependencies = dl.LocateDependencies( missingDependencies );

            var destinationDirectory = Path.GetDirectoryName( file );

            foreach ( var dependency in locatedDependencies )
            {
                Logger.Info( "Found dependency {0}", dependency.Key );

                var depSourcePath = dependency.Value;

                File.Copy( depSourcePath, Path.Combine( destinationDirectory, Path.GetFileName( depSourcePath ) ) );
            }

        }

        /// <summary>
        /// Abstracting directories. Dependencies on each directory must be resolved by it's own since they must be copied
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="recurse"></param>
        private IEnumerable<string> _forEveryDirectory( string directory, bool recurse )
        {

            yield return directory;

            if ( recurse )
            {
                // Get also all other directories
                foreach ( var s in Directory.GetDirectories( directory, "*",SearchOption.AllDirectories ) )
                {
                    yield return s;
                }
            }

        }

        private Config _readConfig( string optionsConfig )
        {
            if ( File.Exists( optionsConfig ) == false )
            {
                // Create default one
                var c = Config.CreateDefaultConfig();
                File.WriteAllText( optionsConfig, JsonConvert.SerializeObject( c ,Formatting.Indented) );
            }

            return JsonConvert.DeserializeObject< Config >( File.ReadAllText( optionsConfig ));
        }

        private List<string> _gatherInputFiles( string dir)
        {
            var allFiles = Directory.GetFiles( dir, "*",  SearchOption.TopDirectoryOnly );

            // Filter data by extension
            var filtered = allFiles.Where( f => _config.AnalyzedExtensions.Contains( Path.GetExtension( f ) ) );

            return filtered.ToList();
            
        }

        private static void _verifyDir(string dir, string name)
        {
            if ( Directory.Exists( dir ) == false )
            {
                throw new DirectoryNotFoundException($"Directory for {name} not found. Value was {dir}");
            }
            
        }
    }
}
