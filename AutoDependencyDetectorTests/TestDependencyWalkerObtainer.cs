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
    public class TestDependencyWalkerObtainer : TestCommon
    {
        [Test]
        public async Task Test_DependencyWalkerObtainer_fetches_x64_and_x86()
        {
            var path = Path.Combine( TestData, "new_dir" );

            var d = new DependencyWalkerObtainer(path);

            await d.DownloadFiles();

            Assert.That( Directory.GetFiles( path, "*.exe", SearchOption.AllDirectories ), Has.Exactly( 2 ).Items );
        }

    }
}
