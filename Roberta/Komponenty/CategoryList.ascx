<%@ Control  Language="C#" ClassName="CategoryList" Inherits="Roberta.WebControls.UserControlBase" %>
<%@ Import Namespace="Wilson.ORMapper" %>
<%@ Import Namespace="Bll" %>

<script runat="server">  
    int ITEMS_COUNT = 900;
    public int TotalCount;
   
   

        
    protected override void SetupList(ListSetup listSetup)
    {
        if (SortMember==string.Empty)
        {
            SortMember = "Name";           
        }
        listSetup.Source = Dm.RetrieveAll<Category>();
        TotalCount = listSetup.Source.Count;
        PageCount = TotalCount / ITEMS_COUNT + 1;
        listSetup.IdMember = "Id";
        listSetup.TextMember = "Name";
        listSetup.Columns.Add(new ListColumn("Name", "Nazwa", HorizontalAlign.Center, TypeColumn.Text));       
	}
   

    protected void FilterList_DataBound(object sender, EventArgs e)
    {
        ListItem emptyItem = new ListItem("Wszystkie", "");
        ((DropDownList)sender).Items.Insert(0, emptyItem);
        ((DropDownList)sender).SelectedValue = FiltrValue;
    }
</script>