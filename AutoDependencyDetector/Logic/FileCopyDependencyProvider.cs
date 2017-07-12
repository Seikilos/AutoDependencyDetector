using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Interfaces;

namespace AutoDependencyDetector.Logic
{
    /// <summary>
    /// Simple dependency provider performing a file copy to destination
    /// </summary>
    public class FileCopyDependencyProvider : IDependencyProvider
    {
        public void ProvideDependency( string dependendcyFullPath, string destinationFilename )
        {
             File.Copy( dependendcyFullPath, destinationFilename );
        }
    }
}
