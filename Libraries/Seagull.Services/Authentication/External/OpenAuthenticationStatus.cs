//Contributor:  Nicholas Mayne


namespace Seagull.Services.Authentication.External
{
    /// <summary>
    /// Open authentication status
    /// </summary>
    public enum OpenAuthenticationStatus
    {
        Unknown,
        Error,
        Authenticated,
        RequiresRedirect,
        AssociateOnLogon,
        AutoRegisteredEmailValidation,
        AutoRegisteredAdminApproval,
        AutoRegisteredStandard,
    }
}