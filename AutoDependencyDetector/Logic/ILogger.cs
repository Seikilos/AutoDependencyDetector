namespace AutoDependencyDetector.Logic
{
    public interface ILogger
    {
        void Info( string msg, params object[] args );

        void Warn( string msg, params object[] args );
    }
}