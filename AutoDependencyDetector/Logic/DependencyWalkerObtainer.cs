using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace AutoDependencyDetector.Logic
{
    public class DependencyWalkerObtainer
    {
        public string DirectoryToDownload { get; }
        public DependencyWalkerObtainer(string directoryToPlace)
        {
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

            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
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
