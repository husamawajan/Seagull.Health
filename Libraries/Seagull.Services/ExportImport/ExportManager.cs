using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Messages;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.ExportImport.Help;
using Seagull.Services.Media;
using Seagull.Services.Messages;
using Seagull.Services.Seo;
using Seagull.Services.Stores;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Seagull.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IPictureService _pictureService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserAttributeFormatter _userAttributeFormatter;
        #endregion

        #region Ctor

        public ExportManager(
            IUserService userService,
            IPictureService pictureService,
            IStoreService storeService,
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            IUserAttributeFormatter userAttributeFormatter)
        {
            this._userService = userService;
            this._pictureService = pictureService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._userAttributeFormatter = userAttributeFormatter;
        }

        #endregion

        #region Utilities


        protected virtual void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }

        /// <summary>
        /// Returns the path to the image file by ID
        /// </summary>
        /// <param name="pictureId">Picture ID</param>
        /// <returns>Path to the image file</returns>
        protected virtual string GetPictures(int pictureId)
        {
            var picture = _pictureService.GetPictureById(pictureId);
            return _pictureService.GetThumbLocalPath(picture);
        }

        private bool IgnoreExportCategoryProperty()
        {
            return !_workContext.CurrentUser.GetAttribute<bool>("category-advanced-mode");
        }

        private bool IgnoreExportManufacturerProperty()
        {
            return !_workContext.CurrentUser.GetAttribute<bool>("manufacturer-advanced-mode");
        }

        /// <summary>
        /// Export objects to XLSX
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="properties">Class access to the object through its properties</param>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns></returns>
        protected virtual byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                    fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
                    
                    //create Headers and format them 
                    var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore));
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        private string GetCustomUserAttributes(User user)
        {
            var selectedUserAttributes = user.GetAttribute<string>(SystemUserAttributeNames.CustomUserAttributes, _genericAttributeService);
            return _userAttributeFormatter.FormatAttributes(selectedUserAttributes, ";");
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        public virtual string ExportCategoriesToXml()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Categories");
            xmlWriter.WriteAttributeString("Version", SeagullVersion.CurrentVersion);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }



        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="users">Users</param>
        public virtual byte[] ExportUsersToXlsx(IList<User> users)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<User>("UserId", p => p.Id),
                new PropertyByName<User>("UserGuid", p => p.UserGuid),
                new PropertyByName<User>("Email", p => p.Email),
                new PropertyByName<User>("Username", p => p.Username),
                new PropertyByName<User>("Password", p => _userService.GetCurrentPassword(p.Id).Return(password => password.Password, null)),
                new PropertyByName<User>("PasswordFormatId", p => _userService.GetCurrentPassword(p.Id).Return(password => password.PasswordFormatId, 0)),
                new PropertyByName<User>("PasswordSalt", p => _userService.GetCurrentPassword(p.Id).Return(password => password.PasswordSalt, null)),
                new PropertyByName<User>("Active", p => p.Active),
                //new PropertyByName<User>("IsGuest", p => p.IsGuest()),
                new PropertyByName<User>("IsRegistered", p => p.IsRegistered()),
                new PropertyByName<User>("IsAdministrator", p => p.IsAdmin()),
                //new PropertyByName<User>("IsForumModerator", p => p.IsForumModerator()),
                new PropertyByName<User>("CreatedOnUtc", p => p.CreatedOnUtc),
                //attributes
                new PropertyByName<User>("FirstName", p => p.GetAttribute<string>(SystemUserAttributeNames.FirstName)),
                new PropertyByName<User>("LastName", p => p.GetAttribute<string>(SystemUserAttributeNames.LastName)),
                new PropertyByName<User>("Gender", p => p.GetAttribute<string>(SystemUserAttributeNames.Gender)),
                new PropertyByName<User>("StreetAddress", p => p.GetAttribute<string>(SystemUserAttributeNames.StreetAddress)),
                new PropertyByName<User>("StreetAddress2", p => p.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2)),
                new PropertyByName<User>("ZipPostalCode", p => p.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode)),
                new PropertyByName<User>("City", p => p.GetAttribute<string>(SystemUserAttributeNames.City)),
                new PropertyByName<User>("CountryId", p => p.GetAttribute<int>(SystemUserAttributeNames.CountryId)),
                new PropertyByName<User>("StateProvinceId", p => p.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId)),
                new PropertyByName<User>("Phone", p => p.GetAttribute<string>(SystemUserAttributeNames.Phone)),
                new PropertyByName<User>("Fax", p => p.GetAttribute<string>(SystemUserAttributeNames.Fax)),
                new PropertyByName<User>("TimeZoneId", p => p.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId)),
                new PropertyByName<User>("AvatarPictureId", p => p.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId)),
                new PropertyByName<User>("ForumPostCount", p => p.GetAttribute<int>(SystemUserAttributeNames.ForumPostCount)),
                new PropertyByName<User>("Signature", p => p.GetAttribute<string>(SystemUserAttributeNames.Signature)),
                new PropertyByName<User>("CustomUserAttributes",  GetCustomUserAttributes)
            };

            return ExportToXlsx(properties, users);
        }

        /// <summary>
        /// Export user list to xml
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportUsersToXml(IList<User> users)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Users");
            xmlWriter.WriteAttributeString("Version", SeagullVersion.CurrentVersion);

            foreach (var user in users)
            {
                xmlWriter.WriteStartElement("User");
                xmlWriter.WriteElementString("UserId", null, user.Id.ToString());
                xmlWriter.WriteElementString("UserGuid", null, user.UserGuid.ToString());
                xmlWriter.WriteElementString("Email", null, user.Email);
                xmlWriter.WriteElementString("Username", null, user.Username);

                var userPassword = _userService.GetCurrentPassword(user.Id);
                xmlWriter.WriteElementString("Password", null, userPassword.Return(password => password.Password, null));
                xmlWriter.WriteElementString("PasswordFormatId", null, userPassword.Return(password => password.PasswordFormatId, 0).ToString());
                xmlWriter.WriteElementString("PasswordSalt", null, userPassword.Return(password => password.PasswordSalt, null));

                xmlWriter.WriteElementString("Active", null, user.Active.ToString());

                //xmlWriter.WriteElementString("IsGuest", null, user.IsGuest().ToString());
                xmlWriter.WriteElementString("IsRegistered", null, user.IsRegistered().ToString());
                xmlWriter.WriteElementString("IsAdministrator", null, user.IsAdmin().ToString());
                //xmlWriter.WriteElementString("IsForumModerator", null, user.IsForumModerator().ToString());
                xmlWriter.WriteElementString("CreatedOnUtc", null, user.CreatedOnUtc.ToString());

                xmlWriter.WriteElementString("FirstName", null, user.GetAttribute<string>(SystemUserAttributeNames.FirstName));
                xmlWriter.WriteElementString("LastName", null, user.GetAttribute<string>(SystemUserAttributeNames.LastName));
                xmlWriter.WriteElementString("Gender", null, user.GetAttribute<string>(SystemUserAttributeNames.Gender));

                xmlWriter.WriteElementString("CountryId", null, user.GetAttribute<int>(SystemUserAttributeNames.CountryId).ToString());
                xmlWriter.WriteElementString("StreetAddress", null, user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress));
                xmlWriter.WriteElementString("StreetAddress2", null, user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2));
                xmlWriter.WriteElementString("ZipPostalCode", null, user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode));
                xmlWriter.WriteElementString("City", null, user.GetAttribute<string>(SystemUserAttributeNames.City));
                xmlWriter.WriteElementString("StateProvinceId", null, user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId).ToString());
                xmlWriter.WriteElementString("Phone", null, user.GetAttribute<string>(SystemUserAttributeNames.Phone));
                xmlWriter.WriteElementString("Fax", null, user.GetAttribute<string>(SystemUserAttributeNames.Fax));
                xmlWriter.WriteElementString("TimeZoneId", null, user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId));

                xmlWriter.WriteElementString("AvatarPictureId", null, user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId).ToString());
                xmlWriter.WriteElementString("ForumPostCount", null, user.GetAttribute<int>(SystemUserAttributeNames.ForumPostCount).ToString());
                xmlWriter.WriteElementString("Signature", null, user.GetAttribute<string>(SystemUserAttributeNames.Signature));

                var selectedUserAttributesString = user.GetAttribute<string>(SystemUserAttributeNames.CustomUserAttributes, _genericAttributeService);

                if (!string.IsNullOrEmpty(selectedUserAttributesString))
                {
                    var selectedUserAttributes = new StringReader(selectedUserAttributesString);
                    var selectedUserAttributesXmlReader = XmlReader.Create(selectedUserAttributes);
                    xmlWriter.WriteNode(selectedUserAttributesXmlReader, false);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        
        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException("states");

            const string separator = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
                sb.Append(separator);
                sb.Append(state.Name);
                sb.Append(separator);
                sb.Append(state.Abbreviation);
                sb.Append(separator);
                sb.Append(state.Published);
                sb.Append(separator);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine); //new line
            }
            return sb.ToString();
        }

        #endregion
    }
}
