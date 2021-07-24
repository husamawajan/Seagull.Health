using Seagull.Services.Localization;
using Seagull.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seagull.Services.Security;
using Seagull.Core;
using Seagull.Core.Data;
using System.Data.SqlClient;
using Seagull.Data;
using Seagull.Services.Helpers;
using Seagull.Admin.Helpers;
using System.Dynamic;
using Seagull.Services.UserTypes;
using Seagull.Services.UserEntitys;
using System.Web.Helpers;
using Seagull.Services.Functionss;
using static Utilities.Constants;
using Newtonsoft.Json.Linq;
using Microsoft.Ajax.Utilities;

namespace Seagull.Admin.Controllers
{
    public class GenericGetsController : Controller
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly IUserTypeService _userTypeService;
        private readonly IUserEntityService _userEntityService;
        private readonly IFunctionsService _functionsService;
        private readonly IDbContext _iDbContext;

        #endregion

        #region Constructors
        public GenericGetsController(
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IUserService userService,
            IUserTypeService userTypeService,
            IUserEntityService userEntityService,
            IFunctionsService functionsService,
            IDbContext iDbContext)
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._userService = userService;
            _userTypeService = userTypeService;
            _userEntityService = userEntityService;
            this._functionsService = functionsService;
            _iDbContext = iDbContext;
        }
        #endregion

        // GET: GenericGets
        #region Select2 Lookups
        #region Class For select2
        public class SelectedSelect2
        {
            public string id { get; set; }
            public string text { get; set; }
        }
        public class Select2PagedResult
        {
            public int Total { get; set; }
            public List<Select2Result> Results { get; set; }
        }
        #endregion

        #region Class For select2 result
        public class Select2Result
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        #endregion

        #region Shared Methods
        private Select2PagedResult Select2Format(List<SelectedSelect2> Objects, int total)
        {
            Select2PagedResult jsonObject = new Select2PagedResult();
            jsonObject.Results = new List<Select2Result>();
            jsonObject.Results = (from a in Objects
                                  select new Select2Result
                                  {
                                      id = a.id,
                                      text = a.text
                                  }).OrderBy(a => a.id).ToList();
            //Set the total count of the results from the query.
            jsonObject.Total = total;
            return jsonObject;
        }
        #endregion

        #endregion

        #region class for AngularDropDown

        public JsonResultHelper ReturnData(List<dynamic> listData)
        {
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.data = listData.OrderBy(a => a.Id).ToList();
            result.success = true;
            return result;
        }
        #endregion

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        #region GetAllUserRoles
        public ActionResult GetAllUserRoles(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            var UserRole = _userService.GetAllUserRoles(pagination, sort, search, search_operator, filter, showHidden);
            var data = (from a in UserRole
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).Cast<object>().ToList();
            return Json(ReturnData(data), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetAllFunctions
        public ActionResult GetAllFunctions(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = true)
        {
            var Functions = _functionsService.GetAllFunctionss(pagination, sort, search, search_operator, filter, showHidden);
            var data = (from a in Functions
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).Cast<object>().ToList();
            return Json(ReturnData(data), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetAllUsers
        public ActionResult GetAllUsers(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            var User = _userService.GetAllUsers(pagination, sort, search, search_operator, filter);
            var data = (from a in User
                        select new
                        {
                            Id = a.Id,
                            Name = a.GetFullName()
                        }).Cast<object>().ToList();
            return Json(ReturnData(data), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Methods
        public class CustomList
        {
            public int id { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }
}