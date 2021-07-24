
using System.IO;

namespace Seagull.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {



        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        int ImportStatesFromTxt(Stream stream);




    }
}
