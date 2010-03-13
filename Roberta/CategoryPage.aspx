<%@ Page Language="C#" Inherits="Roberta.WebControls.PageBase" %>

<%@ Register Src="Komponenty/CategoryEdit.ascx" TagName="CategoryEdit" TagPrefix="uc2" %>

<%@ Register Src="Komponenty/CategoryList.ascx" TagName="CategoryList" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        CategoryList1.EditRow += new OnEditRow(OnEditRowClick);
        CategoryList1.deleteRow += new OnDeleteRow(OndeleteRow);
    }
    protected void OndeleteRow(int rowId)
    {
        Category c=Dm.GetObject<Category>(rowId);
        c.Delete(null);
       
    }

    protected void OnEditRowClick(int rowId)
    {
        CategoryList1.Visible = false;
        EditPanel.Visible = true;
        CategoryEdit1.InitData<Product>(rowId, DetailsViewMode.Edit);
    }


    protected void AnulujBtn_Click(object sender, EventArgs e)
    {
        CategoryList1.Visible = true;
        EditPanel.Visible = false;

    }

    protected void ZapiszBtn_Click(object sender, EventArgs e)
    {
        Category p = CategoryEdit1.GetEntity<Category>();

        p.Save(null);
        CategoryList1.Visible = true;
        EditPanel.Visible = false;

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    
        <uc1:CategoryList id="CategoryList1" runat="server">
        </uc1:CategoryList>
        <asp:Panel ID="EditPanel" runat="server" Visible="false">
            <uc2:CategoryEdit Id="CategoryEdit1" runat="server" />    
        
        <asp:Button ID="AnulujBtn" runat="server" Text="Anuluj" OnClick="AnulujBtn_Click" />
        <asp:Button ID="ZapiszBtn" runat="server" Text="Zapisz" OnClick="ZapiszBtn_Click" />
        </asp:Panel>
    </form>
</body>
</html>
