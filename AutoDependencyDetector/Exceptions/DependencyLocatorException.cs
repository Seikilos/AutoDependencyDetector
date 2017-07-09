using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Exceptions
{
    public class DependencyLocatorException : Exception
    {
        public DependencyLocatorException( string message, Exception innerException = null ) : base( message, innerException )
        {
        }
    }
}
