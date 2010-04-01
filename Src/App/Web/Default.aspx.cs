using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using Core.Entities;
using Roberta.WebControls;

namespace Web

{
	public partial class _Default : PageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			ProductList1.EditRow += OnEditRowClick;
			ProductList1.DeleteRow += OnDeleteRow;
			ProduktEdit1.Anuluj += Anuluj;

			ProduktEdit1.Zapisz += Zapisz;
			if (string.IsNullOrEmpty(GlobalSession.CurrentWebUser))
			{
				LogujLB.Text = "Zaloguj";
			}
			else
			{
				LogujLB.Text = "Wyloguj";
			}
		}

		private void OnDeleteRow(int rowId)
		{
			var p = Db.Session.Get<Product>(rowId);
			Db.Session.Delete(p);
		}


		protected void OnEditRowClick(int rowId)
		{
			ProductList1.Visible = false;
			EditPanel.Visible = true;
			ProduktEdit1.NewProductCategory = ProductList1.FiltrValue;
			ProduktEdit1.InitData(rowId,
			                      string.IsNullOrEmpty(GlobalSession.CurrentWebUser)
			                      	? DetailsViewMode.ReadOnly
			                      	: DetailsViewMode.Edit);
		}


		protected void Anuluj(object sender, EventArgs e)
		{
			ProductList1.Visible = true;
			EditPanel.Visible = false;
		}

		protected void Zapisz(object sender, EventArgs e)
		{
			var p = ProduktEdit1.GetProduct();
			Picture picture = null;

			if (ProduktEdit1.UploadedFile.ContentLength > 0)
			{
				picture = new Picture();
				p.AddPicture(picture);
				Db.Session.Save(picture);

				picture.SavePictures(ProduktEdit1.UploadedFile.InputStream, Server.MapPath("/Data/"));
			}

			Db.Session.Save(p);

			

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
	}
}