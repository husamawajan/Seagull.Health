using System;
using System.Collections.Generic;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Media;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Media;
using Seagull.Services.Seo;
using Seagull.Web.Framework;
using Seagull.Web.Models.Common;
using Seagull.Web.Models.Profile;

namespace Seagull.Web.Factories
{
    /// <summary>
    /// Represents the profile model factory
    /// </summary>
    public partial class ProfileModelFactory : IProfileModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICountryService _countryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly UserSettings _userSettings;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ProfileModelFactory(
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICountryService countryService,
            IDateTimeHelper dateTimeHelper,
            UserSettings userSettings,
            MediaSettings mediaSettings)
        {
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._countryService = countryService;
            this._dateTimeHelper = dateTimeHelper;
            this._userSettings = userSettings;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>Profile index model</returns>
        public virtual ProfileIndexModel PrepareProfileIndexModel(User user, int? page)
        {
            if (user == null)
                throw  new ArgumentNullException("user");

            bool pagingPosts = false;
            int postsPage = 0;

            if (page.HasValue)
            {
                postsPage = page.Value;
                pagingPosts = true;
            }

            var name = user.FormatUserName();
            var title = string.Format(_localizationService.GetResource("Profile.ProfileOf"), name);

            var model = new ProfileIndexModel
            {
                ProfileTitle = title,
                PostsPage = postsPage,
                PagingPosts = pagingPosts,
                UserProfileId = user.Id,
            };
            return model;
        }

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Profile info model</returns>
        public virtual ProfileInfoModel PrepareProfileInfoModel(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            //avatar
            var avatarUrl = "";
            if (_userSettings.AllowUsersToUploadAvatars)
            {
                avatarUrl =_pictureService.GetPictureUrl(
                 user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                 _mediaSettings.AvatarPictureSize,
                 _userSettings.DefaultAvatarEnabled,
                 defaultPictureType: PictureType.Avatar);
            }

            //location
            bool locationEnabled = false;
            string location = string.Empty;
            if (_userSettings.ShowUsersLocation)
            {
                locationEnabled = true;

                var countryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                var country = _countryService.GetCountryById(countryId);
                if (country != null)
                {
                    location = country.GetLocalized(x => x.Name);
                }
                else
                {
                    locationEnabled = false;
                }
            }



            //total forum posts
            bool totalPostsEnabled = false;
            int totalPosts = 0;

            //registration date
            bool joinDateEnabled = false;
            string joinDate = string.Empty;

            if (_userSettings.ShowUsersJoinDate)
            {
                joinDateEnabled = true;
                joinDate = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
            }

            //birth date
            bool dateOfBirthEnabled = false;
            string dateOfBirth = string.Empty;
            if (_userSettings.DateOfBirthEnabled)
            {
                var dob = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
                if (dob.HasValue)
                {
                    dateOfBirthEnabled = true;
                    dateOfBirth = dob.Value.ToString("D");
                }
            }

            var model = new ProfileInfoModel
            {
                UserProfileId = user.Id,
                AvatarUrl = avatarUrl,
                LocationEnabled = locationEnabled,
                Location = location,
                TotalPostsEnabled = totalPostsEnabled,
                TotalPosts = totalPosts.ToString(),
                JoinDateEnabled = joinDateEnabled,
                JoinDate = joinDate,
                DateOfBirthEnabled = dateOfBirthEnabled,
                DateOfBirth = dateOfBirth,
            };

            return model;
        }

        #endregion
    }
}
