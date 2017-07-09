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
    public class TestProcessPipeline : TestCommon
    {
        private string DirectoryOfExeWithMissingDep;
        private ProcessPipeline _pipeline;
        private string _dependsRoot;

        private string _dependencyRoot;


        [SetUp]
        public void Setup()
        {
            _dependsRoot = Path.Combine( TestData, "Depends" );
            _dependencyRoot = Path.Combine( TestData, "x64" );

            // Create scenarios where executable is missing a dependency
            DirectoryOfExeWithMissingDep = Path.Combine( TestData, "only_exe" );
            CreateDir( DirectoryOfExeWithMissingDep );
            File.Copy( GetFiles( "x64", "*.exe" ).First(), Path.Combine(DirectoryOfExeWithMissingDep,"executable.exe") );


            var mock = NSubstitute.Substitute.For< ILogger >();

            var dd = new DependencyDetector( _dependsRoot );

            _pipeline = new ProcessPipeline(mock,dd);

            

        }

        [Test]
        public void Test_Dependency_is_detected_for_file()
        {
            var options = new Options { InputDirectory = DirectoryOfExeWithMissingDep, RecurseInput = false, Config = "config.json", DependencyDirectory = _dependencyRoot };

            _pipeline.ExecutePipeline( options );

            Assert.That( Directory.GetFiles( DirectoryOfExeWithMissingDep, "DependencyA.dll" ), Has.Exactly( 1 ).Items );
        }


        [Test]
        public void Test_Dependency_is_detected_for_file_in_nested_directory()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Test_Dependency_of_dependency_is_detected_in_second_sweep()
        {
            Assert.Fail("TODO");
        }
    }
}
