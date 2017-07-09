using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Logic
{
    /// <summary>
    /// Provides means of obtaining dependencies. Designed to be configurable to allow fine-grained tuning of dependency location behaviours.
    /// </summary>
    public class DependencyLocator
    {
        public string DependencyRootDirectory { get; }
        public DependencyLocator(string dependencyRoot)
        {
            if ( Directory.Exists( dependencyRoot ) == false )
            {
                throw new DirectoryNotFoundException(dependencyRoot);
            }

            DependencyRootDirectory = dependencyRoot;

        }

        public Dictionary<string, string> LocateDependencies( IList<string> dependencyNames )
        {
            // Will fail if key is used twice
            var allFiles = Directory.GetFiles( DependencyRootDirectory, "*", SearchOption.AllDirectories ).ToDictionary( t => _normalizeDependencyName(Path.GetFileName( t )), t => t );

            var results = new Dictionary<string, string>();

            foreach ( var dep in dependencyNames )
            {
                var normalizedDepName = _normalizeDependencyName( dep );

                if ( allFiles.ContainsKey( normalizedDepName ) )
                {
                    // Add this path
                    results.Add( dep, allFiles[normalizedDepName] );
                }
            }

            return results;
        }

        private static string _normalizeDependencyName( string name )
        {
            return name.ToLower();
        }
    }
}
