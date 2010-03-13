<%@ Page Language="c#" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<script runat="server">

    void Button1_Click(object sender, EventArgs e)
    {
        Literal1.Text = FreeTextBox1.Text;        
    }
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:WebPartManager ID="WebPartManager1" runat="server">
        </asp:WebPartManager>
        
        <br />

        <asp:WebPartZone ID="WebPartZone1" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana"
            Padding="6">
            <PartChromeStyle BackColor="#EFF3FB" BorderColor="#D1DDF1" Font-Names="Verdana" ForeColor="#333333" />
            <MenuLabelHoverStyle ForeColor="#D1DDF1" />
            <EmptyZoneTextStyle Font-Size="0.8em" />
            <MenuLabelStyle ForeColor="White" />
            <MenuVerbHoverStyle BackColor="#EFF3FB" BorderColor="#CCCCCC" BorderStyle="Solid"
                BorderWidth="1px" ForeColor="#333333" />
            <HeaderStyle Font-Size="0.7em" ForeColor="#CCCCCC" HorizontalAlign="Center" />
            <MenuVerbStyle BorderColor="#507CD1" BorderStyle="Solid" BorderWidth="1px" ForeColor="White" />
            <PartStyle Font-Size="0.8em" ForeColor="#333333" />
            <TitleBarVerbStyle Font-Size="0.6em" Font-Underline="False" ForeColor="White" />
            <MenuPopupStyle BackColor="#507CD1" BorderColor="#CCCCCC" BorderWidth="1px" Font-Names="Verdana"
                Font-Size="0.6em" />
            <PartTitleStyle BackColor="#507CD1" Font-Bold="True" Font-Size="0.8em" ForeColor="White" />
            <ZoneTemplate>
                <FTB:FreeTextBox ID="FreeTextBox1" runat="server" />
                
                <br />
                Label: <asp:Literal ID="Literal1" runat="server" />
                
                <br /><br />
                
                <asp:Button ID="Button1" Text="Press Me" OnClick="Button1_Click" runat="server" />
                
            </ZoneTemplate>
        </asp:WebPartZone>

    
    </div>
    </form>
</body>
</html>
