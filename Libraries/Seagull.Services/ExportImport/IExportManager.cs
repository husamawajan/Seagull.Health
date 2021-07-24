using System.Collections.Generic;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Messages;

namespace Seagull.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {



        /// <summary>
        /// Export category list to xml
        /// </summary>
        /// <returns>Result in XML format</returns>
        string ExportCategoriesToXml();



        /// <summary>
        /// Export user list to XLSX
        /// </summary>
        /// <param name="users">Users</param>
        byte[] ExportUsersToXlsx(IList<User> users);

        /// <summary>
        /// Export user list to xml
        /// </summary>
        /// <param name="users">Users</param>
        /// <returns>Result in XML format</returns>
        string ExportUsersToXml(IList<User> users);


        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportStatesToTxt(IList<StateProvince> states);
    }
}
