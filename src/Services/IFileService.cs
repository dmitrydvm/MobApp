using Microsoft.AspNetCore.Mvc;
using MobApps.Models;
using System.IO;
using System.Threading.Tasks;

namespace MobApps.Services
{
    public interface IFileService
    {
        Task<VersionInfo> GetLatestVersionInfoAsync(string appName, string platform);

        Task<Stream> DownloadFileAsync(string appName, string platform, string version);
    }
}
