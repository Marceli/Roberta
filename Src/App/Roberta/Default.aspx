<%@ Page  Language="C#" AutoEventWireup="true" ClassName="Produkty" Inherits="Roberta.WebControls.PageBase" %>
<%@ Register Src="Komponenty/ProduktEdit.ascx" TagName="ProduktEdit" TagPrefix="Roberta" %>
<%@ Register Src="Komponenty/ProductList.ascx" TagName="ProductList" TagPrefix="Roberta" %>
<%@ Import Namespace="Bll" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e)
   {
       ProductList1.EditRow += new OnEditRow(OnEditRowClick);
       ProductList1.deleteRow += new OnDeleteRow(ProductList1_deleteRow);
       ProduktEdit1.Anuluj += new EventHandler(Anuluj);
    
       ProduktEdit1.Zapisz+= new EventHandler(Zapisz);
       if(string.IsNullOrEmpty(GlobalSession.CurrentWebUser))
       {
           this.LogujLB.Text = "Zaloguj";
       }else
       {
           this.LogujLB.Text = "Wyloguj";
       }
    
    
    
   
     
   }

    void ProductList1_deleteRow(int rowId)
    {
        
        
        Product p = Dm.GetObject<Product>(rowId);
        p.Delete(null);
    }
    

    protected void OnEditRowClick(int rowId)
    {
        
        ProductList1.Visible=false;
        EditPanel.Visible = true;
        ProduktEdit1.NewProductCategory = ProductList1.FiltrValue;
        ProduktEdit1.InitData(rowId, string.IsNullOrEmpty(GlobalSession.CurrentWebUser) ? DetailsViewMode.ReadOnly:DetailsViewMode.Edit);
        
    }


    protected void Anuluj(object sender,EventArgs e)
    {
        ProductList1.Visible = true;
        EditPanel.Visible=false;
       
    }

    protected void Zapisz(object sender, EventArgs e)
    {
        Product p=ProduktEdit1.GetProduct();
        Picture picture=null;

        if (ProduktEdit1.UploadedFile.ContentLength > 0)
        {
            picture = new Picture();
           
            p.Pictures.Add(picture);
            //p.Image = "Data/" + SaveFile(FileUpload1.PostedFile);
        }
        
        p.Save(null);
        
        if(picture!=null)
        {
           
            picture.SavePictures(ProduktEdit1.UploadedFile, ProduktEdit1.ImagePath);
        }
       
        ProductList1.Visible = true;
        EditPanel.Visible = false;

    }



    protected void LogujLB_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(GlobalSession.CurrentWebUser))
        {
            Response.Redirect("~/Login.aspx");
        }
        else
        {
            FormsAuthentication.SignOut();
            Response.Redirect("~/default.aspx");
        }

    }
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
