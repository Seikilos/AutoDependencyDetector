﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Data;
using AutoDependencyDetector.Exceptions;
using AutoDependencyDetector.Logic;
using NUnit.Framework;

namespace AutoDependencyDetectorTests
{
    public class TestProcessPipeline : TestCommon
    {
        private string DirectoryOfExeWithMissingDep;

        private string DirectoryOfDepAMissingDepB;
        private ProcessPipeline _pipeline;
        private string _dependsRoot;

        private string _dependencyRoot;

        private Options _defaultOptions;

        private Config _defaultConfig;


        [SetUp]
        public void Setup()
        {
            _dependsRoot = Path.Combine( TestData, "Depends" );
            _dependencyRoot = Path.Combine( TestData, "x64" );

            _defaultConfig = Config.CreateDefaultConfig();
            _defaultConfig.HowManyIterations = 2; // Manually control sweeps, always at least 2 required

            // Create scenarios where executable is missing a dependency
            DirectoryOfExeWithMissingDep = Path.Combine( TestData, "only_exe" );


            _defaultOptions = new Options { InputDirectory = DirectoryOfExeWithMissingDep, RecurseInput = false, Config = "config.json", DependencyDirectory = _dependencyRoot };

            TearDown();

            // DepA
            {
                DirectoryOfDepAMissingDepB = Path.Combine( TestData, "depA" );
                CreateDir( DirectoryOfDepAMissingDepB );
                File.Copy( GetFiles( "x64", "*A.dll" ).First(), Path.Combine( DirectoryOfDepAMissingDepB, "dlla.dll" ) );
            }


            CreateDir( DirectoryOfExeWithMissingDep );

            var file = GetFiles( "x64", "*.exe" ).First();

            File.Copy( file, Path.Combine(DirectoryOfExeWithMissingDep,"executable.exe") );

            // Create nested executable
            {
                var nestedDir = Path.Combine( DirectoryOfExeWithMissingDep, "inner" );
                CreateDir( nestedDir );
                File.Copy( file, Path.Combine( nestedDir, "executable.exe" ) );
            }


            var mock = NSubstitute.Substitute.For< ILogger >();

            var dd = new DependencyDetector( _dependsRoot );

            _pipeline = new ProcessPipeline(mock,dd);
        }


        [TearDown]
        public void TearDown()
        {
            // Delete artifacts generated per run
            DeleteDirectoryWithRetries( DirectoryOfExeWithMissingDep );
            DeleteDirectoryWithRetries( DirectoryOfDepAMissingDepB );
        }

        [Test]
        public void Test_Dependency_is_detected_for_file()
        {
            // Use dllA since it has only one Dependency
            _defaultOptions.InputDirectory = DirectoryOfDepAMissingDepB;

            _pipeline.ExecutePipeline( _defaultOptions, _defaultConfig );

            Assert.That( Directory.GetFiles( DirectoryOfDepAMissingDepB, "DependencyB.dll", SearchOption.AllDirectories ), Has.Exactly( 1 ).Items );
        }


        [Test]
        public void Test_Dependency_is_detected_for_file_in_nested_directory()
        {

            _defaultOptions.RecurseInput = true;
            _defaultConfig.HowManyIterations = 3;

            _pipeline.ExecutePipeline( _defaultOptions, _defaultConfig );

            Assert.That( Directory.GetFiles( DirectoryOfExeWithMissingDep, "DependencyA.dll", SearchOption.AllDirectories ), Has.Exactly( 2 ).Items );
        }

        [Test]
        public void Test_Dependency_of_dependency_is_detected_in_second_sweep()
        {
            _defaultOptions.RecurseInput = true;
            _defaultConfig.HowManyIterations = 3;
            _pipeline.ExecutePipeline( _defaultOptions, _defaultConfig );

            Assert.That( Directory.GetFiles( DirectoryOfExeWithMissingDep, "DependencyB.dll", SearchOption.AllDirectories ), Has.Exactly( 2 ).Items );
        }

        [Test]
        public void Test_that_pipeline_throws_if_not_all_dependencies_could_be_resolved()
        {
            var emptyDir = CreateDir( "empty" );
            // Set dep dir so that it won't find any dependencies
            _defaultOptions.DependencyDirectory = emptyDir;

            // Probably Pipeline should wrap DependencyLocatorException into a PipelineException
            Assert.That( () => _pipeline.ExecutePipeline( _defaultOptions, _defaultConfig ), Throws.TypeOf<DependencyLocatorException>() );

        }



        // TODO: Configure filter via configuration
       
    }
}
