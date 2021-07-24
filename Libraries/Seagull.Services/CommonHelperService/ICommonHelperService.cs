using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace Seagull.Services.CommonHelperServices
{

    public partial interface ICommonHelperService
	{
        string GetOperation(int OPId);
        string GetKpiType(int KpiTypeId);
        int GetMonthQuarter(int Month);
        string GetQuarterNamebyMonth(int Month);
        string GetQuarterName(int Month);
        string GetQTypeName(int Q);
        string GetFixedTariffQTypeName(int Q);

    }
}
	
 
 