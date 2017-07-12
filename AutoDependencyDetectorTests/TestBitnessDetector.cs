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
    public class TestBitnessDetector : TestCommon
    {
        [Test]
        public void Test_Detector_is_able_to_detect_64_bit()
        {
            var file64Bit = GetExtractedFiles( "x64", "*.exe" ).First(); // Will fail if nothing is extracted

            var b = new BitnessDetector();
            Assert.That( b.BitnessOf(file64Bit), Is.EqualTo( BitnessType.x64 ) );
        }

        [Test]
        public void Test_Detector_is_able_to_detect_32_bit()
        {
            var file = GetExtractedFiles("x86", "*.exe" ).First(); // Will fail if nothing is extracted

            var b = new BitnessDetector();
            Assert.That( b.BitnessOf(file), Is.EqualTo( BitnessType.x86 ) );
        }
    }
}
