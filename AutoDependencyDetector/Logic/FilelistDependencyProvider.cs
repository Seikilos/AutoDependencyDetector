using System;
using System.IO;
using AutoDependencyDetector.Interfaces;

namespace AutoDependencyDetector.Logic
{
    /// <summary>
    /// Simple variant always directly writing file to list. No caching since this would require implementing IDisposable pattern
    /// </summary>
    public class FilelistDependencyProvider : IDependencyProvider
    {
        private readonly string _filelistPath;

        private readonly string _rootDependencyDirectory;

        
        public FilelistDependencyProvider( string filelistPath, string rootDependencyDirectory )
        {
            _filelistPath = filelistPath;
            _rootDependencyDirectory = rootDependencyDirectory;

            if ( _rootDependencyDirectory.EndsWith( @"\" ) == false )
            {
                // Append the directory delimiter for correct substring path
                _rootDependencyDirectory += @"\";
            }

            if ( File.Exists( _filelistPath ) )
            {
                File.Delete( _filelistPath );
            }

            // Write initial header
            File.WriteAllText( _filelistPath, "#"+_rootDependencyDirectory );

        }
        public void ProvideDependency( string dependendcyFullPath, string destinationDirectory )
        {
            var subPath = dependendcyFullPath.Replace( _rootDependencyDirectory, "" );
           
            File.AppendAllText( _filelistPath, $"{Environment.NewLine}{subPath}" );
        }
    }
}