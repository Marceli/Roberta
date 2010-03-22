using System;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Stypendia : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
//        if (!this.Page.ClientScript.IsStartupScriptRegistered(typeof(Stypendia), "treescript"))
//            this.Page.ClientScript.RegisterStartupScript(typeof(Stypendia), "treescript", string.Format("<script language=\"javascript\">var HIDDENLEFTCLIENTID='{0}';</script>", HiddenLeftWidth.ClientID));
    }

//    void InitializeMenuRoots()
//    {
//        // initialize menu
//        if (this.Content.Page is IMenuProvider)
//        {
//            // initialize tree root level
//            IMenuProvider prov = this.Content.Page as IMenuProvider;
//            if (prov != null)
//            {
//                TreeViewMenu.Nodes.Clear();
//                prov.ProvideRoots(TreeViewMenu);
//            }
//        }
//    }

    protected void MainMenu_MenuItemClick(object sender, MenuEventArgs e)
    {
        Response.Redirect(e.Item.Value);
    }
//    protected void TreeViewManu_TreeNodePopulate(object sender, TreeNodeEventArgs e)
//    {
//        if (this.Content.Page is IMenuProvider)
//        {
//            IMenuProvider prov = this.Content.Page as IMenuProvider;
//
//            if (prov != null)
//                prov.ProvideNodes(e.Node);
//        }
//    }
//    protected void TreeViewMenu_SelectedNodeChanged(object sender, EventArgs e)
//    {
//        if (this.Content.Page is IMenuProvider)
//        {
//            IMenuProvider prov = this.Content.Page as IMenuProvider;
//
//            if (prov != null)
//                prov.MenuNodeChanged(TreeViewMenu.SelectedNode);
//        }
//    }

}
