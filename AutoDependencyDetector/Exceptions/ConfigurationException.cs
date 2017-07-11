using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException( string message, Exception innerException = null ) : base( message, innerException )
        {
        }
    }
}
