using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public List< string > Excludes { get; } = new List< string >();
        public List<string> Includes { get; } = new List< string >();

        #region Cache

        private bool _listCreated;
        Dictionary< string, string > _allFiles;
        
        #endregion

        public DependencyLocator( string dependencyRoot )
        {
            if ( Directory.Exists( dependencyRoot ) == false )
            {
                throw new DirectoryNotFoundException( dependencyRoot );
            }

            DependencyRootDirectory = dependencyRoot;

            _listCreated = false;
        }

        public Dictionary< string, string > LocateDependencies( IList< string > dependencyNames )
        {
            // Fail if key is duplicated
            var allFiles = _setupFileList();
            var results = new Dictionary< string, string >();

            foreach ( var dep in dependencyNames )
            {
                var normalizedDepName = _normalizeDependencyName( dep );

                if ( allFiles.ContainsKey( normalizedDepName ) )
                {
                    // Add this path
                    results.Add( dep, allFiles[ normalizedDepName ] );
                }
            }

            if ( results.Count == 0 )
            {
                throw new DependencyLocatorException( $"Path {DependencyRootDirectory} does not contain any dependencies matching {string.Join( ", ", dependencyNames )}. Check whether filters are correct." );
            }

            return results;
        }

        private Dictionary< string, string > _setupFileList()
        {
            if(_listCreated)
            {
                return _allFiles;
            }

            // Fail if key is duplicated
            _allFiles = new Dictionary< string, string >();

            foreach ( var file in Directory.GetFiles( DependencyRootDirectory, "*", SearchOption.AllDirectories ) )
            {
                // Apply available filters, if string is marked as invalid, do not take it into consideration
                if ( validAfterFilters( file ) == false)
                {
                    continue;
                }


                var key = _normalizeDependencyName( Path.GetFileName( file ) );
                if ( _allFiles.ContainsKey( key ) )
                {
                    var existingModule = _allFiles[key];
                    throw new DependencyLocatorException( $"Dependency with the name '{file}' (handle: {key}) already added: '{existingModule}'. Modules are looked up by their handle therefore it must not be duplicated" );
                }
                _allFiles[ key ] = file;
            }

            return _allFiles;
        }

        private bool validAfterFilters( string file )
        {
            if ( Excludes.Any() == false && Includes.Any() == false )
            {
                // Default match when nothing has been added
                return true;
            }

            // If Exclude has been define, guard everything against it
            // If Include has been defined, do the same
            // If both then check exclude then include first

            if ( Excludes.Any() )
            {
                if ( _matchesFilter( Excludes, file ) )
                {
                    return false;
                }
            }

            if ( Includes.Any() )
            {
                if ( _matchesFilter( Includes, file ) )
                {
                    return true;
                }

                // Otherwise let filter fail
                return false;
                
            }

            return true;
        }

        private bool _matchesFilter( List< string > filter, string text )
        {
            foreach ( var f in filter )
            {
                if ( Regex.IsMatch(text, f, RegexOptions.IgnoreCase ) )
                {
                    return true;
                }
            }

            return false;
        }

        private static string _normalizeDependencyName( string name )
        {
            return name.ToLower();
        }
    }
}