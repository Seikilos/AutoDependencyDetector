using AutoDependencyDetector.Logic;

namespace AutoDependencyDetector.Interfaces
{
    public interface IBitnessDetector
    {
        /// <summary>
        /// Taken from https://stackoverflow.com/a/885481/2416394
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        BitnessType BitnessOf( string filename );
    }
}