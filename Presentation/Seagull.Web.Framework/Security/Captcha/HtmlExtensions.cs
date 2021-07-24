using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Seagull.Core.Infrastructure;

namespace Seagull.Web.Framework.Security.Captcha
{
    public static class HtmlExtensions
    {
        public static string GenerateCaptcha(this HtmlHelper helper)
        {
            var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
            var htmlWriter = new HtmlTextWriter(new StringWriter());

            var captchaControl = new GRecaptchaControl(captchaSettings.ReCaptchaVersion)
            {
                Theme = captchaSettings.ReCaptchaTheme,
                Id = "recaptcha",
                PublicKey = captchaSettings.ReCaptchaPublicKey,
                Language = captchaSettings.ReCaptchaLanguage
            };
            captchaControl.RenderControl(htmlWriter);

            return htmlWriter.InnerWriter.ToString();
        }
    }
}
