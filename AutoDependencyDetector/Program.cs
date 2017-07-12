using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoDependencyDetector.Data;
using AutoDependencyDetector.Interfaces;
using AutoDependencyDetector.Logic;
using CommandLine.Text;
using Newtonsoft.Json;

namespace AutoDependencyDetector
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {

                var options = new Options();
                var res =  CommandLine.Parser.Default.ParseArguments( args,options );

                if ( res == false )
                {
                    Environment.Exit( 1 );
                }

                var logger = new ConsoleLogger();

                logger.Info( "Called with {0}", string.Join( " ", args ) );

                logger.Info( "Starting dependency detection" );


                var pathOfDependsRoot = _getDependencyWalkerIfMissing( logger, options.ProxyUser, options.ProxyPassword );

                var dd = new DependencyDetector( pathOfDependsRoot );

                IDependencyProvider dependencyProvider = new FileCopyDependencyProvider();
                var p = new ProcessPipeline(logger,dd, dependencyProvider);

                var config = _readConfig( options.Config );

                // Fail fast: Check if configuration is available
                config.GetConfigurationSet( options.ConfigurationSetName ?? Config.DefaultSetName );

                p.ExecutePipeline( options, config );

                logger.Info( "Dependency detection finished" );

            }
            catch ( Exception e )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( e.ToString() );
                Environment.Exit( 1 );
            }
        }


        private static string _getDependencyWalkerIfMissing( ConsoleLogger logger, string proxyUser, string proxyPassword )
        {
            var current = Environment.CurrentDirectory;

            var dirName = "DependencyWalker";

            var finalDir = Path.Combine( current, dirName );

            if ( Directory.Exists( finalDir ) )
            {
                return finalDir; // all set
            }

            logger.Warn( "Detected missing dependency walker. Trying to obtain" );
            // Missing depends. Download
            var dwo = new DependencyWalkerObtainer( finalDir,proxyUser, proxyPassword );

            dwo.DownloadFiles().Wait();

            var fileCount = Directory.GetFiles( finalDir, "*.exe", SearchOption.AllDirectories ).Length;
            if ( fileCount != 2 )
            {
                throw new InvalidOperationException( $"Failed to obtain dependency walker. Expected two executables to find in {finalDir}, found {fileCount}" );
            }
            logger.Info( "Successfully obtain dependency walker" );

            return finalDir;
        }


        private static Config _readConfig( string optionsConfig )
        {
            if ( File.Exists( optionsConfig ) == false )
            {
                // Create default one
                var c = Config.CreateDefaultConfig();
                File.WriteAllText( optionsConfig, JsonConvert.SerializeObject( c, Formatting.Indented ) );
            }

            return JsonConvert.DeserializeObject<Config>( File.ReadAllText( optionsConfig ) );
        }
    }
}
