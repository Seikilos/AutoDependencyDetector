using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Exceptions;

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
            // Fail if key is duplicated
            var allFiles = new Dictionary<string, string>();

            foreach ( var file in Directory.GetFiles( DependencyRootDirectory, "*", SearchOption.AllDirectories ) )
            {
                var key = _normalizeDependencyName( Path.GetFileName( file ) );
                if ( allFiles.ContainsKey( key ) )
                {
                    throw new DependencyLocatorException($"Dependency with the name {file} (handle: {key}) already added. Modules are looked up by their handle therefore it must not be duplicated"  );
                }
                allFiles[ key ] = file;
            }

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
