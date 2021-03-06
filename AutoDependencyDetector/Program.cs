﻿using System;
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
                var logger = new ConsoleLogger();

                
                var options = new Options();
                var res =  CommandLine.Parser.Default.ParseArguments( args,options );

                logger.Info( "Starting dependency detection" );

                _dumpOptions( logger, options );

                // See #4, even a start without params should provide the minimum configuration setup, which is config and dependency walker
                var config = _readConfig( options.Config );

                var pathOfDependsRoot = _getDependencyWalkerIfMissing( logger, options.ProxyUser, options.ProxyPassword );


                if ( res == false )
                {
                    Environment.Exit( 1 );
                }
              
                var dd = new DependencyDetector( pathOfDependsRoot );

                IDependencyProvider dependencyProvider = _getDependencyProvider( options );
                
             
                var p = new ProcessPipeline(logger,dd, dependencyProvider);

        ;

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

        private static void _dumpOptions( ILogger logger, Options options )
        {
            var divider = new string('*',40 );
            logger.Info( divider );
            logger.Info( options.ToString() );
            logger.Info( divider );
            
        }

        private static IDependencyProvider _getDependencyProvider( Options options )
        {
            if ( string.IsNullOrEmpty( options.CreateFileList ) == false )
            {
                return new AggregatedDependencyProvider( new FilelistDependencyProvider( options.CreateFileList, options.DependencyDirectory), new FileCopyDependencyProvider() );
            }

            // Default fallback
            return new FileCopyDependencyProvider();
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
            var dwo = new DependencyWalkerObtainer( finalDir, proxyUser, proxyPassword );

            try
            {

                dwo.DownloadFiles().Wait();
                var fileCount = Directory.GetFiles( finalDir, "*.exe", SearchOption.AllDirectories ).Length;
                if ( fileCount != 2 )
                {
                    // Remove directory because this would prevent a download next time
                    throw new InvalidOperationException( $"Failed to obtain dependency walker. Expected two executables to find in {finalDir}, found {fileCount}" );
                }
            }
            catch ( Exception )
            {
                Directory.Delete( finalDir, true );
                throw;
            }


            logger.Info( "Successfully obtained dependency walker" );

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
