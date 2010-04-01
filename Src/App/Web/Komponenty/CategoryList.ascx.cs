using System;

using System.Linq;

using System.Web.UI.WebControls;

using Roberta.WebControls;

namespace Web.Komponenty
{
	public partial class CategoryList : UserControlBase
	{
		int ITEMS_COUNT = 900;
		public int TotalCount;
		protected override void SetupList(ListSetup listSetup)
		{
			if (SortMember==string.Empty)
			{
				SortMember = "Name";           
			}
			listSetup.Source = Db.Category.ToList();
			TotalCount = listSetup.Source.Count;
			PageCount = TotalCount / ITEMS_COUNT + 1;
			listSetup.IdMember = "Id";
			listSetup.TextMember = "Name";
			listSetup.Columns.Add(new ListColumn("Name", "Nazwa", HorizontalAlign.Center, TypeColumn.Text));       
		}
	   

		protected void FilterList_DataBound(object sender, EventArgs e)
		{
			var emptyItem = new ListItem("Wszystkie", "");
			((DropDownList)sender).Items.Insert(0, emptyItem);
			((DropDownList)sender).SelectedValue = FiltrValue;
		}

	}
}