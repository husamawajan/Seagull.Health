using System;
using System.Linq;
using Seagull.Core.Data;
using Seagull;
using Seagull.Services.Localization;


namespace Seagull.Web.Framework.CommonHelperServices
{
    /// <summary>
    ///  SectorGoalsKpiProgresTarget service
    /// </summary>
    public partial class CommonHelperService : ICommonHelperService
    {

        #region Fields
        private readonly ILocalizationService _localizationService;
      
        #endregion

        #region Constructors

        public CommonHelperService(ILocalizationService localizationService )
        {
            this._localizationService = localizationService;
            
        }
        #endregion

        public string GetOperation(int OPId)
        {
            string Op = string.Empty;

            switch (OPId)
            {
                case 2:
                    Op = "+";
                    break;
                case 3:
                    Op = "-";
                    break;
                case 4:
                    Op = "/";
                    break;
                case 5:
                    Op = "*";
                    break;
                case 6:
                    Op = "*100";
                    break;
                case 7:
                    Op = "/100";
                    break;
                default:
                    Op = string.Empty;
                    break;
            }
            return Op;
        }
        public string GetKpiType(int KpiTypeId)
        {

            string KpiType = string.Empty;
            switch (KpiTypeId)
            {

                case 1:
                    KpiType = _localizationService.GetResource("Admin.Output");
                    break;
                case 2:
                    KpiType = _localizationService.GetResource("Admin.OutCome");
                    break;
                case 3:
                    KpiType = _localizationService.GetResource("Admin.efficiency");
                    break;
                case 4:
                    KpiType = _localizationService.GetResource("Admin.througput");
                    break;
                case 5:
                    KpiType = _localizationService.GetResource("Admin.Impact");
                    break;
                case 6:
                    KpiType = _localizationService.GetResource("Admin.General");
                    break;

            }
            return KpiType;

        }
        public int GetMonthQuarter(int Month) 
        {
            switch (Month)
            {
                case 2:
                case 3:
                case 5:
                case 6:
                case 8:
                case 9:
                case 11:
                case 12:
                    return -1;
                case 4:
                    return 1;
                case 7:
                    return 2;
                case 10:
                    return 3;
                default :
                    return 4;
            }
        }

        public string GetQuarterNamebyMonth(int Month)
        {
            switch (Month)
            {
                case 1:
                case 2:
                case 3:
                    return _localizationService.GetResource("Admin.Q1");
                case 4:
                case 5:
                case 6:
                    return _localizationService.GetResource("Admin.Q2");
                case 7:
                case 8:
                case 9:
                    return _localizationService.GetResource("Admin.Q3");
                default:
                    return _localizationService.GetResource("Admin.Q4");
            }
        }
        public string GetQuarterName(int Q)
        {
            switch (Q)
            {
                case 1:
                    return _localizationService.GetResource("Admin.Q1");
                case 2:
                    return _localizationService.GetResource("Admin.Q2");
                case 3:
                    return _localizationService.GetResource("Admin.Q3");
                default:
                    return _localizationService.GetResource("Admin.Q4");
            }
        }
        public string GetQTypeName(int Q)
        {
            switch (Q)
            {
                case 1:
                    return "checkbox";
                case 2:
                    return "currency";
                case 3:
                    return "number";
                default:
                    return "text";
            }
        }
        public string GetFixedTariffQTypeName(int Q)
        {
            switch (Q)
            {
                case 1:
                    return "number";
                case 2:
                    return "date";
                default:
                    return "text";
            }
        }

    }
}
	
