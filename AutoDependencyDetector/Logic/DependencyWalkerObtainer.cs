using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Logic
{
    public class DependencyWalkerObtainer
    {
        private readonly string _user;
        private readonly string _password;

        public string DirectoryToDownload { get; }
        public DependencyWalkerObtainer(string directoryToPlace, string user = null, string password = null)
        {
            _user = user;
            _password = password;
            if ( Directory.Exists( directoryToPlace ) == false )
            {
                Directory.CreateDirectory( directoryToPlace );
            }

            DirectoryToDownload = directoryToPlace;


        }

        public async Task DownloadFiles()
        {
            await _downloadFile( "http://www.dependencywalker.com/depends22_x64.zip", Path.Combine( DirectoryToDownload, "x64" ) );
            await _downloadFile( "http://www.dependencywalker.com/depends22_x86.zip", Path.Combine( DirectoryToDownload, "x86" ) );
        }

        private async Task _downloadFile( string url, string destination )
        {
            // Compatibility temp file location since temp directory is not available on ci server due to permissions
            Directory.CreateDirectory( destination );
            var tempFile = Path.Combine( destination, Guid.NewGuid() + ".tmp" );

            using ( WebClient webClient = new WebClient() )
            {

                webClient.Proxy = WebRequest.GetSystemWebProxy();

                if ( string.IsNullOrWhiteSpace( _user ) == false && string.IsNullOrWhiteSpace( _password ) == false )
                {
                    // Some proxies may require authentication which is not provided by the credential manager (because it is not the logged in user credentials)
                    webClient.Proxy.Credentials = new NetworkCredential( _user, _password );
                }
             

                await webClient.DownloadFileTaskAsync(new Uri(url), tempFile);
            }
            _extract( tempFile, destination );

            File.Delete( tempFile );
        }

        private void _extract( string tempFile, string destination )
        {
            ZipFile.ExtractToDirectory( tempFile, destination );
        }
    }
}
