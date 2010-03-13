<%@ Control  Language="C#" ClassName="ProductList" Inherits="Roberta.WebControls.UserControlBase" %>
<%@ Import Namespace="Wilson.ORMapper" %>
<%@ Import Namespace="Bll" %>

<script runat="server">  
    int ITEMS_COUNT = 9;
    public int TotalCount;
   
   
    protected override string ListAction(ListActionType listAction, string id)
    {
        switch (listAction)
        {
            case ListActionType.Edit:
                this.Lista.EditImage = Dm.GetObject<Product>(int.Parse(id)).Image + "128x96.jpeg";
                return this.ActionInfo.ActionEvent(this, "edit", id);
            case ListActionType.AddNew: return string.IsNullOrEmpty(GlobalSession.CurrentWebUser) ? string.Empty : this.ActionInfo.ActionEvent(this, "edit", 0);
            case ListActionType.Delete: return string.IsNullOrEmpty(GlobalSession.CurrentWebUser) ? string.Empty : this.ActionInfo.ActionEvent(this, "delete", id);
            default: return base.ListAction(listAction, id);
        }        
    }
        
    protected override void SetupList(ListSetup listSetup)
    {
        if (SortMember==string.Empty)
        {
            SortMember = "Title";           
        }   
        FiltrMember = "CategoryName";
        FiltrValue = FilterList.SelectedValue;
        ObjectSet<Product> os = Product.GetByField(FiltrMember
        , SortMember, SortAscending, FiltrValue, ITEMS_COUNT, PageNumber);
        listSetup.Source = os;
        TotalCount = listSetup.Source.Count;
        PageCount = TotalCount / ITEMS_COUNT + 1;
        listSetup.IdMember = "Id";
        listSetup.TextMember = "Title";
        listSetup.Columns.Add(new ListColumn("Title", "Nazwa", HorizontalAlign.Center, TypeColumn.Text));       
        FilterPanel.Visible = true;
        FilterList.DataBind();
	}
 
    protected void FilterList_DataBound(object sender, EventArgs e)
    {
        ListItem emptyItem = new ListItem("Wszystkie", "");
        ((DropDownList)sender).Items.Insert(0, emptyItem);
        ((DropDownList)sender).SelectedValue = FiltrValue;
    }
</script>



<asp:Panel ID="FilterPanel" runat="server" Width="100%">
    <asp:DropDownList ID="FilterList"  DataSource='<%#Dm.RetrieveAll<Category>() %>' DataTextField="Name" DataValueField="Name" runat="server" AutoPostBack="True" OnDataBound="FilterList_DataBound">
        
       
        
    </asp:DropDownList>&nbsp;</asp:Panel>

   
