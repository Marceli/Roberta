<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductEdit.ascx.cs" Inherits="Komponenty.ProductEdit" %>
<%@ Import Namespace="Web"%>

<script language=javascript>
function InsertImage(url)
{
   //var area;
   //area=document.getElementById('ctl02_Opis')
   //alert(area);
   //alert(area.innerText);
   FTB_API['ctl02_Opis'].ExecuteCommand('insertimage',null,url);
   //this.ExecuteCommand('insertimage',null,tempUrl);
   
  
}
</script>
<script runat="server">

</script>

&nbsp;<asp:Table ID="ItemTable" runat="server" Width="100%" Height="100%">
        <asp:TableRow>
            <asp:TableCell >&nbsp;<asp:Label ID="Title" runat="server"/></asp:TableCell>            
        </asp:TableRow>
        <asp:TableRow Height=100%>
            <asp:TableCell Width="100%">
            <asp:Label ID="Description" runat="server"/></asp:TableCell>           
        </asp:TableRow>
        <asp:TableRow>
        <asp:TableCell >
           <asp:Button ID="EditBtn" runat="server" OnClick="EditBtn_Click" Text="Edytuj" />                
           </asp:TableCell>
        </asp:TableRow>
       
</asp:Table>


    <asp:Table ID=EditTable runat="server" Visible="false" CssClass="ItemTable">
        <asp:TableRow>
            <asp:TableCell CssClass="LabelCell">Tytuł</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TitleEdit" runat="server" CssClass="textBox"></asp:TextBox>
               
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell CssClass="LabelCell">Rodzaj</asp:TableCell>
            <asp:TableCell CssClass="ItemCell">
                <asp:DropDownList ID="CategoryEdit" runat="server" DataSource="<%# Db.Category %>" DataValueField="Id" DataTextField="Name">
                </asp:DropDownList>
               <input type="Button" value="Edytuj kategorie" onclick="olo()" />
               <asp:Button id="Refresh" runat="server" text="Odśwież" OnClick="RefreshCategory" />              
            </asp:TableCell>
         </asp:TableRow>
         <asp:TableRow>
            <asp:TableCell ColumnSpan=2>
            <asp:Table ID="ZdjeciaTable" Width=100% runat=server>
                <asp:TableRow>
                    <asp:TableCell>Zdjęcia</asp:TableCell>
                </asp:TableRow>
            </asp:table>
            </asp:TableCell>
          </asp:TableRow>
          <asp:TableRow>
            <asp:TableCell CssClass="LabelCell">Nowe zdjęcie</asp:TableCell>
            <asp:TableCell CssClass="ItemCell">
            <asp:Label ID="ImageEdit" runat="server"></asp:Label> 
            <asp:FileUpload ID="FileUpload1" runat="server"   ></asp:FileUpload>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell CssClass="LabelCell" ColumnSpan=2>
            <FTB:FreeTextBox SupportFolder="~/FTB/FreeTextBox" ID="DescriptionEdit" runat="server" DownLevelCols="100" DownLevelRows="25" EnableHtmlMode="true" FormatHtmlTagsToXhtml="true" 
    JavaScriptLocation="ExternalFile" ToolbarImagesLocation="ExternalFile"
	ButtonImagesLocation="ExternalFile"
    />
            
            </asp:TableCell>
            </asp:TableRow>
             <asp:TableRow>
             <asp:TableCell CssClass="LabelCell" ColumnSpan=2>
                <asp:Button ID="ReadOnlyBtn" runat="server" OnClick="ReadOnlyBtn_Click" Text="Podgląd" />        
                <asp:Button ID="AnulujBtn" runat="server" OnClick="AnulujBtn_Click" Text="Anuluj" />
                <asp:Button ID="ZapiszBtn" runat="server" OnClick="ZapiszBtn_Click" Text="Zapisz" />
             </asp:TableCell>
             </asp:TableRow>
           
    </asp:table>



