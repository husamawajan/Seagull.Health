//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using System.Web;
using Seagull.Core.Infrastructure;

namespace Seagull.Services.Authentication.External
{
    /// <summary>
    /// External authorizer helper
    /// </summary>
    public static partial class ExternalAuthorizerHelper
    {
        private static HttpSessionStateBase GetSession()
        {
            var session = EngineContext.Current.Resolve<HttpSessionStateBase>();
            return session;
        }

        public static void StoreParametersForRoundTrip(OpenAuthenticationParameters parameters)
        {
            var session = GetSession();
            session["Seagull.externalauth.parameters"] = parameters;
        }
        public static OpenAuthenticationParameters RetrieveParametersFromRoundTrip(bool removeOnRetrieval)
        {
            var session = GetSession();
            var parameters = session["Seagull.externalauth.parameters"];
            if (parameters != null && removeOnRetrieval)
                RemoveParameters();

            return parameters as OpenAuthenticationParameters;
        }

        public static void RemoveParameters()
        {
            var session = GetSession();
            session.Remove("Seagull.externalauth.parameters");
        }

        public static void AddErrorsToDisplay(string error)
        {
            var session = GetSession();
            var errors = session["Seagull.externalauth.errors"] as IList<string>;
            if (errors == null)
            {
                errors = new List<string>();
                session.Add("Seagull.externalauth.errors", errors);
            }
            errors.Add(error);
        }

        public static IList<string> RetrieveErrorsToDisplay(bool removeOnRetrieval)
        {
            var session = GetSession();
            var errors = session["Seagull.externalauth.errors"] as IList<string>;
            if (errors != null && removeOnRetrieval)
                session.Remove("Seagull.externalauth.errors");
            return errors;
        }
    }
}