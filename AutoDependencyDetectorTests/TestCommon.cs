using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AutoDependencyDetectorTests
{
    public class TestCommon
    {
        public readonly string TestDataRoot;
        public readonly string TestDataExtracted;
        public readonly string TestDataOwn;

        public TestCommon()
        {
            TestDataRoot = Path.Combine( Environment.CurrentDirectory, "TestData" );
            TestDataExtracted = Path.Combine(TestDataRoot, "extracted" );
            TestDataOwn = Path.Combine(TestDataRoot, "own" );

        }

        [OneTimeSetUp]
        public void SetupOnce()
        {
            TeardownOnce();

            Directory.CreateDirectory( TestDataRoot );
            Directory.CreateDirectory( TestDataExtracted );
            Directory.CreateDirectory( TestDataOwn );


            // Extract all test data
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach ( var name in names )
            {
                
                var dirs = name.Split( '.' ).Skip( 2 ); // Skip assembly name and root name of extracted data

                var destinationFileName = string.Join(".",dirs.Reverse().Take( 2 ).Reverse()); // Filename

                   
                dirs = dirs.Reverse().Skip( 2 ).Reverse().ToList(); // Not fastest but suitable, drops last two entries, assuming last two names are always file.extension
                var dir = string.Join( "/", dirs );
                var destDir = Path.Combine( TestDataExtracted, dir );
                Directory.CreateDirectory( destDir );

                
                var destFile = Path.Combine( destDir, destinationFileName); // Build the file name again
                ExtractResource(name, destFile );

            }

        }

        private void ExtractResource( string resName, string destFile )
        {
            using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName))
            {
                using(var file = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                } 
            }

            Assert.That( File.Exists( destFile ), Is.True );
        }


        [OneTimeTearDown]
        public void TeardownOnce()
        {
            DeleteDirectoryWithRetries( TestDataRoot );
        }

        public static void DeleteDirectoryWithRetries(string directory)
        {
            var tries = 10;

            while ( tries-- > 0 )
            {
                try
                {
                    if ( Directory.Exists( directory ) )
                    {
                        Directory.Delete( directory, true );
                    }

                    if ( Directory.Exists( directory ) == false )
                    {
                        return;
                    }
                }
                catch ( Exception )
                {
                    
                }
                Assert.Warn( $"Tried to delete {directory} but failed. Retrying {tries} times" );
                Thread.Sleep( 200 );
            }

            // Dir is there and retries have ended
            Assert.Fail($"Failed to delete {directory} multiple times.");
        }


        public List<string> GetExtractedFiles( string path, string pattern )
        {
            return GetFiles( Path.Combine( TestDataExtracted, path ), pattern );
        }

        public List< string > GetFiles( string path, string pattern )
        {
            return Directory.GetFiles( Path.Combine( TestDataRoot, path ), pattern ).ToList();
        }

        public string CreateDir( string name )
        {
            return Directory.CreateDirectory( Path.Combine( TestDataOwn, name ) ).FullName;
        }
    }
}
