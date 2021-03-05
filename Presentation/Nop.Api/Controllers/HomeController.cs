using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Media;
using System;

namespace Nop.Api.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IDownloadService _downloadService;
        private readonly IStaticCacheManager _staticCacheManager;

        public HomeController(IDownloadService downloadService, IStaticCacheManager staticCacheManager)
        {
            _downloadService = downloadService;
            _staticCacheManager = staticCacheManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/ClearCache")]
        public virtual IActionResult ClearCache(string password = "")
        {
            if (password != "13243546")
                return Content("False");

            _staticCacheManager.Clear();

            return Content("True");
        }

        [Route("GetFileUpload")]
        public virtual IActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point.
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }
    }
}