using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Data;
using AutoDependencyDetector.Exceptions;
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

        private HashSet< string > _listOfProcessedInputFiles;

        public ProcessPipeline( ILogger logger, DependencyDetector detector )
        {
            _detector = detector;
            Logger = logger;
        }

        public void ExecutePipeline( Options options, Config config)
        {

            _options = options;

            Logger.Info( "Analyzing {0}{1}", options.InputDirectory, options.RecurseInput ? " and children" : "" );

            _verifyDir( options.InputDirectory, "Input directory" );
            _verifyDir( options.DependencyDirectory, "Dependency directory" );


            // List of all successfully processed files
            _listOfProcessedInputFiles = new HashSet< string >();


            // --------- Obtain configuration
            _config = config;

            // --------- For the configured amount of sweeps repeat the dependency lookup
            _repeatDependencyResovling( options );

        }

        private void _repeatDependencyResovling( Options options )
        {
            var locatedDependencies = 0;
            for ( var i = 0; i < _config.HowManyIterations; ++i )
            {
                Logger.Info( "Starting dependency search sweep {0}/{1}", i + 1, _config.HowManyIterations );

                // --------- Get all directories
                locatedDependencies = handleDependenciesForDirectories( options );
                if ( locatedDependencies == 0 )
                {
                    break;
                }

            }

            if ( locatedDependencies != 0 )
            {
                throw new ProcessPipelineException( $"After {_config.HowManyIterations} there might still be missing dependencies. Increase the HowManyIterations to ensure all dependencies are located" );
            }
        }

        private int handleDependenciesForDirectories( Options options )
        {
            var directories = _forEveryDirectory( options.InputDirectory, options.RecurseInput );

            var locatedDependencies = 0;

            // --------- Read all dependencies
            foreach ( var directory in directories )
            {
                locatedDependencies +=_handleDependenciesForDirectory( directory );
            }

            return locatedDependencies;
        }

        private int _handleDependenciesForDirectory( string directory )
        {
            var locatedDependencies = 0;
            Logger.Info( "Analyzing dependencies in {0}", directory );

            var inputs = _gatherInputFiles( directory );

            Logger.Info( "Found {0} input files", inputs.Count );
            foreach ( var input in inputs )
            {
                locatedDependencies += _handleDependenciesForFile( input );
            }

            return locatedDependencies;
        }

        private int _handleDependenciesForFile( string file )
        {
            Logger.Info( "Looking for missing dependencies for {0}", file );

            var bitness = new BitnessDetector().BitnessOf( file );

            var missingDependencies = _detector.GetMissingDependencies( file, bitness );

            if ( missingDependencies.Count == 0 )
            {
                Logger.Info( "No missing dependencies found" );
                return 0;
            }

            Logger.Info( "Found {0} missing dependencies. Resolving", missingDependencies.Count );


            var dl = new DependencyLocator( _options.DependencyDirectory );

            var locatedDependencies = dl.LocateDependencies( missingDependencies );

            if ( missingDependencies.Count != locatedDependencies.Count )
            {
                throw new ProcessPipelineException( $"Not all missing dependencies for {file} could be found: Missing ({missingDependencies.Count}): {string.Join(", ", missingDependencies )}, Found ({locatedDependencies.Count}): {string.Join( ", ",locatedDependencies )}" );
            }

            // All dependencies have been located, add it to finished files
            _listOfProcessedInputFiles.Add( file );

            var destinationDirectory = Path.GetDirectoryName( file );

            foreach ( var dependency in locatedDependencies )
            {
                Logger.Info( "Found dependency {0}", dependency.Key );

                var depSourcePath = dependency.Value;

                File.Copy( depSourcePath, Path.Combine( destinationDirectory, Path.GetFileName( depSourcePath ) ) );
            }

            return locatedDependencies.Count;

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


        private List<string> _gatherInputFiles( string dir)
        {
            var allFiles = Directory.GetFiles( dir, "*",  SearchOption.TopDirectoryOnly );

            // Filter data by extension
            var filtered = allFiles
                .Where( f => _config.AnalyzedExtensions.Contains( Path.GetExtension( f ) ) ) // Must support extension
                .Where( f => _listOfProcessedInputFiles.Contains( f ) == false  ) // must not be already processed
                .ToList();

    
            return filtered;
            
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
