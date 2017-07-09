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
        public void Test_Locator_throws_on_multiple_dependencies()
        {
           Assert.That( () => _locator.LocateDependencies( new [] { "DependencyA.dll" } ), Throws.TypeOf<DependencyLocatorException>() );
        }
    }
}
