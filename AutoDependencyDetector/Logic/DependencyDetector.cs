using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Interfaces;

namespace AutoDependencyDetector.Logic
{
    public class DependencyDetector
    {
        public string DependencyWalkerRoot { get; }

        private const string _dependencyExecutable = "depends.exe";
        private const string _dependsFlags = "/c /oc:{0} {1}";

        /// <summary>
        /// Configures DependencyDetector
        /// </summary>
        /// <param name="dependencyWalkerRoot">Path to various architectures of dependency walker. Currently expects x64 and x86 subdirectories in it</param>
        public DependencyDetector(string dependencyWalkerRoot)
        {
            if ( Directory.Exists( dependencyWalkerRoot ) == false )
            {
                throw new DirectoryNotFoundException($"Path {dependencyWalkerRoot} does not exist");
            }

            var foundExecutables = Directory.GetFiles( dependencyWalkerRoot, _dependencyExecutable, SearchOption.AllDirectories ).Length;

            if ( foundExecutables == 0 )
            {
                throw new ArgumentException($"Path {dependencyWalkerRoot} and its subdirectories do not contain the dependency walker executable '{_dependencyExecutable}'");
            }

            DependencyWalkerRoot = dependencyWalkerRoot;


        }
        
        public IList< string > GetMissingDependencies( string filename, BitnessType bitness )
        {
            var dependencyWalkerPath = _getWalkerPath( bitness );

            var tempPath = Path.Combine( Path.GetTempPath(),"output.csv");

            var p = Process.Start( dependencyWalkerPath, string.Format( _dependsFlags, tempPath,filename ) );
            p.WaitForExit( 10000 );

           
            // Do not evaluate exit code, rely on output file

            if ( File.Exists( tempPath ) == false )
            {
                throw new InvalidOperationException($"Dependency walker exited with {p.ExitCode} without producing an output file. File called: {filename}");
            }


            var lines = File.ReadAllLines( tempPath ).Where( FilterMissingDependencies ).ToList();

            // Get only the names and remove quotes
            return lines.Select( l => l.Split( ',' )[ 1 ].Trim( '"' ) ).ToList();

        }

        /// <summary>
        /// Some dependencies like MS API are technically missing but not required anyway, they are removed from list.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool FilterMissingDependencies( string line )
        {
            // TODO: Make this configurable from config
            if ( line.Contains( "API-" ) )
            {
                return false;
            }
            if ( line.StartsWith( "?" ) )
            {
                return true;
            }
         
            return false;
        }

        private string _getWalkerPath( BitnessType bitness )
        {
            var path = Path.Combine( DependencyWalkerRoot, bitness.ToString(), _dependencyExecutable );
            if ( File.Exists( path ) == false )
            {
                throw new InvalidOperationException($"No dependency walker found at path {path}. Is this bitness supported?");
            }

            return path;
        }
    }
}
