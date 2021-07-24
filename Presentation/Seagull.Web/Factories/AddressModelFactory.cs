using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Infrastructure;
using Seagull.Services.Common;
using Seagull.Services.Directory;
using Seagull.Services.Localization;
using Seagull.Web.Models.Common;

namespace Seagull.Web.Factories
{
    /// <summary>
    /// Represents the address model factory
    /// </summary>
    public partial class AddressModelFactory : IAddressModelFactory
    {
        #region Fields

        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;

        #endregion

        #region Constructors

        public AddressModelFactory(IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IAddressAttributeFormatter addressAttributeFormatter)
        {
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._localizationService = localizationService;
            this._stateProvinceService = stateProvinceService;
            this._addressAttributeFormatter = addressAttributeFormatter;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address attributes
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of address entity</param>
        protected virtual void PrepareCustomAddressAttributes(AddressModel model,
            Address address, string overrideAttributesXml = "")
        {
            var attributes = _addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    IsRequired = attribute.IsRequired,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = !String.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    (address != null ? address.CustomAttributes : null);

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
        /// <param name="prePopulateWithUserFields">Whether to populate model properties with the user fields (used with the user entity)</param>
        /// <param name="user">User entity; required if prePopulateWithUserFields is true</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
        public virtual void PrepareAddressModel(AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            Func<IList<Country>> loadCountries = null,
            bool prePopulateWithUserFields = false,
            User user = null,
            string overrideAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null
                    ? address.Country.GetLocalized(x => x.Name)
                    : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = address.StateProvince != null
                    ? address.StateProvince.GetLocalized(x => x.Name)
                    : null;
                model.City = address.City;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
            }

            if (address == null && prePopulateWithUserFields)
            {
                if (user == null)
                    throw new Exception("User cannot be null when prepopulating an address");
                model.Email = user.Email;
                model.FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                model.LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
                model.Address1 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
                model.Address2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
                model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
                model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
                //ignore country and state for prepopulation. it can cause some issues when posting pack with errors, etc
                //model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                //model.StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
                model.PhoneNumber = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
                model.FaxNumber = user.GetAttribute<string>(SystemUserAttributeNames.Fax);
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    var languageId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    var states = _stateProvinceService
                        .GetStateProvincesByCountryId(model.CountryId.HasValue ? model.CountryId.Value : 0, languageId)
                        .ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;

            //user attribute services
            if (_addressAttributeService != null && _addressAttributeParser != null)
            {
                PrepareCustomAddressAttributes(model, address, overrideAttributesXml);
            }
            if (_addressAttributeFormatter != null && address != null)
            {
                model.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(address.CustomAttributes);
            }
        }

        #endregion
    }
}
