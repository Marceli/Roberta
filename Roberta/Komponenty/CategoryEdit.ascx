<%@ Control Language="C#" ClassName="CategoryEdit" AutoEventWireup="true"  Inherits="VDetailsView" %>
<%@ Import namespace="Bll"%>
<script runat="server">
    
    public override Control GetItemTable()
    {
        return ItemTable;
    }

    public override Control GetEditTable()
    {
        return EditTable;
    }

    public override Control GetInsertTable()
    {
        return EditTable;
    }
    

  
</script>

<asp:Table ID="ItemTable" runat="server" CssClass="ItemTable">
        <asp:TableRow>
            <asp:TableCell CssClass="LabelCell">Nazwa</asp:TableCell>
            <asp:TableCell CssClass="ItemCell">&nbsp;<asp:Label ID="Name" runat="server"/></asp:TableCell>            
        </asp:TableRow>
</asp:Table>


    <asp:Table ID=EditTable runat="server" Visible="false" CssClass="ItemTable">
        <asp:TableRow>
            <asp:TableCell CssClass="LabelCell">Nazwa</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="NameEdit" runat="server" CssClass="textBox"></asp:TextBox>               
            </asp:TableCell>
        </asp:TableRow>

    </asp:table>

   

