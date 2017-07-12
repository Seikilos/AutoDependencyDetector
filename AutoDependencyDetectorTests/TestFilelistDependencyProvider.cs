using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Logic;
using NUnit.Framework;

namespace AutoDependencyDetectorTests
{
    public class TestFilelistDependencyProvider : TestCommon
    {
        [Test]
        public void Test_FilelistDependencyProvider_writes_correct_path()
        {
            var dest = Path.Combine( TestDataOwn, "my_outfile.txt" );
            var f = new FilelistDependencyProvider(dest, TestDataExtracted);

            var filename = "foobar.txt";
            var depFile = Path.Combine( TestDataExtracted, filename );
            f.ProvideDependency( depFile, "somewhere" );

            var lines = File.ReadAllLines( dest );

            Assert.That( lines, Contains.Item( filename ) );

        }
    }
}
