using System;
using System.Text;
using System.Web;
using Seagull.Core;
using Seagull.Core.Html;
using Seagull.Services.Localization;

namespace Seagull.Services.Users
{
    /// <summary>
    /// User attributes formatter
    /// </summary>
    public partial class UserAttributeFormatter : IUserAttributeFormatter
    {
        #region Fields

        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public UserAttributeFormatter(IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IWorkContext workContext)
        {
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(string attributesXml, string serapator = "<br />", bool htmlEncode = true)
        {
            var result = new StringBuilder();

            var attributes = _userAttributeParser.ParseUserAttributes(attributesXml);
            for (int i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = _userAttributeParser.ParseValues(attributesXml, attribute.Id);
                for (int j = 0; j < valuesStr.Count; j++)
                {
                    string valueStr = valuesStr[j];
                    string formattedAttribute = "";
           

                    if (!String.IsNullOrEmpty(formattedAttribute))
                    {
                        if (i != 0 || j != 0)
                            result.Append(serapator);
                        result.Append(formattedAttribute);
                    }
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
