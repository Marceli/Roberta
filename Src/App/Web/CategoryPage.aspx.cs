using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Core.Entities;
using Roberta.WebControls;

namespace Web
{
	public partial class CategoryPage : PageBase
	{
	
			protected void Page_Load(object sender, EventArgs e)
			{
				CategoryList1.EditRow += OnEditRowClick;
				CategoryList1.DeleteRow += OndeleteRow;
			}
			protected void OndeleteRow(int rowId)
			{
				var c=Db.Session.Get<Category>(rowId);
				Db.Session.Delete(c);
		       
			}

			protected void OnEditRowClick(int rowId)
			{
				CategoryList1.Visible = false;
				EditPanel.Visible = true;
				CategoryEdit1.InitData<Product>(rowId, DetailsViewMode.Edit);
			}


			protected void AnulujBtn_Click(object sender, EventArgs e)
			{
				CategoryList1.Visible = true;
				EditPanel.Visible = false;

			}

			protected void ZapiszBtn_Click(object sender, EventArgs e)
			{
				var c = CategoryEdit1.GetEntity<Category>();

				Db.Session.Save(c);
				CategoryList1.Visible = true;
				EditPanel.Visible = false;

			}

	}
}
