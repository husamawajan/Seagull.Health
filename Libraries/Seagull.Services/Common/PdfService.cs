// RTL Support provided by Credo inc (www.credo.co.il  ||   info@credo.co.il)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Seagull.Core;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Html;
using Seagull.Services.Configuration;
using Seagull.Services.Directory;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Media;
using Seagull.Services.Stores;

namespace Seagull.Services.Common
{
    /// <summary>
    /// PDF service
    /// </summary>
    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingContext;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;

        private readonly CurrencySettings _currencySettings;
        private readonly PdfSettings _pdfSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        public PdfService(ILocalizationService localizationService, 
            ILanguageService languageService,
            IWorkContext workContext,
            IDateTimeHelper dateTimeHelper,
            ICurrencyService currencyService, 
            IPictureService pictureService,
            IStoreService storeService,
            IStoreContext storeContext,
            ISettingService settingContext,
            IAddressAttributeFormatter addressAttributeFormatter,
            CurrencySettings currencySettings,
            PdfSettings pdfSettings,
            AddressSettings addressSettings)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._settingContext = settingContext;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._currencySettings = currencySettings;
            this._pdfSettings = pdfSettings;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get font
        /// </summary>
        /// <returns>Font</returns>
        protected virtual Font GetFont()
        {
            //SeagullCommerce supports unicode characters
            //SeagullCommerce uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            return GetFont(_pdfSettings.FontFileName);
        }
        /// <summary>
        /// Get font
        /// </summary>
        /// <param name="fontFileName">Font file name</param>
        /// <returns>Font</returns>
        protected virtual Font GetFont(string fontFileName)
        {
            if (fontFileName == null)
                throw new ArgumentNullException("fontFileName");

            string fontPath = Path.Combine(CommonHelper.MapPath("~/App_Data/Pdf/"), fontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }

        /// <summary>
        /// Get font direction
        /// </summary>
        /// <param name="lang">Language</param>
        /// <returns>Font direction</returns>
        protected virtual int GetDirection(Language lang)
        {
            return lang.Rtl ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
        }

        /// <summary>
        /// Get element alignment
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="isOpposite">Is opposite?</param>
        /// <returns>Element alignment</returns>
        protected virtual int GetAlignment(Language lang, bool isOpposite = false)
        {
            //if we need the element to be opposite, like logo etc`.
            if (!isOpposite)
                return lang.Rtl ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;
            
            return lang.Rtl ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
        }

        #endregion

        #region Methods


        #endregion
    }
}