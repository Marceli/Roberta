using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for InputParametersHelper
/// </summary>
public class Ph
{
    public static T IsNull<T>(object o, T defVal)
    {
        return o != null ? (T)o : defVal;
    }

    public static string EmptyIfNull(object o)
    {
        return o != null ? (string)o : string.Empty;
    }
}
