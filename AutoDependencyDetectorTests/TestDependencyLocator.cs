using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Exceptions;
using AutoDependencyDetector.Logic;
using NUnit.Framework;

namespace AutoDependencyDetectorTests
{
    public class TestDependencyLocator :TestCommon
    {
        private DependencyLocator _locator;
        [SetUp]
        public void SetUp()
        {
            _locator = new DependencyLocator( TestData );
        }


        [Test]
        public void Test_Locator_finds_single_dependency()
        {
            _locator = new DependencyLocator( Path.Combine( TestData, "x64" ) );

            var result = _locator.LocateDependencies( new [] { "DependencyA.dll" } );

            Assert.That( result, Has.Exactly( 1 ).Items );

        }

        [Test]
        public void Test_Locator_throws_if_no_dependencies_found()
        {
            _locator = new DependencyLocator( Path.Combine( TestData, "x64" ) );

            Assert.That( () => _locator.LocateDependencies( new[] { "Does_not_Exist.dll" } ), Throws.TypeOf<DependencyLocatorException>() );


        }

        [Test]
        public void Test_Locator_throws_on_multiple_dependencies()
        {
           Assert.That( () => _locator.LocateDependencies( new [] { "DependencyA.dll" } ), Throws.TypeOf<DependencyLocatorException>() );
        }

        private string _createDummyStructure()
        {
            var dRoot = Path.Combine( TestData, "tree" );
            Directory.CreateDirectory( dRoot+"/A" );
            Directory.CreateDirectory( dRoot+"/B" );
            File.WriteAllText( dRoot+"/A/foo.dll", "" );
            File.WriteAllText( dRoot+"/B/foo.dll", "" );

            return dRoot;
        }

        [Test]
        public void Test_Locator_accept_filter_exclude_rules()
        {
            
            var path = _createDummyStructure();

            _locator = new DependencyLocator( path );
            _locator.Excludes.Add( @"\\A\\" );

            Assert.That( () => _locator.LocateDependencies( new[] { "foo.dll" }), Throws.Nothing );
        }


        
        [Test]
        public void Test_Locator_accept_filter_include_rules()
        {
            var path = _createDummyStructure();

            _locator = new DependencyLocator( path );
            _locator.Includes.Add( @"\\B\\" );

            Assert.That( () => _locator.LocateDependencies( new[] { "foo.dll" }), Throws.Nothing );
        }

        [Test]
        public void Test_Locator_accept_filter_exclude_and_include_regex_rules()
        {
            // Create a valid exclude and a valid include
            var path = _createDummyStructure();

            _locator = new DependencyLocator( path );
            _locator.Excludes.Add( @".*B\\" );
            _locator.Includes.Add( @".*" );

            Assert.That( () => _locator.LocateDependencies( new[] { "foo.dll" }), Throws.Nothing );
        }
    }
}
