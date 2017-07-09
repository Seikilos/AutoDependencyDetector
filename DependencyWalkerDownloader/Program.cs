using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Logic;

namespace DependencyWalkerDownloader
{
    /// <summary>
    /// Helper to actually download dependency walker to external. Must be run separately in order to get dependency walker. This is machine independent
    /// and should also work on linux based build nodes
    /// </summary>
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                Execute( args );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
                Environment.Exit( 1 );
            }
        }

        static void Execute(string[] args)
        {
            if ( args.Length != 1 )
            {
                throw new ArgumentException("First argument must be directory for dependency walker");
            }

            var path = args[ 0 ];

            // Make sure this directory is empty
            foreach ( var directoryInfo in new DirectoryInfo(path).GetDirectories() )
            {
                directoryInfo.Delete(true);
            }
           

            Console.WriteLine( $"Downloading depends to {path}" );

            var d = new DependencyWalkerObtainer(path);

            d.DownloadFiles().GetAwaiter().GetResult();

            var exeFiles = Directory.GetFiles( path, "*.exe", SearchOption.AllDirectories ).Length;
            if ( exeFiles != 2 )
            {
                throw new InvalidOperationException($"Expected two executables, found {exeFiles}");
            }
        }
    }
}
