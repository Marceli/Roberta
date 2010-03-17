using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class WievStateTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        

    }
    protected override void  OnPreRender(EventArgs e)
    {
 	    base.OnPreRender(e);
        WniosekView.InitData<Vulcan.Stypendia.Bll.Wniosek>(null, DetailsViewMode.Insert);
        WniosekView.DataBind();
    }
  
    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
    }
}
