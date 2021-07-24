using System;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Models.Tasks;
using Seagull.Core.Domain.Tasks;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Security;
using Seagull.Services.Tasks;
using Seagull.Web.Framework.Kendoui;
using Seagull.Web.Framework.Mvc;
using Seagull.Admin.Helpers;

namespace Seagull.Admin.Controllers
{
    public partial class ScheduleTaskController : BaseAdminController
	{
		#region Fields

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IUserActivityService _userActivityService;

        #endregion

        #region Constructors

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService, 
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IUserActivityService userActivityService)
        {
            this._scheduleTaskService = scheduleTaskService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._userActivityService = userActivityService;
        }

		#endregion 

        #region Utility

        [NonAction]
        protected virtual ScheduleTaskModel PrepareScheduleTaskModel(ScheduleTask task)
        {
            var model = new ScheduleTaskModel
                            {
                                Id = task.Id,
                                Name = task.Name,
                                Seconds = task.Seconds,
                                Enabled = task.Enabled,
                                StopOnError = task.StopOnError,
                                LastStartUtc = task.LastStartUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(task.LastStartUtc.Value, DateTimeKind.Utc).ToString("G") : "",
                                LastEndUtc = task.LastEndUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(task.LastEndUtc.Value, DateTimeKind.Utc).ToString("G") : "",
                                LastSuccessUtc = task.LastSuccessUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(task.LastSuccessUtc.Value, DateTimeKind.Utc).ToString("G") : "",
                            };
            return model;
        }

        #endregion

        #region Methods

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            return View();
		}

		[HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedKendoGridJson();

            var models = _scheduleTaskService.GetAllTasks(true)
                .Select(PrepareScheduleTaskModel)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = models,
                Total = models.Count
            };

            return Json(gridModel);
		}

        [HttpPost]
        public virtual ActionResult TaskUpdate(ScheduleTaskModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var scheduleTask = _scheduleTaskService.GetTaskById(model.Id);
            if (scheduleTask == null)
                return Content("Schedule task cannot be loaded");

            scheduleTask.Name = model.Name;
            scheduleTask.Seconds = model.Seconds;
            scheduleTask.Enabled = model.Enabled;
            scheduleTask.StopOnError = model.StopOnError;
            _scheduleTaskService.UpdateTask(scheduleTask);

            //activity log
            _userActivityService.InsertActivity("EditTask", _localizationService.GetResource("ActivityLog.EditTask"), scheduleTask.Id);

            return new NullJsonResult();
        }

        public virtual ActionResult RunNow(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            try
            {
                var scheduleTask = _scheduleTaskService.GetTaskById(id);
                if (scheduleTask == null)
                    throw new Exception("Schedule task cannot be loaded");

                var task = new Task(scheduleTask);
                //ensure that the task is enabled
                task.Enabled = true;
                //do not dispose. otherwise, we can get exception that DbContext is disposed
                task.Execute(true, false, false);
                SuccessNotification(_localizationService.GetResource("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }
        [HttpPost]
        public ActionResult RunPriorityNotifyNow()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedJson();

            JsonResultHelper data = new JsonResultHelper{Access = true,success = false};

            try
            {
                var scheduleTask = _scheduleTaskService.GetTaskById(7);
                if (scheduleTask == null)
                {
                    data.Msg.Add("No Schedule Tasks");
                    return Json(data);
                }
                    

                var task = new Task(scheduleTask);
                //ensure that the task is enabled
                task.Enabled = true;
                //do not dispose. otherwise, we can get exception that DbContext is disposed
                task.Execute(true, false, false,true);
            }
            catch (Exception exc)
            {
                data.Msg.Add(exc.InnerException.ToString());
                return Json(data);
            }
            data.success = !data.success;
            return Json(data);
        }
        #endregion
    }
}
