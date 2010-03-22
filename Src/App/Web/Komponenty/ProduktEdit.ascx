<%@ Control Language="C#" AutoEventWireup="true"  Inherits="VDetailsView" %>
<%@ Import Namespace="Web"%>
<%@ Import Namespace="Core.Entities"%>
<%@ Import Namespace="Roberta.WebControls"%>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Import namespace="Bll" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="NHibernate.Linq"%>
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
    public event EventHandler Anuluj;
    public event EventHandler Zapisz;
    
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        if(string.IsNullOrEmpty(GlobalSession.CurrentWebUser))
        {
            this.EditBtn.Visible = false;
        }
        else
        {
            string script = @"<script type=text/javascript>function olo() {window.open('CategoryPage.aspx','Kategorie','menubar=1,resizable=1,scrollbars = yes, width=350,height=250');document.form1.submit();}<" + @"/script>";
            Page.ClientScript.RegisterClientScriptBlock(typeof(Button), "olo", script);
            
            
            //if(GetProduct()!=null)
            //    this.CategoryEdit.SelectedValue = GetProduct().CategoryId.ToString();  
        }
    }
        public override Control GetItemTable()
        {
            return ItemTable;
        }
        public string ImagePath
        {
            get{return Server.MapPath(@"./Data") + @"\";}
        }
        public HttpPostedFile UploadedFile
        {
            get { return FileUpload1.PostedFile; }
        }
    

        public override Control GetEditTable()
        {
            return EditTable;
        }

        public override Control GetInsertTable()
        {
            return EditTable;
        }
        public Product GetProduct()
        {
            var p = GetEntity<Product>();
            p.Description = Opis.Text;
            
            return p;
        }
    
        
        string newProductCategory = "";
        public string NewProductCategory
        {
            get { return newProductCategory; }
            set { newProductCategory=value; }

        }
        public void InitData(int? id, DetailsViewMode mode)
        {
 	         base.InitData<Product>(id,mode);
             if (mode==DetailsViewMode.Edit)
             {
                 foreach (var picture in Db.Session.Get<Product>(id.Value).Pictures)
                 {
                     var tr = new TableRow();
                     var cel = new TableCell();
                     var i = new Image
                             	{
                             		ImageUrl = string.Format("{0}/Data/xs{1}.jpeg", Request.ApplicationPath, picture.Id)
                             	};
                 	cel.Controls.Add(i);
                     var b = new HtmlInputButton {Value = "Wstaw"};
                 	b.Attributes.Add("OnClick", string.Format(@"javascript:InsertImage('{0}/data/xs{1}.jpeg')", Request.ApplicationPath, picture.Id));
                     cel.Controls.Add(b);
                     tr.Cells.Add(cel);
                     ZdjeciaTable.Rows.Add(tr);
                 }                 
             }
            
             Opis.Text = id.HasValue && id>0 ? Db.Session.Get<Product>(id.Value).Description: "";
             CategoryEdit.DataBind();
             if (id > 0 &&   )
             {
                 CategoryEdit.SelectedValue = GetProduct().Category.Id.ToString();
             }
             else // dodałem na życzenie merka domyślnie przy dodawaniu wyświetlana jest ta ktegoria która jest wybrana
             {
				var c = new Category() {Name = NewProductCategory};
             	Db.Session.Save(c); 
                 if(c!=null)
                 {                    
                    CategoryEdit.SelectedValue= c.Id.ToString();    
                 }
            }
        }
        protected void AnulujBtn_Click(object sender, EventArgs e)
        {
            if (Anuluj != null)
                Anuluj(sender,e);

        
        }
        protected void ZapiszBtn_Click(object sender, EventArgs e)
        {
            if (Zapisz != null)
                Zapisz(sender, e);


        }
        protected void EditBtn_Click(object sender, EventArgs e)
        {
            this.InitData(this.GetEntity<Product>().Id, DetailsViewMode.Edit);
            
        }
        protected void RefreshCategory(object sender, EventArgs e)
        {
            var selected = CategoryEdit.SelectedValue;
            CategoryEdit.DataBind();
            CategoryEdit.SelectedValue = selected;
            
        }
        protected void ReadOnlyBtn_Click(object sender, EventArgs e)
        {
            InitData<Product>(GetEntity<Product>().Id, DetailsViewMode.ReadOnly);
        }
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
            <FTB:FreeTextBox SupportFolder="~/FTB/FreeTextBox" ID="Opis" runat="server" DownLevelCols="100" DownLevelRows="25" EnableHtmlMode="true" FormatHtmlTagsToXhtml="true" 
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




   

