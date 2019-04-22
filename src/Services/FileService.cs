using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using MobApps.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobApps.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IFileProvider _fileProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(ILogger<FileService> logger, IFileProvider fileProvider, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _fileProvider = fileProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Stream> DownloadFileAsync(string appName, string platform, string version)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentNullException(nameof(appName));
            }
            if (string.IsNullOrWhiteSpace(platform))
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _logger.LogInformation($"User {_httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == JwtClaimTypes.Subject).Value} requested file {appName}/{platform}/{version}");

            var dirPath = await GetRelativeDirPathAsync(appName, platform, version);

            return _fileProvider.GetDirectoryContents(dirPath)
                .FirstOrDefault()
                ?.CreateReadStream();
        }


        public async Task<VersionInfo> GetLatestVersionInfoAsync(string appName, string platform)
        {
            _logger.LogInformation($"User {_httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == JwtClaimTypes.Subject).Value} requested latest version");

            var lastDir = await GetRelativeDirPathAsync(appName, platform, null);

            if (lastDir == null)
            {
                return null;
            }

            var baseUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}");

            return new VersionInfo
            {
                LatestVersion = Path.GetFileName(lastDir),
                ReleaseNotes = await GetReleaseNotesAsync(lastDir),
                Url = Uri.TryCreate(baseUri, $"{appName}/{platform}?version={Path.GetFileName(lastDir)}", out Uri uri) ? uri : null
            };
        }


        /// <summary>
        /// Собирает путь к папке по указанным параметрам
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="platform"></param>
        /// <param name="version"></param>
        /// <returns>
        /// Возвращает относительный путь к каталогу с приложением.
        /// Если указана версия, то ищет ее. Если не указана, возвращает путь к последней доступной.
        /// Если не находит ничего, то возвращает null
        /// </returns>
        private Task<string> GetRelativeDirPathAsync(string appName, string platform, string version)
        {
            var dirs = _fileProvider.GetDirectoryContents(Path.Combine("content", appName, platform))
                .OrderBy(d => d.Name)
                .Select(d => d.Name)
                .ToList();

            if (dirs.Count == 0)
            {
                _logger.LogError($"Specified {nameof(platform)} {platform} or {nameof(appName)} {appName} not exists");
            }

            var dirName = string.IsNullOrWhiteSpace(version)
                ? dirs.LastOrDefault()
                : dirs.Find(n => n.Equals(version, StringComparison.InvariantCultureIgnoreCase));

            var result = dirName == null
                ? null
                : Path.Combine("content", appName, platform, dirName);

            return Task.FromResult(result);
        }


        private async Task<string[]> GetReleaseNotesAsync(string dirPath)
        {
            var fullPathToFile = Path.Combine(dirPath, "ReleaseNotes.txt");

            return File.Exists(fullPathToFile)
                ? await File.ReadAllLinesAsync(fullPathToFile)
                : Array.Empty<string>();
        }
    }
}
