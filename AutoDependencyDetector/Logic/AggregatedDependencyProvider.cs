using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoDependencyDetector.Interfaces;

namespace AutoDependencyDetector.Logic
{
    /// <summary>
    /// Simple aggregator for multiple dependency provides to run.
    /// </summary>
    public class AggregatedDependencyProvider : IDependencyProvider
    {
        private List< IDependencyProvider > _providers;
        public AggregatedDependencyProvider(params IDependencyProvider[] providers)
        {
            _providers = providers.ToList();
        }

        public void ProvideDependency( string dependendcyFullPath, string destinationDirectory )
        {
            foreach ( var provider in _providers )
            {
                provider.ProvideDependency( dependendcyFullPath, destinationDirectory );
            }
        }
    }
}
