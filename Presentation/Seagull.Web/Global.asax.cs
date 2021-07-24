using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Mvc;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain;
using Seagull.Core.Domain.Common;
using Seagull.Core.Infrastructure;
using Seagull.Services.Logging;
using Seagull.Services.Tasks;
using Seagull.Web.Controllers;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Framework.Mvc.Routes;
using Seagull.Web.Framework.Themes;
using StackExchange.Profiling;
using StackExchange.Profiling.Mvc;
using System.Web.Optimization;
using System.Web.Configuration;
using System.Configuration;
using System.Net.Mail;
using Seagull.Core.Domain.Emails;
using Seagull.Services.Emails;
using System.Data.SqlClient;
using System.Data;

namespace Seagull.Web
{
    public class MvcApplication : HttpApplication
    {

        public MvcApplication()
        {

        }
        private static long maxRequestLength = 0;
        /// <summary>
        /// Returns the max size of a request, in kb
        /// </summary>
        /// <returns></returns>
        private long getMaxRequestLength()
        {
            long requestLength = 4096; // assume default value
            HttpRuntimeSection runTime = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection; // check web.config
            if (runTime != null)
            {
                requestLength = runTime.MaxRequestLength;
            }
            else
            {
                // not found...check machine.config
                Configuration cfg = ConfigurationManager.OpenMachineConfiguration();
                ConfigurationSection cs = cfg.SectionGroups["system.web"].Sections["httpRuntime"];
                if (cs != null)
                {
                    requestLength = Convert.ToInt64(cs.ElementInformation.Properties["maxRequestLength"].Value);
                }
            }
            return requestLength;
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "User", action = "Login", id = UrlParameter.Optional },
                new[] { "Seagull.Web.Controllers" }
            );
            //routePublisher.RegisterRoutes(routes);

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    new[] { "Seagull.Web.Controllers" }
            //);

        }
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Admnistration/Scripts/angular.js"));
            BundleTable.EnableOptimizations = true;
        }

        protected void Application_Start()
        {
            // set the maxRequestLength for json post
            maxRequestLength = getMaxRequestLength();

            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;



            //disable "X-AspNetMvc-Version" header name
            MvcHandler.DisableMvcResponseHeader = true;

            //initialize engine context
            EngineContext.Initialize(false);

            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            if (databaseInstalled)
            {
                //remove all view engines
                ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new SeagullMetadataProvider();

            //Registering some regular mvc stuff
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            //Husam Awajan Seagull - Angular 13-04-2016
            RegisterBundles(BundleTable.Bundles);
            //fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new SeagullValidatorFactory()));

            if (databaseInstalled)
            {
                //start scheduled tasks
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();

                //miniprofiler
                if (EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
                {
                    GlobalFilters.Filters.Add(new ProfilingActionFilter());
                }

                //log application start
                try
                {
                    //log
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Information("Application started", null, null);
                }
                catch (Exception)
                {
                    //don't throw new exception if occurs
                }
            }
            //Thread thread = new Thread(new ThreadStart(MaintaingSendEmailThread));
            //thread.IsBackground = true;
            //thread.Start();
            
        }

        //#region Maintaing Send Email Thread
        //public void MaintaingSendEmailThread()
        //{
        //    int _24Hours = 86400000;
        //    while (true)
        //    {
        //        SendEmail();
        //        Thread.Sleep(_24Hours);
        //    }
        //}
        //#endregion

        //#region Send Email
        //public void SendEmail()
        //{
        //    //Thread thread;
        //    string subject = string.Empty;
        //    string body = string.Empty;
        //    int day = DateTime.Now.Day;
        //    int month = DateTime.Now.Month;
        //    bool flag = false;
        //    int EmailType = 0;

        //    if (day == 1 && month == 4)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "اشارة الى نص المادة (29/ ب ) من قانون الاتصالات رقم 13 لسنة 1995 وتعديلاته، وإلى اتفاقية الترخيص الموقعه مع شركتكم، وحتى تتمكن الهيئة من مراقبة التطور في قطاع الاتصالات تدعوكم الهيئة لتزويدها بالبيانات الربعية الخاصة بشركتكم عن الربع الاول من خلال الدخول على الرابط التالي ،وتؤكد الهيئة على أن الموعد النهائي هو (15/4) مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 1;
        //    }
        //    else if ((day == 8 || day == 13) && month == 4)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "إشارة الى طلب تزويد البيانات الربعية ، للتكرم بالعلم بأنه لغاية تاريخة لم تقم شركتكم بتزويد الهيئة بما هو مطلوب وتذكركم بأن الموعدالنهائي هو 15/4.  مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 2;
        //    }
        //    else if (day == 1 && month == 7)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "اشارة الى نص المادة (29/ ب ) من قانون الاتصالات رقم 13 لسنة 1995 وتعديلاته، وإلى اتفاقية الترخيص الموقعه مع شركتكم، وحتى تتمكن الهيئة من مراقبة التطور في قطاع الاتصالات تدعوكم الهيئة لتزويدها بالبيانات الربعية الخاصة بشركتكم عن الربع الثاني من خلال الدخول على الرابط التالي ،وتؤكد الهيئة على أن الموعد النهائي هو (15/7)مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 1;
        //    }
        //    else if ((day == 8 || day == 13) && month == 7)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "إشارة الى طلب تزويد البيانات الربعية ، للتكرم بالعلم بأنه لغاية تاريخة لم تقم شركتكم بتزويد الهيئة بما هو مطلوب وتذكركم بأن الموعدالنهائي هو 15/7.  مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 2;
        //    }
        //    else if (day == 1 && month == 10)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "اشارة الى نص المادة (29/ ب ) من قانون الاتصالات رقم 13 لسنة 1995 وتعديلاته، وإلى اتفاقية الترخيص الموقعه مع شركتكم، وحتى تتمكن الهيئة من مراقبة التطور في قطاع الاتصالات تدعوكم الهيئة لتزويدها بالبيانات الربعية الخاصة بشركتكم عن الربع الثالث من خلال الدخول على الرابط التالي ،وتؤكد الهيئة على أن الموعد النهائي هو (15/10)مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 1;
        //    }
        //    else if ((day == 8 || day == 13) && month == 10)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "إشارة الى طلب تزويد البيانات الربعية ، للتكرم بالعلم بأنه لغاية تاريخة لم تقم شركتكم بتزويد الهيئة بما هو مطلوب وتذكركم بأن الموعدالنهائي هو 15/10.  مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 2;
        //    }
        //    else if (day == 1 && month == 1)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "اشارة الى نص المادة (29/ ب ) من قانون الاتصالات رقم 13 لسنة 1995 وتعديلاته، وإلى اتفاقية الترخيص الموقعه مع شركتكم، وحتى تتمكن الهيئة من مراقبة التطور في قطاع الاتصالات تدعوكم الهيئة لتزويدها بالبيانات الربعية الخاصة بشركتكم عن الربع الرابع من خلال الدخول على الرابط التالي ،وتؤكد الهيئة على أن الموعد النهائي هو (15/1)مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 1;
        //    }
        //    else if ((day == 8 || day == 13) && month == 1)
        //    {
        //        subject = "البيانات الربعية";
        //        body = "إشارة الى طلب تزويد البيانات الربعية ، للتكرم بالعلم بأنه لغاية تاريخة لم تقم شركتكم بتزويد الهيئة بما هو مطلوب وتذكركم بأن الموعدالنهائي هو 15/1.  مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 2;
        //    }
        //    else if (day == 15 && month == 2)
        //    {
        //        subject = "البيانات السنوية";
        //        body = "اشارة الى نص المادة (29/ ب ) من قانون الاتصالات رقم 13 لسنة 1995 وتعديلاته، وإلى اتفاقية الترخيص الموقعه مع شركتكم، وحتى تتمكن الهيئة من مراقبة التطور في قطاع الاتصالات تدعوكم الهيئة لتزويدها بالبيانات السنوية الخاصة بشركتكم من خلال الدخول على الرابط التالي ،وتؤكد الهيئة على أن الموعد النهائي هو (31/5) مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 1;
        //    }
        //    else if ((day == 28 && month == 2) || ((day == 1 || day == 14 || day == 30) && (month == 3 || month == 4 || month == 5)))
        //    {
        //        subject = "البيانات السنوية";
        //        body = "إشارة الى طلب تزويد البيانات السنوية ، للتكرم بالعلم بأنه لغاية تاريخة لم تقم شركتكم بتزويد الهيئة بما هو مطلوب وتذكركم بأن الموعدالنهائي هو 31/5. مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 2;
        //    }
        //    else if (day == 15 && month == 3)
        //    {
        //        subject = "البيانات المالية";
        //        body = "اشارة الى نص المادة (29/ ب ) من قانون الاتصالات رقم 13 لسنة 1995 وتعديلاته، وإلى اتفاقية الترخيص الموقعه مع شركتكم، وحتى تتمكن الهيئة من مراقبة التطور في قطاع الاتصالات تدعوكم الهيئة لتزويدها بالبيانات المالية الخاصة بشركتكم من خلال الدخول على الرابط التالي ،وتؤكد الهيئة على أن الموعد النهائي هو (31/5) مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 1;
        //    }
        //    else if ((day == 29 && month == 3) || ((day == 2 || day == 15 || day == 29) && (month == 4 || month == 5)))
        //    {
        //        subject = "البيانات المالية";
        //        body = "إشارة الى طلب تزويد البيانات المالية ، للتكرم بالعلم بأنه لغاية تاريخة لم تقم شركتكم بتزويد الهيئة بما هو مطلوب وتذكركم بأن الموعدالنهائي هو 31/5. مع فائق الاحترام والتقدير";
        //        flag = true;
        //        EmailType = 2;
        //    }

        //    if (flag)
        //    {
        //        var fromAddress = new MailAddress("seagulltechnology7@gmail.com", "seagulltechnology");
        //        //var toAddress = new MailAddress("n.dabbas@seagull-technology.com", "Nour Dabbas");
        //        var toAddress = new MailAddress("a.alzoubi@seagull-technology.com", "Ahmad");
        //        const string fromPassword = "Lfrm1995";

        //        var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //        };
        //        using (var message = new MailMessage(fromAddress, toAddress)
        //        {
        //            Subject = subject,
        //            Body = body
        //        })
        //        {
        //            smtp.Send(message);
        //        }

        //        Email email = new Email
        //        {
        //            From = fromAddress.Address,
        //            To = toAddress.Address,
        //            Subject = subject,
        //            Msg = body,
        //            EmailDate = DateTime.Now,
        //            OperatorId = EngineContext.Current.Resolve<IWorkContext>().CurrentUser.UserOperatorId,
        //            Type = EmailType,
        //            Flag = true
        //        };

        //        addToDataBase(email);
        //    }
        //}
        //#endregion

        //#region Add To Database
        //private void addToDataBase(Email email)
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Settings.txt";
        //    string[] lines = System.IO.File.ReadAllLines(path);
        //    string connectionString = lines[1].Split(':')[1].Trim();

        //    string query = "INSERT INTO [dbo].[Email]([From],[To],[Subject],[Msg],[EmailDate],[OperatorId],[Type],[Flag])" +
        //        "VALUES(@From,@To,@Subject,@Msg,@EmailDate,@OperatorId,@Type,@Flag)";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.CommandText = query;
        //            SqlParameter Param = new SqlParameter("@From", SqlDbType.Text, 50);
        //            Param.Value = email.From;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@To", SqlDbType.Text, 50);
        //            Param.Value = email.To;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@Subject", SqlDbType.NText, 50);
        //            Param.Value = email.Subject;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@Msg", SqlDbType.NText, 50);
        //            Param.Value = email.Msg;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@EmailDate", SqlDbType.DateTime, 250);
        //            Param.Value = email.EmailDate;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@OperatorId", SqlDbType.Int, 50);
        //            Param.Value = email.OperatorId;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@Type", SqlDbType.Int, 50);
        //            Param.Value = email.Type;
        //            command.Parameters.Add(Param);

        //            Param = new SqlParameter("@Flag", SqlDbType.Int, 50);
        //            Param.Value = email.Flag;
        //            command.Parameters.Add(Param);
        //            connection.Open();
        //            // Call Prepare after setting the Commandtext and Parameters.
        //            command.Prepare();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}
        //#endregion

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            #region Max Request Length For Json Post
            long requestLength = HttpContext.Current.Request.ContentLength / 1024; // returns the request length in bytes, then converted to kb
            if (requestLength > maxRequestLength)
            {
                IServiceProvider provider = (IServiceProvider)HttpContext.Current;
                HttpWorkerRequest workerRequest = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));

                // Check if body contains data
                if (workerRequest.HasEntityBody())
                {
                    // get the total body length
                    int bodyLength = workerRequest.GetTotalEntityBodyLength();
                    // Get the initial bytes loaded
                    int initialBytes = 0;
                    if (workerRequest.GetPreloadedEntityBody() != null)
                    {
                        initialBytes = workerRequest.GetPreloadedEntityBody().Length;
                    }
                    if (!workerRequest.IsEntireEntityBodyIsPreloaded())
                    {
                        byte[] buffer = new byte[512000];
                        // Set the received bytes to initial bytes before start reading
                        int receivedBytes = initialBytes;
                        while (bodyLength - receivedBytes >= initialBytes)
                        {
                            // Read another set of bytes
                            initialBytes = workerRequest.ReadEntityBody(buffer, buffer.Length);
                            // Update the received bytes
                            receivedBytes += initialBytes;
                        }
                        initialBytes = workerRequest.ReadEntityBody(buffer, bodyLength - receivedBytes);
                    }
                }

                try
                {
                    throw new HttpException("Request too large");
                }
                catch { }

                // Redirect the user
                Server.Transfer("~/ApplicationError.aspx", false);
            }
            #endregion
            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            //keep alive page requested (we ignore it to prevent creating a guest user records)
            string keepAliveUrl = string.Format("{0}keepalive/index", webHelper.GetStoreLocation());
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                return;

            //ensure database is installed
            if (!DataSettingsHelper.DatabaseIsInstalled())
            {
                string installUrl = string.Format("{0}install", webHelper.GetStoreLocation());
                if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Response.Redirect(installUrl);
                }
            }

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //miniprofiler
            if (EngineContext.Current.Resolve<StoreInformationSettings>().DisplayMiniProfilerInPublicStore)
            {
                MiniProfiler.Start();
                //store a value indicating whether profiler was started
                HttpContext.Current.Items["Seagull.MiniProfilerStarted"] = true;
            }


        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //miniprofiler
            var miniProfilerStarted = HttpContext.Current.Items.Contains("Seagull.MiniProfilerStarted") &&
                 Convert.ToBoolean(HttpContext.Current.Items["Seagull.MiniProfilerStarted"]);
            if (miniProfilerStarted)
            {
                MiniProfiler.Stop();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //we don't do it in Application_BeginRequest because a user is not authenticated yet
            SetWorkingCulture();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            //log error
            LogException(exception);

            //process 404 HTTP errors
            var httpException = exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                if (!webHelper.IsStaticResource(this.Request))
                {
                    Response.Clear();
                    Server.ClearError();
                    Response.TrySkipIisCustomErrors = true;

                    // Call target Controller and pass the routeData.
                    IController errorController = EngineContext.Current.Resolve<CommonController>();

                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Common");
                    routeData.Values.Add("action", "PageNotFound");

                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }
            }
        }

        protected void SetWorkingCulture()
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore static resources
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            //keep alive page requested (we ignore it to prevent creation of guest user records)
            string keepAliveUrl = string.Format("{0}keepalive/index", webHelper.GetStoreLocation());
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                return;


            if (webHelper.GetThisPageUrl(false).StartsWith(string.Format("{0}admin", webHelper.GetStoreLocation()),
                StringComparison.InvariantCultureIgnoreCase))
            {
                //admin area


                //always set culture to 'en-US'
                //we set culture of admin area to 'en-US' because current implementation of Telerik grid 
                //doesn't work well in other cultures
                //e.g., editing decimal value in russian culture
                CommonHelper.SetTelerikCulture();
            }
            else
            {
                //public store
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore 404 HTTP errors
            var httpException = exc as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404 &&
                !EngineContext.Current.Resolve<CommonSettings>().Log404Errors)
                return;

            try
            {
                //log
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentUser);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}