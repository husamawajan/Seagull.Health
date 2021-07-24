using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain;
using Seagull.Core.Domain.Cms;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Domain.Logging;
using Seagull.Core.Domain.Media;
using Seagull.Core.Domain.Messages;
using Seagull.Core.Domain.Security;
using Seagull.Core.Domain.Seo;
using Seagull.Core.Domain.Stores;
using Seagull.Core.Domain.Tasks;
using Seagull.Core.Domain.Topics;
using Seagull.Core.Infrastructure;
using Seagull.Services.Common;
using Seagull.Services.Configuration;
using Seagull.Services.Users;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Media;
using Seagull.Services.Seo;

namespace Seagull.Services.Installation
{
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<Store> storeRepository,
            IRepository<Language> languageRepository,
            IRepository<Currency> currencyRepository,
            IRepository<User> userRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<Topic> topicRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<TopicTemplate> topicTemplateRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,

            IRepository<Address> addressRepository,

            IRepository<SearchTerm> searchTermRepository,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper)
        {
            this._storeRepository = storeRepository;
            this._languageRepository = languageRepository;
            this._currencyRepository = currencyRepository;
            this._userRepository = userRepository;
            this._userPasswordRepository = userPasswordRepository;
            this._userRoleRepository = userRoleRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._emailAccountRepository = emailAccountRepository;
            this._messageTemplateRepository = messageTemplateRepository;
            this._countryRepository = countryRepository;
            this._stateProvinceRepository = stateProvinceRepository;
            this._topicRepository = topicRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._activityLogRepository = activityLogRepository;
            this._topicTemplateRepository = topicTemplateRepository;
            this._scheduleTaskRepository = scheduleTaskRepository;
            this._addressRepository = addressRepository;
            this._searchTermRepository = searchTermRepository;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallStores()
        {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "Your store name",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1,
                    //should we set some default company info?
                    CompanyName = "Your company name",
                    CompanyAddress = "your company country, state, zip, street, etc",
                    CompanyPhoneNumber = "(123) 456-78901",
                    CompanyVat = null,
                },
            };

            _storeRepository.Insert(stores);
        }



        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(CommonHelper.MapPath("~/App_Data/Localization/"), "*.Seagullres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void InstallCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "US Dollar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = "",
                    Published = true,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Australian Dollar",
                    CurrencyCode = "AUD",
                    Rate = 1.36M,
                    DisplayLocale = "en-AU",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "British Pound",
                    CurrencyCode = "GBP",
                    Rate = 0.82M,
                    DisplayLocale = "en-GB",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Canadian Dollar",
                    CurrencyCode = "CAD",
                    Rate = 1.32M,
                    DisplayLocale = "en-CA",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 4,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Chinese Yuan Renminbi",
                    CurrencyCode = "CNY",
                    Rate = 6.93M,
                    DisplayLocale = "zh-CN",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.95M,
                    DisplayLocale = "",
                    //CustomFormatting = "ˆ0.00",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"),
                    Published = true,
                    DisplayOrder = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Hong Kong Dollar",
                    CurrencyCode = "HKD",
                    Rate = 7.75M,
                    DisplayLocale = "zh-HK",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Japanese Yen",
                    CurrencyCode = "JPY",
                    Rate = 116.64M,
                    DisplayLocale = "ja-JP",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 8,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Russian Rouble",
                    CurrencyCode = "RUB",
                    Rate = 59.75M,
                    DisplayLocale = "ru-RU",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 9,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Swedish Krona",
                    CurrencyCode = "SEK",
                    Rate = 9.08M,
                    DisplayLocale = "sv-SE",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 10,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding1
                },
                new Currency
                {
                    Name = "Romanian Leu",
                    CurrencyCode = "RON",
                    Rate = 4.28M,
                    DisplayLocale = "ro-RO",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 11,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Indian Rupee",
                    CurrencyCode = "INR",
                    Rate = 68.17M,
                    DisplayLocale = "en-IN",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 12,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
            };
            _currencyRepository.Insert(currencies);
        }

        protected virtual void InstallCountriesAndStates()
        {
            var cUsa = new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                SubjectToVat = false,
                DisplayOrder = 1,
                Published = true,
            };
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplayOrder = 1,
            });
            var cCanada = new Country
            {
                Name = "Canada",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                SubjectToVat = false,
                DisplayOrder = 100,
                Published = true,
            };
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince
            {
                Name = "Yukon Territory",
                Abbreviation = "YT",
                Published = true,
                DisplayOrder = 1,
            });
            var countries = new List<Country>
                                {
                                    cUsa,
                                    cCanada,
                                    //other countries
                                    new Country
                                    {
                                        Name = "Argentina",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AR",
                                        ThreeLetterIsoCode = "ARG",
                                        NumericIsoCode = 32,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Armenia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AM",
                                        ThreeLetterIsoCode = "ARM",
                                        NumericIsoCode = 51,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Aruba",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AW",
                                        ThreeLetterIsoCode = "ABW",
                                        NumericIsoCode = 533,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Australia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AU",
                                        ThreeLetterIsoCode = "AUS",
                                        NumericIsoCode = 36,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Austria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AT",
                                        ThreeLetterIsoCode = "AUT",
                                        NumericIsoCode = 40,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Azerbaijan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AZ",
                                        ThreeLetterIsoCode = "AZE",
                                        NumericIsoCode = 31,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bahamas",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BS",
                                        ThreeLetterIsoCode = "BHS",
                                        NumericIsoCode = 44,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bangladesh",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BD",
                                        ThreeLetterIsoCode = "BGD",
                                        NumericIsoCode = 50,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belarus",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BY",
                                        ThreeLetterIsoCode = "BLR",
                                        NumericIsoCode = 112,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belgium",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BE",
                                        ThreeLetterIsoCode = "BEL",
                                        NumericIsoCode = 56,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Belize",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BZ",
                                        ThreeLetterIsoCode = "BLZ",
                                        NumericIsoCode = 84,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bermuda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BM",
                                        ThreeLetterIsoCode = "BMU",
                                        NumericIsoCode = 60,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bolivia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BO",
                                        ThreeLetterIsoCode = "BOL",
                                        NumericIsoCode = 68,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bosnia and Herzegowina",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BA",
                                        ThreeLetterIsoCode = "BIH",
                                        NumericIsoCode = 70,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Brazil",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BR",
                                        ThreeLetterIsoCode = "BRA",
                                        NumericIsoCode = 76,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bulgaria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BG",
                                        ThreeLetterIsoCode = "BGR",
                                        NumericIsoCode = 100,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cayman Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KY",
                                        ThreeLetterIsoCode = "CYM",
                                        NumericIsoCode = 136,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Chile",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CL",
                                        ThreeLetterIsoCode = "CHL",
                                        NumericIsoCode = 152,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "China",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CN",
                                        ThreeLetterIsoCode = "CHN",
                                        NumericIsoCode = 156,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Colombia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CO",
                                        ThreeLetterIsoCode = "COL",
                                        NumericIsoCode = 170,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Costa Rica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CR",
                                        ThreeLetterIsoCode = "CRI",
                                        NumericIsoCode = 188,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Croatia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HR",
                                        ThreeLetterIsoCode = "HRV",
                                        NumericIsoCode = 191,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cuba",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CU",
                                        ThreeLetterIsoCode = "CUB",
                                        NumericIsoCode = 192,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cyprus",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CY",
                                        ThreeLetterIsoCode = "CYP",
                                        NumericIsoCode = 196,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Czech Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CZ",
                                        ThreeLetterIsoCode = "CZE",
                                        NumericIsoCode = 203,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Denmark",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DK",
                                        ThreeLetterIsoCode = "DNK",
                                        NumericIsoCode = 208,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Dominican Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DO",
                                        ThreeLetterIsoCode = "DOM",
                                        NumericIsoCode = 214,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "East Timor",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TL",
                                        ThreeLetterIsoCode = "TLS",
                                        NumericIsoCode = 626,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ecuador",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EC",
                                        ThreeLetterIsoCode = "ECU",
                                        NumericIsoCode = 218,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Egypt",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EG",
                                        ThreeLetterIsoCode = "EGY",
                                        NumericIsoCode = 818,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Finland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FI",
                                        ThreeLetterIsoCode = "FIN",
                                        NumericIsoCode = 246,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "France",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FR",
                                        ThreeLetterIsoCode = "FRA",
                                        NumericIsoCode = 250,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Georgia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GE",
                                        ThreeLetterIsoCode = "GEO",
                                        NumericIsoCode = 268,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Germany",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DE",
                                        ThreeLetterIsoCode = "DEU",
                                        NumericIsoCode = 276,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gibraltar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GI",
                                        ThreeLetterIsoCode = "GIB",
                                        NumericIsoCode = 292,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Greece",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GR",
                                        ThreeLetterIsoCode = "GRC",
                                        NumericIsoCode = 300,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guatemala",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GT",
                                        ThreeLetterIsoCode = "GTM",
                                        NumericIsoCode = 320,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Hong Kong",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HK",
                                        ThreeLetterIsoCode = "HKG",
                                        NumericIsoCode = 344,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Hungary",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HU",
                                        ThreeLetterIsoCode = "HUN",
                                        NumericIsoCode = 348,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "India",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IN",
                                        ThreeLetterIsoCode = "IND",
                                        NumericIsoCode = 356,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Indonesia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ID",
                                        ThreeLetterIsoCode = "IDN",
                                        NumericIsoCode = 360,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ireland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IE",
                                        ThreeLetterIsoCode = "IRL",
                                        NumericIsoCode = 372,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Israel",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IL",
                                        ThreeLetterIsoCode = "ISR",
                                        NumericIsoCode = 376,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Italy",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IT",
                                        ThreeLetterIsoCode = "ITA",
                                        NumericIsoCode = 380,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Jamaica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "JM",
                                        ThreeLetterIsoCode = "JAM",
                                        NumericIsoCode = 388,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Japan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "JP",
                                        ThreeLetterIsoCode = "JPN",
                                        NumericIsoCode = 392,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Jordan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "JO",
                                        ThreeLetterIsoCode = "JOR",
                                        NumericIsoCode = 400,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kazakhstan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KZ",
                                        ThreeLetterIsoCode = "KAZ",
                                        NumericIsoCode = 398,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Korea, Democratic People's Republic of",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KP",
                                        ThreeLetterIsoCode = "PRK",
                                        NumericIsoCode = 408,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kuwait",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KW",
                                        ThreeLetterIsoCode = "KWT",
                                        NumericIsoCode = 414,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malaysia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MY",
                                        ThreeLetterIsoCode = "MYS",
                                        NumericIsoCode = 458,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mexico",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MX",
                                        ThreeLetterIsoCode = "MEX",
                                        NumericIsoCode = 484,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Netherlands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NL",
                                        ThreeLetterIsoCode = "NLD",
                                        NumericIsoCode = 528,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "New Zealand",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NZ",
                                        ThreeLetterIsoCode = "NZL",
                                        NumericIsoCode = 554,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Norway",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NO",
                                        ThreeLetterIsoCode = "NOR",
                                        NumericIsoCode = 578,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Pakistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PK",
                                        ThreeLetterIsoCode = "PAK",
                                        NumericIsoCode = 586,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Palestine",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PS",
                                        ThreeLetterIsoCode = "PSE",
                                        NumericIsoCode = 275,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Paraguay",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PY",
                                        ThreeLetterIsoCode = "PRY",
                                        NumericIsoCode = 600,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Peru",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PE",
                                        ThreeLetterIsoCode = "PER",
                                        NumericIsoCode = 604,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Philippines",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PH",
                                        ThreeLetterIsoCode = "PHL",
                                        NumericIsoCode = 608,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Poland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PL",
                                        ThreeLetterIsoCode = "POL",
                                        NumericIsoCode = 616,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Portugal",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PT",
                                        ThreeLetterIsoCode = "PRT",
                                        NumericIsoCode = 620,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Puerto Rico",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PR",
                                        ThreeLetterIsoCode = "PRI",
                                        NumericIsoCode = 630,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Qatar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "QA",
                                        ThreeLetterIsoCode = "QAT",
                                        NumericIsoCode = 634,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Romania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RO",
                                        ThreeLetterIsoCode = "ROM",
                                        NumericIsoCode = 642,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Russian Federation",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RU",
                                        ThreeLetterIsoCode = "RUS",
                                        NumericIsoCode = 643,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saudi Arabia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SA",
                                        ThreeLetterIsoCode = "SAU",
                                        NumericIsoCode = 682,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Singapore",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SG",
                                        ThreeLetterIsoCode = "SGP",
                                        NumericIsoCode = 702,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Slovakia (Slovak Republic)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SK",
                                        ThreeLetterIsoCode = "SVK",
                                        NumericIsoCode = 703,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Slovenia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SI",
                                        ThreeLetterIsoCode = "SVN",
                                        NumericIsoCode = 705,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Africa",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ZA",
                                        ThreeLetterIsoCode = "ZAF",
                                        NumericIsoCode = 710,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Spain",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ES",
                                        ThreeLetterIsoCode = "ESP",
                                        NumericIsoCode = 724,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sweden",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SE",
                                        ThreeLetterIsoCode = "SWE",
                                        NumericIsoCode = 752,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Switzerland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CH",
                                        ThreeLetterIsoCode = "CHE",
                                        NumericIsoCode = 756,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Taiwan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TW",
                                        ThreeLetterIsoCode = "TWN",
                                        NumericIsoCode = 158,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Thailand",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TH",
                                        ThreeLetterIsoCode = "THA",
                                        NumericIsoCode = 764,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turkey",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TR",
                                        ThreeLetterIsoCode = "TUR",
                                        NumericIsoCode = 792,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ukraine",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UA",
                                        ThreeLetterIsoCode = "UKR",
                                        NumericIsoCode = 804,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United Arab Emirates",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AE",
                                        ThreeLetterIsoCode = "ARE",
                                        NumericIsoCode = 784,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United Kingdom",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GB",
                                        ThreeLetterIsoCode = "GBR",
                                        NumericIsoCode = 826,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "United States minor outlying islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UM",
                                        ThreeLetterIsoCode = "UMI",
                                        NumericIsoCode = 581,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uruguay",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UY",
                                        ThreeLetterIsoCode = "URY",
                                        NumericIsoCode = 858,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uzbekistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UZ",
                                        ThreeLetterIsoCode = "UZB",
                                        NumericIsoCode = 860,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Venezuela",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VE",
                                        ThreeLetterIsoCode = "VEN",
                                        NumericIsoCode = 862,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Serbia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RS",
                                        ThreeLetterIsoCode = "SRB",
                                        NumericIsoCode = 688,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Afghanistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AF",
                                        ThreeLetterIsoCode = "AFG",
                                        NumericIsoCode = 4,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Albania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AL",
                                        ThreeLetterIsoCode = "ALB",
                                        NumericIsoCode = 8,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Algeria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DZ",
                                        ThreeLetterIsoCode = "DZA",
                                        NumericIsoCode = 12,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "American Samoa",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AS",
                                        ThreeLetterIsoCode = "ASM",
                                        NumericIsoCode = 16,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Andorra",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AD",
                                        ThreeLetterIsoCode = "AND",
                                        NumericIsoCode = 20,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Angola",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AO",
                                        ThreeLetterIsoCode = "AGO",
                                        NumericIsoCode = 24,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Anguilla",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AI",
                                        ThreeLetterIsoCode = "AIA",
                                        NumericIsoCode = 660,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Antarctica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AQ",
                                        ThreeLetterIsoCode = "ATA",
                                        NumericIsoCode = 10,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Antigua and Barbuda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AG",
                                        ThreeLetterIsoCode = "ATG",
                                        NumericIsoCode = 28,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bahrain",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BH",
                                        ThreeLetterIsoCode = "BHR",
                                        NumericIsoCode = 48,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Barbados",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BB",
                                        ThreeLetterIsoCode = "BRB",
                                        NumericIsoCode = 52,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Benin",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BJ",
                                        ThreeLetterIsoCode = "BEN",
                                        NumericIsoCode = 204,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bhutan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BT",
                                        ThreeLetterIsoCode = "BTN",
                                        NumericIsoCode = 64,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Botswana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BW",
                                        ThreeLetterIsoCode = "BWA",
                                        NumericIsoCode = 72,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Bouvet Island",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BV",
                                        ThreeLetterIsoCode = "BVT",
                                        NumericIsoCode = 74,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "British Indian Ocean Territory",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IO",
                                        ThreeLetterIsoCode = "IOT",
                                        NumericIsoCode = 86,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Brunei Darussalam",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BN",
                                        ThreeLetterIsoCode = "BRN",
                                        NumericIsoCode = 96,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Burkina Faso",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BF",
                                        ThreeLetterIsoCode = "BFA",
                                        NumericIsoCode = 854,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Burundi",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "BI",
                                        ThreeLetterIsoCode = "BDI",
                                        NumericIsoCode = 108,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cambodia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KH",
                                        ThreeLetterIsoCode = "KHM",
                                        NumericIsoCode = 116,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cameroon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CM",
                                        ThreeLetterIsoCode = "CMR",
                                        NumericIsoCode = 120,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cape Verde",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CV",
                                        ThreeLetterIsoCode = "CPV",
                                        NumericIsoCode = 132,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Central African Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CF",
                                        ThreeLetterIsoCode = "CAF",
                                        NumericIsoCode = 140,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Chad",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TD",
                                        ThreeLetterIsoCode = "TCD",
                                        NumericIsoCode = 148,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Christmas Island",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CX",
                                        ThreeLetterIsoCode = "CXR",
                                        NumericIsoCode = 162,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cocos (Keeling) Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CC",
                                        ThreeLetterIsoCode = "CCK",
                                        NumericIsoCode = 166,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Comoros",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KM",
                                        ThreeLetterIsoCode = "COM",
                                        NumericIsoCode = 174,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Congo",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CG",
                                        ThreeLetterIsoCode = "COG",
                                        NumericIsoCode = 178,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Congo (Democratic Republic of the)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CD",
                                        ThreeLetterIsoCode = "COD",
                                        NumericIsoCode = 180,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cook Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CK",
                                        ThreeLetterIsoCode = "COK",
                                        NumericIsoCode = 184,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Cote D'Ivoire",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "CI",
                                        ThreeLetterIsoCode = "CIV",
                                        NumericIsoCode = 384,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Djibouti",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DJ",
                                        ThreeLetterIsoCode = "DJI",
                                        NumericIsoCode = 262,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Dominica",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "DM",
                                        ThreeLetterIsoCode = "DMA",
                                        NumericIsoCode = 212,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "El Salvador",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SV",
                                        ThreeLetterIsoCode = "SLV",
                                        NumericIsoCode = 222,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Equatorial Guinea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GQ",
                                        ThreeLetterIsoCode = "GNQ",
                                        NumericIsoCode = 226,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Eritrea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ER",
                                        ThreeLetterIsoCode = "ERI",
                                        NumericIsoCode = 232,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Estonia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EE",
                                        ThreeLetterIsoCode = "EST",
                                        NumericIsoCode = 233,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ethiopia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ET",
                                        ThreeLetterIsoCode = "ETH",
                                        NumericIsoCode = 231,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Falkland Islands (Malvinas)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FK",
                                        ThreeLetterIsoCode = "FLK",
                                        NumericIsoCode = 238,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Faroe Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FO",
                                        ThreeLetterIsoCode = "FRO",
                                        NumericIsoCode = 234,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Fiji",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FJ",
                                        ThreeLetterIsoCode = "FJI",
                                        NumericIsoCode = 242,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Guiana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GF",
                                        ThreeLetterIsoCode = "GUF",
                                        NumericIsoCode = 254,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Polynesia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PF",
                                        ThreeLetterIsoCode = "PYF",
                                        NumericIsoCode = 258,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "French Southern Territories",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TF",
                                        ThreeLetterIsoCode = "ATF",
                                        NumericIsoCode = 260,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gabon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GA",
                                        ThreeLetterIsoCode = "GAB",
                                        NumericIsoCode = 266,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Gambia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GM",
                                        ThreeLetterIsoCode = "GMB",
                                        NumericIsoCode = 270,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Ghana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GH",
                                        ThreeLetterIsoCode = "GHA",
                                        NumericIsoCode = 288,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Greenland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GL",
                                        ThreeLetterIsoCode = "GRL",
                                        NumericIsoCode = 304,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Grenada",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GD",
                                        ThreeLetterIsoCode = "GRD",
                                        NumericIsoCode = 308,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guadeloupe",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GP",
                                        ThreeLetterIsoCode = "GLP",
                                        NumericIsoCode = 312,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guam",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GU",
                                        ThreeLetterIsoCode = "GUM",
                                        NumericIsoCode = 316,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guinea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GN",
                                        ThreeLetterIsoCode = "GIN",
                                        NumericIsoCode = 324,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guinea-bissau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GW",
                                        ThreeLetterIsoCode = "GNB",
                                        NumericIsoCode = 624,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Guyana",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GY",
                                        ThreeLetterIsoCode = "GUY",
                                        NumericIsoCode = 328,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Haiti",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HT",
                                        ThreeLetterIsoCode = "HTI",
                                        NumericIsoCode = 332,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Heard and Mc Donald Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HM",
                                        ThreeLetterIsoCode = "HMD",
                                        NumericIsoCode = 334,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Honduras",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "HN",
                                        ThreeLetterIsoCode = "HND",
                                        NumericIsoCode = 340,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iceland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IS",
                                        ThreeLetterIsoCode = "ISL",
                                        NumericIsoCode = 352,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iran (Islamic Republic of)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IR",
                                        ThreeLetterIsoCode = "IRN",
                                        NumericIsoCode = 364,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Iraq",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "IQ",
                                        ThreeLetterIsoCode = "IRQ",
                                        NumericIsoCode = 368,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kenya",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KE",
                                        ThreeLetterIsoCode = "KEN",
                                        NumericIsoCode = 404,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kiribati",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KI",
                                        ThreeLetterIsoCode = "KIR",
                                        NumericIsoCode = 296,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Korea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KR",
                                        ThreeLetterIsoCode = "KOR",
                                        NumericIsoCode = 410,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Kyrgyzstan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KG",
                                        ThreeLetterIsoCode = "KGZ",
                                        NumericIsoCode = 417,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lao People's Democratic Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LA",
                                        ThreeLetterIsoCode = "LAO",
                                        NumericIsoCode = 418,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Latvia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LV",
                                        ThreeLetterIsoCode = "LVA",
                                        NumericIsoCode = 428,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lebanon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LB",
                                        ThreeLetterIsoCode = "LBN",
                                        NumericIsoCode = 422,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lesotho",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LS",
                                        ThreeLetterIsoCode = "LSO",
                                        NumericIsoCode = 426,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Liberia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LR",
                                        ThreeLetterIsoCode = "LBR",
                                        NumericIsoCode = 430,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Libyan Arab Jamahiriya",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LY",
                                        ThreeLetterIsoCode = "LBY",
                                        NumericIsoCode = 434,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Liechtenstein",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LI",
                                        ThreeLetterIsoCode = "LIE",
                                        NumericIsoCode = 438,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Lithuania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LT",
                                        ThreeLetterIsoCode = "LTU",
                                        NumericIsoCode = 440,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Luxembourg",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LU",
                                        ThreeLetterIsoCode = "LUX",
                                        NumericIsoCode = 442,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Macau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MO",
                                        ThreeLetterIsoCode = "MAC",
                                        NumericIsoCode = 446,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Macedonia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MK",
                                        ThreeLetterIsoCode = "MKD",
                                        NumericIsoCode = 807,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Madagascar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MG",
                                        ThreeLetterIsoCode = "MDG",
                                        NumericIsoCode = 450,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malawi",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MW",
                                        ThreeLetterIsoCode = "MWI",
                                        NumericIsoCode = 454,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Maldives",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MV",
                                        ThreeLetterIsoCode = "MDV",
                                        NumericIsoCode = 462,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mali",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ML",
                                        ThreeLetterIsoCode = "MLI",
                                        NumericIsoCode = 466,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Malta",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MT",
                                        ThreeLetterIsoCode = "MLT",
                                        NumericIsoCode = 470,
                                        SubjectToVat = true,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Marshall Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MH",
                                        ThreeLetterIsoCode = "MHL",
                                        NumericIsoCode = 584,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Martinique",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MQ",
                                        ThreeLetterIsoCode = "MTQ",
                                        NumericIsoCode = 474,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mauritania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MR",
                                        ThreeLetterIsoCode = "MRT",
                                        NumericIsoCode = 478,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mauritius",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MU",
                                        ThreeLetterIsoCode = "MUS",
                                        NumericIsoCode = 480,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mayotte",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "YT",
                                        ThreeLetterIsoCode = "MYT",
                                        NumericIsoCode = 175,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Micronesia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "FM",
                                        ThreeLetterIsoCode = "FSM",
                                        NumericIsoCode = 583,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Moldova",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MD",
                                        ThreeLetterIsoCode = "MDA",
                                        NumericIsoCode = 498,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Monaco",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MC",
                                        ThreeLetterIsoCode = "MCO",
                                        NumericIsoCode = 492,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mongolia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MN",
                                        ThreeLetterIsoCode = "MNG",
                                        NumericIsoCode = 496,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Montenegro",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ME",
                                        ThreeLetterIsoCode = "MNE",
                                        NumericIsoCode = 499,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Montserrat",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MS",
                                        ThreeLetterIsoCode = "MSR",
                                        NumericIsoCode = 500,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Morocco",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MA",
                                        ThreeLetterIsoCode = "MAR",
                                        NumericIsoCode = 504,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Mozambique",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MZ",
                                        ThreeLetterIsoCode = "MOZ",
                                        NumericIsoCode = 508,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Myanmar",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MM",
                                        ThreeLetterIsoCode = "MMR",
                                        NumericIsoCode = 104,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Namibia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NA",
                                        ThreeLetterIsoCode = "NAM",
                                        NumericIsoCode = 516,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nauru",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NR",
                                        ThreeLetterIsoCode = "NRU",
                                        NumericIsoCode = 520,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nepal",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NP",
                                        ThreeLetterIsoCode = "NPL",
                                        NumericIsoCode = 524,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Netherlands Antilles",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "AN",
                                        ThreeLetterIsoCode = "ANT",
                                        NumericIsoCode = 530,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "New Caledonia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NC",
                                        ThreeLetterIsoCode = "NCL",
                                        NumericIsoCode = 540,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nicaragua",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NI",
                                        ThreeLetterIsoCode = "NIC",
                                        NumericIsoCode = 558,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Niger",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NE",
                                        ThreeLetterIsoCode = "NER",
                                        NumericIsoCode = 562,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Nigeria",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NG",
                                        ThreeLetterIsoCode = "NGA",
                                        NumericIsoCode = 566,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Niue",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NU",
                                        ThreeLetterIsoCode = "NIU",
                                        NumericIsoCode = 570,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Norfolk Island",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "NF",
                                        ThreeLetterIsoCode = "NFK",
                                        NumericIsoCode = 574,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Northern Mariana Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "MP",
                                        ThreeLetterIsoCode = "MNP",
                                        NumericIsoCode = 580,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Oman",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "OM",
                                        ThreeLetterIsoCode = "OMN",
                                        NumericIsoCode = 512,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Palau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PW",
                                        ThreeLetterIsoCode = "PLW",
                                        NumericIsoCode = 585,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Panama",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PA",
                                        ThreeLetterIsoCode = "PAN",
                                        NumericIsoCode = 591,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Papua New Guinea",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PG",
                                        ThreeLetterIsoCode = "PNG",
                                        NumericIsoCode = 598,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Pitcairn",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PN",
                                        ThreeLetterIsoCode = "PCN",
                                        NumericIsoCode = 612,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Reunion",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RE",
                                        ThreeLetterIsoCode = "REU",
                                        NumericIsoCode = 638,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Rwanda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "RW",
                                        ThreeLetterIsoCode = "RWA",
                                        NumericIsoCode = 646,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Kitts and Nevis",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "KN",
                                        ThreeLetterIsoCode = "KNA",
                                        NumericIsoCode = 659,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Lucia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LC",
                                        ThreeLetterIsoCode = "LCA",
                                        NumericIsoCode = 662,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Saint Vincent and the Grenadines",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VC",
                                        ThreeLetterIsoCode = "VCT",
                                        NumericIsoCode = 670,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Samoa",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "WS",
                                        ThreeLetterIsoCode = "WSM",
                                        NumericIsoCode = 882,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "San Marino",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SM",
                                        ThreeLetterIsoCode = "SMR",
                                        NumericIsoCode = 674,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sao Tome and Principe",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ST",
                                        ThreeLetterIsoCode = "STP",
                                        NumericIsoCode = 678,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Senegal",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SN",
                                        ThreeLetterIsoCode = "SEN",
                                        NumericIsoCode = 686,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Seychelles",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SC",
                                        ThreeLetterIsoCode = "SYC",
                                        NumericIsoCode = 690,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sierra Leone",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SL",
                                        ThreeLetterIsoCode = "SLE",
                                        NumericIsoCode = 694,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Solomon Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SB",
                                        ThreeLetterIsoCode = "SLB",
                                        NumericIsoCode = 90,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Somalia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SO",
                                        ThreeLetterIsoCode = "SOM",
                                        NumericIsoCode = 706,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Georgia & South Sandwich Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "GS",
                                        ThreeLetterIsoCode = "SGS",
                                        NumericIsoCode = 239,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "South Sudan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SS",
                                        ThreeLetterIsoCode = "SSD",
                                        NumericIsoCode = 728,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sri Lanka",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "LK",
                                        ThreeLetterIsoCode = "LKA",
                                        NumericIsoCode = 144,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "St. Helena",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SH",
                                        ThreeLetterIsoCode = "SHN",
                                        NumericIsoCode = 654,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "St. Pierre and Miquelon",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "PM",
                                        ThreeLetterIsoCode = "SPM",
                                        NumericIsoCode = 666,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Sudan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SD",
                                        ThreeLetterIsoCode = "SDN",
                                        NumericIsoCode = 736,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Suriname",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SR",
                                        ThreeLetterIsoCode = "SUR",
                                        NumericIsoCode = 740,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Svalbard and Jan Mayen Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SJ",
                                        ThreeLetterIsoCode = "SJM",
                                        NumericIsoCode = 744,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Swaziland",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SZ",
                                        ThreeLetterIsoCode = "SWZ",
                                        NumericIsoCode = 748,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Syrian Arab Republic",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "SY",
                                        ThreeLetterIsoCode = "SYR",
                                        NumericIsoCode = 760,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tajikistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TJ",
                                        ThreeLetterIsoCode = "TJK",
                                        NumericIsoCode = 762,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tanzania",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TZ",
                                        ThreeLetterIsoCode = "TZA",
                                        NumericIsoCode = 834,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Togo",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TG",
                                        ThreeLetterIsoCode = "TGO",
                                        NumericIsoCode = 768,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tokelau",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TK",
                                        ThreeLetterIsoCode = "TKL",
                                        NumericIsoCode = 772,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tonga",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TO",
                                        ThreeLetterIsoCode = "TON",
                                        NumericIsoCode = 776,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Trinidad and Tobago",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TT",
                                        ThreeLetterIsoCode = "TTO",
                                        NumericIsoCode = 780,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tunisia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TN",
                                        ThreeLetterIsoCode = "TUN",
                                        NumericIsoCode = 788,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turkmenistan",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TM",
                                        ThreeLetterIsoCode = "TKM",
                                        NumericIsoCode = 795,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Turks and Caicos Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TC",
                                        ThreeLetterIsoCode = "TCA",
                                        NumericIsoCode = 796,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Tuvalu",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "TV",
                                        ThreeLetterIsoCode = "TUV",
                                        NumericIsoCode = 798,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Uganda",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "UG",
                                        ThreeLetterIsoCode = "UGA",
                                        NumericIsoCode = 800,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Vanuatu",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VU",
                                        ThreeLetterIsoCode = "VUT",
                                        NumericIsoCode = 548,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Vatican City State (Holy See)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VA",
                                        ThreeLetterIsoCode = "VAT",
                                        NumericIsoCode = 336,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Viet Nam",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VN",
                                        ThreeLetterIsoCode = "VNM",
                                        NumericIsoCode = 704,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Virgin Islands (British)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VG",
                                        ThreeLetterIsoCode = "VGB",
                                        NumericIsoCode = 92,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Virgin Islands (U.S.)",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "VI",
                                        ThreeLetterIsoCode = "VIR",
                                        NumericIsoCode = 850,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Wallis and Futuna Islands",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "WF",
                                        ThreeLetterIsoCode = "WLF",
                                        NumericIsoCode = 876,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Western Sahara",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "EH",
                                        ThreeLetterIsoCode = "ESH",
                                        NumericIsoCode = 732,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Yemen",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "YE",
                                        ThreeLetterIsoCode = "YEM",
                                        NumericIsoCode = 887,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Zambia",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ZM",
                                        ThreeLetterIsoCode = "ZMB",
                                        NumericIsoCode = 894,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                    new Country
                                    {
                                        Name = "Zimbabwe",
                                        AllowsBilling = true,
                                        AllowsShipping = true,
                                        TwoLetterIsoCode = "ZW",
                                        ThreeLetterIsoCode = "ZWE",
                                        NumericIsoCode = 716,
                                        SubjectToVat = false,
                                        DisplayOrder = 100,
                                        Published = true
                                    },
                                };
            _countryRepository.Insert(countries);
        }


        protected virtual void InstallUsersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new UserRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Administrators,
            };
            //var crForumModerators = new UserRole
            //{
            //    Name = "Forum Moderators",
            //    Active = true,
            //    IsSystemRole = true,
            //    SystemName = SystemUserRoleNames.ForumModerators,
            //};
            var crRegistered = new UserRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Registered,
            };
            //var crGuests = new UserRole
            //{
            //    Name = "Guests",
            //    Active = true,
            //    IsSystemRole = true,
            //    SystemName = SystemUserRoleNames.Guests,
            //};
            
            var userRoles = new List<UserRole>
                                {
                                    crAdministrators,
                                    //crForumModerators,
                                    crRegistered,
                                    //crGuests,
                                };
            _userRoleRepository.Insert(userRoles);

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //admin user
            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultAdminUserAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = defaultUserEmail,
                FaxNumber = "",
                Address1 = "21 West 52nd Street",
                Address2 = "",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.BillingAddress = defaultAdminUserAddress;

            adminUser.UserRoles.Add(crAdministrators);
            //adminUser.UserRoles.Add(crForumModerators);
            adminUser.UserRoles.Add(crRegistered);

            _userRepository.Insert(adminUser);
            //set default user name
            _genericAttributeService.SaveAttribute(adminUser, SystemUserAttributeNames.FirstName, "John");
            _genericAttributeService.SaveAttribute(adminUser, SystemUserAttributeNames.LastName, "Smith");

            //set hashed admin password
            var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            userRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));

            //second user
            var secondUserEmail = "steve_gates@SeagullCommerce.com";
            var secondUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSecondUserAddress = new Address
            {
                FirstName = "Steve",
                LastName = "Gates",
                PhoneNumber = "87654321",
                Email = secondUserEmail,
                FaxNumber = "",
                Address1 = "750 Bel Air Rd.",
                Address2 = "",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90077",
                CreatedOnUtc = DateTime.UtcNow,
            };
            secondUser.Addresses.Add(defaultSecondUserAddress);
            secondUser.BillingAddress = defaultSecondUserAddress;

            secondUser.UserRoles.Add(crRegistered);

            _userRepository.Insert(secondUser);
            //set default user name
            _genericAttributeService.SaveAttribute(secondUser, SystemUserAttributeNames.FirstName, defaultSecondUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(secondUser, SystemUserAttributeNames.LastName, defaultSecondUserAddress.LastName);

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = secondUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //third user
            var thirdUserEmail = "arthur_holmes@SeagullCommerce.com";
            var thirdUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultThirdUserAddress = new Address
            {
                FirstName = "Arthur",
                LastName = "Holmes",
                PhoneNumber = "111222333",
                Email = thirdUserEmail,
                FaxNumber = "",
                Address1 = "221B Baker Street",
                Address2 = "",
                City = "London",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "NW1 6XE",
                CreatedOnUtc = DateTime.UtcNow,
            };
            thirdUser.Addresses.Add(defaultThirdUserAddress);
            thirdUser.BillingAddress = defaultThirdUserAddress;

            thirdUser.UserRoles.Add(crRegistered);

            _userRepository.Insert(thirdUser);
            //set default user name
            _genericAttributeService.SaveAttribute(thirdUser, SystemUserAttributeNames.FirstName, defaultThirdUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(thirdUser, SystemUserAttributeNames.LastName, defaultThirdUserAddress.LastName);

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = thirdUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fourth user
            var fourthUserEmail = "james_pan@SeagullCommerce.com";
            var fourthUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = fourthUserEmail,
                Username = fourthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFourthUserAddress = new Address
            {
                FirstName = "James",
                LastName = "Pan",
                PhoneNumber = "369258147",
                Email = fourthUserEmail,
                FaxNumber = "",
                Address1 = "St Katharine’s West 16",
                Address2 = "",
                City = "St Andrews",
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR"),
                ZipPostalCode = "KY16 9AX",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fourthUser.Addresses.Add(defaultFourthUserAddress);
            fourthUser.BillingAddress = defaultFourthUserAddress;

            fourthUser.UserRoles.Add(crRegistered);

            _userRepository.Insert(fourthUser);
            //set default user name
            _genericAttributeService.SaveAttribute(fourthUser, SystemUserAttributeNames.FirstName, defaultFourthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fourthUser, SystemUserAttributeNames.LastName, defaultFourthUserAddress.LastName);

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = fourthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fifth user
            var fifthUserEmail = "brenda_lindgren@SeagullCommerce.com";
            var fifthUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = fifthUserEmail,
                Username = fifthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFifthUserAddress = new Address
            {
                FirstName = "Brenda",
                LastName = "Lindgren",
                PhoneNumber = "14785236",
                Email = fifthUserEmail,
                FaxNumber = "",
                Address1 = "1249 Tongass Avenue, Suite B",
                Address2 = "",
                City = "Ketchikan",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Alaska"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "99901",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fifthUser.Addresses.Add(defaultFifthUserAddress);
            fifthUser.BillingAddress = defaultFifthUserAddress;

            fifthUser.UserRoles.Add(crRegistered);

            _userRepository.Insert(fifthUser);
            //set default user name
            _genericAttributeService.SaveAttribute(fifthUser, SystemUserAttributeNames.FirstName, defaultFifthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fifthUser, SystemUserAttributeNames.LastName, defaultFifthUserAddress.LastName);

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = fifthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //sixth user
            var sixthUserEmail = "victoria_victoria@SeagullCommerce.com";
            var sixthUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = sixthUserEmail,
                Username = sixthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSixthUserAddress = new Address
            {
                FirstName = "Victoria",
                LastName = "Terces",
                PhoneNumber = "45612378",
                Email = sixthUserEmail,
                FaxNumber = "",
                Address1 = "201 1st Avenue South",
                Address2 = "",
                City = "Saskatoon",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Saskatchewan"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "CAN"),
                ZipPostalCode = "S7K 1J9",
                CreatedOnUtc = DateTime.UtcNow,
            };
            sixthUser.Addresses.Add(defaultSixthUserAddress);
            sixthUser.BillingAddress = defaultSixthUserAddress;

            sixthUser.UserRoles.Add(crRegistered);

            _userRepository.Insert(sixthUser);
            //set default user name
            _genericAttributeService.SaveAttribute(sixthUser, SystemUserAttributeNames.FirstName, defaultSixthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(sixthUser, SystemUserAttributeNames.LastName, defaultSixthUserAddress.LastName);

            //set user password
            _userPasswordRepository.Insert(new UserPassword
            {
                User = sixthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            ////search engine (crawler) built-in user
            //var searchEngineUser = new User
            //{
            //    Email = "builtin@search_engine_record.com",
            //    UserGuid = Guid.NewGuid(),
            //    AdminComment = "Built-in system guest record used for requests from search engines.",
            //    Active = true,
            //    IsSystemAccount = true,
            //    SystemName = SystemUserNames.SearchEngine,
            //    CreatedOnUtc = DateTime.UtcNow,
            //    LastActivityDateUtc = DateTime.UtcNow,
            //    RegisteredInStoreId = storeId
            //};
            //searchEngineUser.UserRoles.Add(crGuests);
            //_userRepository.Insert(searchEngineUser);


            ////built-in user for background tasks
            //var backgroundTaskUser = new User
            //{
            //    Email = "builtin@background-task-record.com",
            //    UserGuid = Guid.NewGuid(),
            //    AdminComment = "Built-in system record used for background tasks.",
            //    Active = true,
            //    IsSystemAccount = true,
            //    SystemName = SystemUserNames.BackgroundTask,
            //    CreatedOnUtc = DateTime.UtcNow,
            //    LastActivityDateUtc = DateTime.UtcNow,
            //    RegisteredInStoreId = storeId
            //};
            //backgroundTaskUser.UserRoles.Add(crGuests);
            //_userRepository.Insert(backgroundTaskUser);
        }

        protected virtual void InstallActivityLog(string defaultUserEmail)
        {
            //default user/user
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditCategory")),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditDiscount")),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditSpecAttribute")),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("AddNewProductAttribute")),
                Comment = "Added a new product attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteGiftCard")),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                User = defaultUser,
                IpAddress = "127.0.0.1"
            });
        }

        protected virtual void InstallSearchTerms()
        {
            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 34,
                Keyword = "computer",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 30,
                Keyword = "camera",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 27,
                Keyword = "jewelry",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 26,
                Keyword = "shoes",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 19,
                Keyword = "jeans",
                StoreId = defaultStore.Id
            });
            _searchTermRepository.Insert(new SearchTerm()
            {
                Count = 10,
                Keyword = "gift",
                StoreId = defaultStore.Id
            });
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
                               {
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "Store name",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                               };
            _emailAccountRepository.Insert(emailAccounts);
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BlogCommentNotification,
                    Subject = "%Store.Name%. New blog comment.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BackInStockNotification,
                    Subject = "%Store.Name%. Back in stock notification",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%,{0}<br />{0}Product <a target=\"_blank\" href=\"%BackInStockSubscription.ProductUrl%\">%BackInStockSubscription.ProductName%</a> is in stock.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserEmailValidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = string.Format("<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}To activate your account <a href=\"%User.AccountActivationURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserEmailRevalidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%!{0}<br />{0}To validate your new email address <a href=\"%User.EmailRevalidationURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.PrivateMessageNotification,
                    Subject = "%Store.Name%. You have received a new private message",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}You have received a new private message.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserPasswordRecoveryMessage,
                    Subject = "%Store.Name%. Password recovery",
                    Body = string.Format("<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.{0}<br />{0}<br />{0}%Store.Name%{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserWelcomeMessage,
                    Subject = "Welcome to %Store.Name%",
                    Body = string.Format("We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.{0}<br />{0}<br />{0}You can now take part in the various services we have to offer you. Some of these services include:{0}<br />{0}<br />{0}Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.{0}<br />{0}Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{0}<br />{0}Order History - View your history of purchases that you have made with us.{0}<br />{0}Products Reviews - Share your opinions on products with our other users.{0}<br />{0}<br />{0}For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{0}<br />{0}<br />{0}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumPostMessage,
                    Subject = "%Store.Name%. New Post Notification.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{0}<br />{0}<br />{0}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{0}<br />{0}<br />{0}Post author: %Forums.PostAuthor%{0}<br />{0}Post body: %Forums.PostBody%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumTopicMessage,
                    Subject = "%Store.Name%. New Topic Notification.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{0}<br />{0}<br />{0}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.GiftCardNotification,
                    Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                    Body = string.Format("<p>{0}You have received a gift card for %Store.Name%{0}</p>{0}<p>{0}Dear %GiftCard.RecipientName%,{0}<br />{0}<br />{0}%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift cart for <a href=\"%Store.URL%\"> %Store.Name%</a>{0}</p>{0}<p>{0}You gift card code is %GiftCard.CouponCode%{0}</p>{0}<p>{0}%GiftCard.Message%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserRegisteredNotification,
                    Subject = "%Store.Name%. New user registration",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new user registered with your store. Below are the user's details:{0}<br />{0}Full name: %User.FullName%{0}<br />{0}Email: %User.Email%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%User.FullName% has just submitted a new return request. Details are below:{0}<br />{0}Request ID: %ReturnRequest.CustomNumber%{0}<br />{0}Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%{0}<br />{0}Reason for return: %ReturnRequest.Reason%{0}<br />{0}Requested action: %ReturnRequest.RequestedAction%{0}<br />{0}User comments:{0}<br />{0}%ReturnRequest.UserComment%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestUserNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%!{0}<br />{0}You have just submitted a new return request. Details are below:{0}<br />{0}Request ID: %ReturnRequest.CustomNumber%{0}<br />{0}Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%{0}<br />{0}Reason for return: %ReturnRequest.Reason%{0}<br />{0}Requested action: %ReturnRequest.RequestedAction%{0}<br />{0}User comments:{0}<br />{0}%ReturnRequest.UserComment%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsCommentNotification,
                    Subject = "%Store.Name%. New news comment.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
                    Subject = "%Store.Name%. Subscription activation message.",
                    Body = string.Format("<p>{0}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{0}</p>{0}<p>{0}If you received this email by mistake, simply delete it.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
                    Subject = "%Store.Name%. Subscription deactivation message.",
                    Body = string.Format("<p>{0}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{0}</p>{0}<p>{0}If you received this email by mistake, simply delete it.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification,
                    Subject = "%Store.Name%. New VAT number is submitted.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%User.FullName% (%User.Email%) has just submitted a new VAT number. Details are below:{0}<br />{0}VAT number: %User.VatNumber%{0}<br />{0}VAT number status: %User.VatNumberStatus%{0}<br />{0}Received name: %VatValidationResult.Name%{0}<br />{0}Received address: %VatValidationResult.Address%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderCancelledUserNotification,
                    Subject = "%Store.Name%. Your order cancelled",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%,{0}<br />{0}Your order has been cancelled. Below is the summary of the order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% %Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderCompletedUserNotification,
                    Subject = "%Store.Name%. Your order completed",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%,{0}<br />{0}Your order has been completed. Below is the summary of the order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% %Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentDeliveredUserNotification,
                    Subject = "Your order from %Store.Name% has been delivered.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%,{0}<br />{0}Good news! You order has been delivered.{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% Delivered Products:{0}<br />{0}<br />{0}%Shipment.Product(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedUserNotification,
                    Subject = "Order receipt from %Store.Name%.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%,{0}<br />{0}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% %Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification,
                    Subject = "%Store.Name%. Purchase Receipt for Order #%Order.OrderNumber%",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Order.UserFullName% (%Order.UserEmail%) has just placed an order from your store. Below is the summary of the order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% %Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentSentUserNotification,
                    Subject = "Your order from %Store.Name% has been shipped.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%!,{0}<br />{0}Good news! You order has been shipped.{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% Shipped Products:{0}<br />{0}<br />{0}%Shipment.Product(s)%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ProductReviewNotification,
                    Subject = "%Store.Name%. New product review.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}A new product review has been written for product \"%ProductReview.ProductName%\".{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Product.Name% (ID: %Product.ID%) low quantity.{0}<br />{0}<br />{0}Quantity: %Product.StockQuantity%{0}<br />{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%Product.Name% (ID: %Product.ID%) low quantity.{0}<br />{0}%AttributeCombination.Formatted%{0}<br />{0}Quantity: %AttributeCombination.StockQuantity%{0}<br />{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ReturnRequestStatusChangedUserNotification,
                    Subject = "%Store.Name%. Return request status was changed.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%,{0}<br />{0}Your return request #%ReturnRequest.CustomNumber% status has been changed.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.EmailAFriendMessage,
                    Subject = "%Store.Name%. Referred Item",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you.{0}<br />{0}<br />{0}<b><a target=\"_blank\" href=\"%Product.ProductURLForUser%\">%Product.Name%</a></b>{0}<br />{0}%Product.ShortDescription%{0}<br />{0}<br />{0}For more info click <a target=\"_blank\" href=\"%Product.ProductURLForUser%\">here</a>{0}<br />{0}<br />{0}<br />{0}%EmailAFriend.PersonalMessage%{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.WishlistToFriendMessage,
                    Subject = "%Store.Name%. Wishlist",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\"> %Store.Name%</a>{0}<br />{0}<br />{0}%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you.{0}<br />{0}<br />{0}<br />{0}For more info click <a target=\"_blank\" href=\"%Wishlist.URLForUser%\">here</a>{0}<br />{0}<br />{0}<br />{0}%Wishlist.PersonalMessage%{0}<br />{0}<br />{0}%Store.Name%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewOrderNoteAddedUserNotification,
                    Subject = "%Store.Name%. New order note has been added",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%,{0}<br />{0}New order note has been added to your account:{0}<br />{0}\"%Order.NewNoteText%\".{0}<br />{0}<a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%if (%RecurringPayment.CancelAfterFailedPayment%) The last payment for the recurring payment ID=%RecurringPayment.ID% failed, so it was cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) %User.FullName% (%User.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%. endif%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledUserNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%,{0}<br />{0}%if (%RecurringPayment.CancelAfterFailedPayment%) It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>){0}<br />{0}So your subscription has been canceled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) The recurring payment ID=%RecurringPayment.ID% was cancelled. endif%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentFailedUserNotification,
                    Subject = "%Store.Name%. Last recurring payment failed",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %User.FullName%,{0}<br />{0}It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>){0}<br /> %if (%RecurringPayment.RecurringPaymentType% == \"Manual\") {0}You can recharge balance and manually retry payment or cancel it on the order history page. endif% %if (%RecurringPayment.RecurringPaymentType% == \"Automatic\") {0}You can recharge balance and wait, we will try to make the payment again, or you can cancel it on the order history page. endif%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedVendorNotification,
                    Subject = "%Store.Name%. Order placed",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%User.FullName% (%User.Email%) has just placed an order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}%Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderRefundedUserNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%,{0}<br />{0}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.{0}<br />{0}<br />{0}Amount refunded: %Order.AmountRefunded%{0}<br />{0}<br />{0}Below is the summary of the order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br /{0}>Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% %Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = string.Format("%Store.Name%. Order #%Order.OrderNumber% refunded', N'{0}<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Order #%Order.OrderNumber% has been just refunded{0}<br />{0}<br />{0}Amount refunded: %Order.AmountRefunded%{0}<br />{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidStoreOwnerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Order #%Order.OrderNumber% has been just paid{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidUserNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Hello %Order.UserFullName%,{0}<br />{0}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been just paid. Below is the summary of the order.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}<br />{0}<br />{0}Billing Address{0}<br />{0}%Order.BillingFirstName% %Order.BillingLastName%{0}<br />{0}%Order.BillingAddress1%{0}<br />{0}%Order.BillingCity% %Order.BillingZipPostalCode%{0}<br />{0}%Order.BillingStateProvince% %Order.BillingCountry%{0}<br />{0}<br />{0}<br />{0}<br />{0}%if (%Order.Shippable%) Shipping Address{0}<br />{0}%Order.ShippingFirstName% %Order.ShippingLastName%{0}<br />{0}%Order.ShippingAddress1%{0}<br />{0}%Order.ShippingCity% %Order.ShippingZipPostalCode%{0}<br />{0}%Order.ShippingStateProvince% %Order.ShippingCountry%{0}<br />{0}<br />{0}Shipping Method: %Order.ShippingMethod%{0}<br />{0}<br />{0} endif% %Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidVendorNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Order #%Order.OrderNumber% has been just paid.{0}<br />{0}<br />{0}Order Number: %Order.OrderNumber%{0}<br />{0}Date Ordered: %Order.CreatedOn%{0}<br />{0}<br />{0}%Order.Product(s)%{0}</p>{0}", Environment.NewLine),
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification,
                    Subject = "%Store.Name%. New vendor account submitted.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}%User.FullName% (%User.Email%) has just submitted for a vendor account. Details are below:{0}<br />{0}Vendor name: %Vendor.Name%{0}<br />{0}Vendor email: %Vendor.Email%{0}<br />{0}<br />{0}You can activate it in admin area.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.VendorInformationChangeNotification,
                    Subject = "%Store.Name%. Vendor information change.",
                    Body = string.Format("<p>{0}<a href=\"%Store.URL%\">%Store.Name%</a>{0}<br />{0}<br />{0}Vendor %Vendor.Name% (%Vendor.Email%) has just changed information about itself.{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = string.Format("<p>{0}%ContactUs.Body%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactVendorMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = string.Format("<p>{0}%ContactUs.Body%{0}</p>{0}", Environment.NewLine),
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                }
            };
            _messageTemplateRepository.Insert(messageTemplates);
        }

        protected virtual void InstallTopics()
        {
            var defaultTopicTemplate =
                _topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template");
            if (defaultTopicTemplate == null)
                throw new Exception("Topic template cannot be loaded");

            var topics = new List<Topic>
                               {
                                   new Topic
                                       {
                                           SystemName = "AboutUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 20,
                                           Published = true,
                                           Title = "About us",
                                           Body = "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "CheckoutAsGuestOrRegister",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p><strong>Register and save time!</strong><br />Register with us for future convenience:</p><ul><li>Fast and easy check out</li><li>Easy access to your order history and status</li></ul>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ConditionsOfUse",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 15,
                                           Published = true,
                                           Title = "Conditions of Use",
                                           Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ContactUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ForumWelcomeMessage",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "Forums",
                                           Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "HomePageText",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "Welcome to our store",
                                           Body = "<p>Online shopping is the process consumers go through to purchase products or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://www.Seagullcommerce.com/documentation.aspx\">Documentation</a>, or post in the <a href=\"http://www.Seagullcommerce.com/boards/\">Forums</a> at <a href=\"http://www.Seagullcommerce.com\">SeagullCommerce.com</a></p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "LoginRegistrationInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "About login / registration",
                                           Body = "<p>Put your login / registration information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "PrivacyInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 10,
                                           Published = true,
                                           Title = "Privacy notice",
                                           Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "PageNotFound",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly, please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li></ul>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ShippingInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           IncludeInFooterColumn1 = true,
                                           DisplayOrder = 5,
                                           Published = true,
                                           Title = "Shipping & returns",
                                           Body = "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                                   new Topic
                                       {
                                           SystemName = "ApplyVendor",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           DisplayOrder = 1,
                                           Published = true,
                                           Title = "",
                                           Body = "<p>Put your apply vendor instructions here. You can edit this in the admin site.</p>",
                                           TopicTemplateId = defaultTopicTemplate.Id
                                       },
                               };
            _topicRepository.Insert(topics);


            //search engine names
            foreach (var topic in topics)
            {
                _urlRecordRepository.Insert(new UrlRecord
                {
                    EntityId = topic.Id,
                    EntityName = "Topic",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = topic.ValidateSeName("", !String.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName, true)
                });
            }

        }

        protected virtual void InstallSettings(bool installSampleData)
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new PdfSettings
            {
                LogoPictureId = 0,
                LetterPageSizeEnabled = false,
                RenderOrderNotes = true,
                FontFileName = "FreeSerif.ttf",
                InvoiceFooterTextColumn1 = null,
                InvoiceFooterTextColumn2 = null,
            });

            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProceduresIfSupported = true,
                UseStoredProcedureForLoadingCategories = false,
                SitemapEnabled = true,
                SitemapIncludeCategories = true,
                SitemapIncludeManufacturers = true,
                SitemapIncludeProducts = false,
                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge",
                BbcodeEditorOpenLinksInNewWindow = false
            });

            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                DefaultTitle = "Your store",
                DefaultMetaKeywords = "",
                DefaultMetaDescription = "",
                GenerateProductMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>
                {
                    "admin",
                    "install",
                    "recentlyviewedproducts",
                    "newproducts",
                    "compareproducts",
                    "clearcomparelist",
                    "setproductreviewhelpfulness",
                    "login",
                    "register",
                    "logout",
                    "cart",
                    "wishlist",
                    "emailwishlist",
                    "checkout",
                    "onepagecheckout",
                    "contactus",
                    "passwordrecovery",
                    "subscribenewsletter",
                    "blog",
                    "boards",
                    "inboxupdate",
                    "sentupdate",
                    "news",
                    "sitemap",
                    "search",
                    "config",
                    "eucookielawaccept",
                    "page-not-found",
                    //system names are not allowed (anyway they will cause a runtime error),
                    "con",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "null",
                    "prn",
                    "aux"
                },
                CustomHeadTags = ""
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 10,
                GridPageSizes = "10, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false,
                UseRichEditorInMessageTemplates = false,
                UseIsoDateTimeConverterInJson = true
            });

            settingService.SaveSetting(new LocalizationSettings
            {
                DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            settingService.SaveSetting(new UserSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = "SHA1",
                PasswordMinLength = 6,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowUsersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowUsersLocation = false,
                ShowUsersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewUserRegistration = false,
                UserNameFormat = UserNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                OnlineUserMinutes = 20,
                StoreLastVisitedPage = false,
                SuffixDeletedUsers = false,
                EnteringEmailTwice = false,
                RequireRegistrationForDownloadableProducts = false,
                DeleteGuestTaskOlderThanMinutes = 1440
            });

            settingService.SaveSetting(new AddressSettings
            {
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                PhoneEnabled = true,
                PhoneRequired = true,
                FaxEnabled = true,
            });

            settingService.SaveSetting(new MediaSettings
            {
                AvatarPictureSize = 120,
                ProductThumbPictureSize = 415,
                ProductDetailsPictureSize = 550,
                ProductThumbPictureSizeOnProductDetailsPage = 100,
                AssociatedProductPictureSize = 220,
                CategoryThumbPictureSize = 450,
                ManufacturerThumbPictureSize = 420,
                VendorThumbPictureSize = 450,
                CartThumbPictureSize = 80,
                MiniCartThumbPictureSize = 70,
                AutoCompleteSearchThumbPictureSize = 20,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                ImportProductImagesUsingHash = true,
                AzureCacheControlHeader = string.Empty
            });

            settingService.SaveSetting(new StoreInformationSettings
            {
                StoreClosed = false,
                DefaultStoreTheme = "DefaultClean",
                AllowUserToSelectTheme = false,
                DisplayMiniProfilerInPublicStore = false,
                DisplayMiniProfilerForAdminOnly = false,
                DisplayEuCookieLawWarning = false,
                FacebookLink = "http://www.facebook.com/SeagullCommerce",
                TwitterLink = "https://twitter.com/SeagullCommerce",
                YoutubeLink = "http://www.youtube.com/user/SeagullCommerce",
                GooglePlusLink = "https://plus.google.com/+Seagullcommerce",
                HidePoweredBySeagullCommerce = false
            });

            settingService.SaveSetting(new ExternalAuthenticationSettings
            {
                AutoRegisterEnabled = true,
                RequireEmailValidation = false
            });

            settingService.SaveSetting(new RewardPointsSettings
            {
                Enabled = true,
                ExchangeRate = 1,
                PointsForRegistration = 0,
                PointsForPurchases_Amount = 10,
                PointsForPurchases_Points = 1,
                ActivationDelay = 0,
                ActivationDelayPeriodId = 0,
                DisplayHowMuchWillBeEarned = true,
                PointsAccumulatedForAllStores = true,
                PageSize = 10
            });

            settingService.SaveSetting(new CurrencySettings
            {
                DisplayCurrencyLabel = false,
                PrimaryStoreCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                ActiveExchangeRateProviderSystemName = "CurrencyExchange.MoneyConverter",
                AutoUpdateEnabled = false
            });

            settingService.SaveSetting(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6",
            });

            settingService.SaveSetting(new SecuritySettings
            {
                ForceSslForAllPages = false,
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AdminAreaAllowedIpAddresses = null,
                EnableXsrfProtectionForAdminArea = true,
                EnableXsrfProtectionForPublicStore = true,
                HoneypotEnabled = false,
                HoneypotInputName = "hpinput"
            });




            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultStoreTimeZoneId = "",
                AllowUsersToSetTimeZone = false
            });

            

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            settingService.SaveSetting(new EmailAccountSettings
            {
                DefaultEmailAccountId = eaGeneral.Id
            });

            settingService.SaveSetting(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" },
            });

            settingService.SaveSetting(new DisplayDefaultMenuItemSettings
            {
                DisplayHomePageMenuItem = !installSampleData,
                DisplayNewProductsMenuItem = !installSampleData,
                DisplayProductSearchMenuItem = !installSampleData,
                DisplayUserInfoMenuItem = !installSampleData,
                DisplayBlogMenuItem = !installSampleData,
                DisplayForumsMenuItem = !installSampleData,
                DisplayContactUsMenuItem = !installSampleData
            });
        }






        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUser",
                    Enabled = true,
                    Name = "Add a new user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserAttribute",
                    Enabled = true,
                    Name = "Add a new user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserAttributeValue",
                    Enabled = true,
                    Name = "Add a new user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserRole",
                    Enabled = true,
                    Name = "Add a new user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewManufacturer",
                    Enabled = true,
                    Name = "Add a new manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProduct",
                    Enabled = true,
                    Name = "Add a new product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProductAttribute",
                    Enabled = true,
                    Name = "Add a new product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendor",
                    Enabled = true,
                    Name = "Add a new vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUser",
                    Enabled = true,
                    Name = "Delete a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserAttribute",
                    Enabled = true,
                    Name = "Delete a user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserAttributeValue",
                    Enabled = true,
                    Name = "Delete a user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserRole",
                    Enabled = true,
                    Name = "Delete a user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteManufacturer",
                    Enabled = true,
                    Name = "Delete a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteOrder",
                    Enabled = true,
                    Name = "Delete an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProduct",
                    Enabled = true,
                    Name = "Delete a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductAttribute",
                    Enabled = true,
                    Name = "Delete a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductReview",
                    Enabled = true,
                    Name = "Delete a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendor",
                    Enabled = true,
                    Name = "Delete a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUser",
                    Enabled = true,
                    Name = "Edit a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserAttribute",
                    Enabled = true,
                    Name = "Edit a user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserAttributeValue",
                    Enabled = true,
                    Name = "Edit a user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserRole",
                    Enabled = true,
                    Name = "Edit a user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditManufacturer",
                    Enabled = true,
                    Name = "Edit a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditOrder",
                    Enabled = true,
                    Name = "Edit an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProduct",
                    Enabled = true,
                    Name = "Edit a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductAttribute",
                    Enabled = true,
                    Name = "Edit a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductReview",
                    Enabled = true,
                    Name = "Edit a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendor",
                    Enabled = true,
                    Name = "Edit a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "User impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "User impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportProducts",
                    Enabled = true,
                    Name = "Products were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public store activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewManufacturer",
                    Enabled = false,
                    Name = "Public store. View a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewProduct",
                    Enabled = false,
                    Name = "Public store. View a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.PlaceOrder",
                    Enabled = false,
                    Name = "Public store. Place an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToCompareList",
                    Enabled = false,
                    Name = "Public store. Add to compare list"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public store. Add to shopping cart"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToWishlist",
                    Enabled = false,
                    Name = "Public store. Add to wishlist"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Login",
                    Enabled = false,
                    Name = "Public store. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddProductReview",
                    Enabled = false,
                    Name = "Public store. Add product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                }
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallTopicTemplates()
        {
            var topicTemplates = new List<TopicTemplate>
                               {
                                   new TopicTemplate
                                       {
                                           Name = "Default template",
                                           ViewPath = "TopicDetails",
                                           DisplayOrder = 1
                                       },
                               };
            _topicTemplateRepository.Insert(topicTemplates);
        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Seagull.Services.Messages.QueuedMessagesSendTask, Seagull.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Seagull.Services.Common.KeepAliveTask, Seagull.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Seagull.Services.Users.DeleteGuestsTask, Seagull.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Seagull.Services.Caching.ClearCacheTask, Seagull.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Seagull.Services.Logging.ClearLogTask, Seagull.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Update currency exchange rates",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Seagull.Services.Directory.UpdateExchangeRateTask, Seagull.Services",
                    Enabled = true,
                    StopOnError = false,
                },
            };

            _scheduleTaskRepository.Insert(tasks);
        }


        protected virtual void InstallWarehouses()
        {
            var warehouse1address = new Address
            {
                Address1 = "21 West 52nd Street",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            _addressRepository.Insert(warehouse1address);
            var warehouse2address = new Address
            {
                Address1 = "300 South Spring Stree",
                City = "Los Angeles",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "90013",
                CreatedOnUtc = DateTime.UtcNow,
            };
            _addressRepository.Insert(warehouse2address);

        }

        



        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallStores();
            InstallLanguages();
            InstallCurrencies();
            InstallCountriesAndStates();
            InstallUsersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings(installSampleData);
            InstallTopicTemplates();
            InstallTopics();
            InstallLocaleResources();
            InstallActivityLogTypes();
            InstallScheduleTasks();

            if (installSampleData)
            {
                InstallWarehouses();
                InstallActivityLog(defaultUserEmail);
                InstallSearchTerms();
            }
        }

        #endregion
    }
}