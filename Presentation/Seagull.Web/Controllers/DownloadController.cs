﻿using System;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Localization;
using Seagull.Services.Media;

namespace Seagull.Web.Controllers
{
    public partial class DownloadController : BasePublicController
    {
        private readonly IDownloadService _downloadService;

        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly UserSettings _userSettings;

        public DownloadController(IDownloadService downloadService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            UserSettings userSettings)
        {
            this._downloadService = downloadService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._userSettings = userSettings;
        }
        



        public virtual ActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

    }
}
