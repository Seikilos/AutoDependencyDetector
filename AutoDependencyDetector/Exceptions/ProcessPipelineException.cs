using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Exceptions
{
    public class ProcessPipelineException : Exception
    {
        public ProcessPipelineException( string message, Exception innerException = null ) : base( message, innerException )
        {
        }
    }
}
