namespace AutoDependencyDetector.Interfaces
{
    public interface IDependencyProvider
    {
        void ProvideDependency( string dependendcyFullPath, string destinationDirectory );
    }
}