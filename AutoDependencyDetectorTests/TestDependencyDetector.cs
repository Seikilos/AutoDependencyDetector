using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Interfaces;
using AutoDependencyDetector.Logic;
using NUnit.Framework;

namespace AutoDependencyDetectorTests
{
    public class TestDependencyDetector : TestCommon
    {
        private DependencyDetector detector;
        
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            detector = new DependencyDetector( Path.Combine( TestData, "Depends" ) );
        }

        [Test]
        public void Test_DependencyDetector_returns_no_missing_dependency_if_all_dependencies_are_available()
        {
            var file = GetFiles( "x64", "Main.exe" ).First();

            
            var results = detector.GetMissingDependencies( file, BitnessType.x64 );
            Assert.That( results, Has.Count.EqualTo( 0 ) , "No dependency should be missing");
        }

        [Test]
        public void Test_DependencyDetector_returns_one_missing_dependency()
        {
            var file = GetFiles( "x64", "Main.exe" ).First();
            var path = CreateDir( "dummy" );
            File.Copy( file, Path.Combine( path, Path.GetFileName( file )) );

            file = GetFiles( "dummy", "*" ).First();

            
            
            var results = detector.GetMissingDependencies( file, BitnessType.x64 );
            Assert.That( results, Has.Count.AtLeast( 1 ) , "At least one dependency should be missing");

            Assert.That( results, Contains.Item("DependencyA.dll" ).IgnoreCase, "DependencyA should be part of missing dependencies" );
    

        }

        [Test]
        public void Test_DependencyDetector_throws_on_invalid_bitness()
        {

            Assert.That( () => detector.GetMissingDependencies( "file", BitnessType.Native ), Throws.Exception );
        }
    }
}
