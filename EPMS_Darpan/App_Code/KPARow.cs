using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for KPARow
/// </summary>

[Serializable]
public class KPARow
{
    public string KPAName { get; set; }
    public string UnitOfKPA { get; set; }

    public string CategoryValue { get; set; }
    public string CategoryText { get; set; }
    public string DatasourceValue { get; set; }
    public string DatasourceText { get; set; }
    public string Importance { get; set; }
    public string OtherSource { get; set; }
}

