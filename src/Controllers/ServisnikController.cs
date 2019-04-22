using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MobApps.Models;
using MobApps.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobApps.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "Downloader")]
    public class ServisnikController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly Dictionary<string, string> _contentTypes;
        private readonly ILogger<ServisnikController> _logger;

        public ServisnikController(IFileService fileService, IOptions<Dictionary<string, string>> contentTypes, ILogger<ServisnikController> logger)
        {
            _fileService = fileService;
            _contentTypes = contentTypes.Value;
            _logger = logger;
        }

        /// <summary>
        /// Получение последней версии приложения
        /// </summary>
        /// <remarks>
        /// Если описание отсутствует, то в поле releaseNotes вернется пустой список
        /// Запрос доступен только по токену. У юзера должен быть scope mobapps.download.
        /// </remarks>
        /// <returns></returns>
        [ProducesResponseType(typeof(VersionInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(VersionInfo), StatusCodes.Status404NotFound)]
        [HttpGet("{platform}/LatestVersion")]
        public async Task<VersionInfo> GetLatestVersion([FromRoute] string controller, string platform)
        {
            return await _fileService.GetLatestVersionInfoAsync(controller, platform);
        }


        /// <summary>
        /// Скачивание приложения
        /// </summary>
        /// <remarks>
        /// Если указанная версия не найдена, то вернется 404.
        /// Если версия не указана, то вернется самая новая.
        /// Запрос доступен только по токену. У юзера должен быть scope mobapps.download.
        /// </remarks>
        /// <param name="version">Запрашиваемая версия в формате х.х.х</param>
        /// <returns>Для Android возвращается файл в виде android.apk с типом application/vnd.android.package-archive </returns>
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status404NotFound)]
        [HttpGet("{platform}")]
        public async Task<IActionResult> GetAppAsync([FromRoute] string controller, string platform, string version)
        {
            var result = await _fileService.DownloadFileAsync(controller, platform, version);

            return result == null
                ? (IActionResult)NotFound("Specified version not found")
                : new FileStreamResult(result, _contentTypes.Single(kv => string.Equals(kv.Key, platform, StringComparison.InvariantCultureIgnoreCase)).Value)
                {
                    FileDownloadName = platform + ".apk" //Хардкод, т.к. других платформ не предвидится
                };
        }
    }
}