<%@ Page  Language="C#" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="Web._Default" %>
<%@ Import Namespace="Web"%>
<%@ Import Namespace="Core.Entities"%>
<%@ Import Namespace="Roberta.WebControls"%>
<%@ Register Src="Komponenty/ProductEdit.ascx" TagName="ProduktEdit" TagPrefix="Roberta" %>
<%@ Register Src="Komponenty/ProductList.ascx" TagName="ProductList" TagPrefix="Roberta" %>
<%@ Import Namespace="Bll" %>
<script runat="server">
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>

<body>

    <form id="form1" runat="server">
    <div align="right">
        &nbsp;
        <asp:LinkButton ID="LogujLB" runat="server" OnClick="LogujLB_Click" >Loguj</asp:LinkButton></div>
    <div>
        <Roberta:ProductList ID="ProductList1" runat="server"  />
        <asp:Panel ID="EditPanel" runat="server" Visible="false">
            <Roberta:ProduktEdit Id="ProduktEdit1" runat="server"   />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
