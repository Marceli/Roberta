<%@ Page Language="C#" Inherits="Roberta.WebControls.PageBase" %>
<%@ Import Namespace="Roberta.WebControls"%>
<%@ Import Namespace="Web"%>
<%@ Import Namespace="Core.Entities"%>

<%@ Register Src="Komponenty/CategoryEdit.ascx" TagName="CategoryEdit" TagPrefix="uc2" %>

<%@ Register Src="Komponenty/CategoryList.ascx" TagName="CategoryList" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">



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
