using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Interfaces
{
    public enum BitnessType
    {
        Native = 0,
        // ReSharper disable once InconsistentNaming
        x86 = 0x014c,
        Itanium = 0x0200,
        // ReSharper disable once InconsistentNaming
        x64 = 0x8664
    }
}
