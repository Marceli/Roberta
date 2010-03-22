//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Roberta.WebControls
{
	public class EditValues
	{
		private EditView editView;

		internal EditValues(EditView editView) {
			this.editView = editView;
		}

		public string GetString(string name) {
			return (editView.FindControl(name) as TextBox).Text;
		}

		

		public bool GetBool(string name) {
			return (editView.FindControl(name) as CheckBox).Checked;
		}

		public int? GetSelected(string name) {
			int value;
			int.TryParse((editView.FindControl(name) as DropDownList).SelectedValue, out value);
			return (value == 0 ? default(int?) : value);
		}

		public int[] GetSelects(string name) {
			ListBox listBox = editView.FindControl(name) as ListBox;
			List<int> selects = new List<int>();
			foreach (ListItem item in listBox.Items) {
				if (item.Selected) {
					int value;
					if (int.TryParse(item.Value, out value)) {
						selects.Add(value);
					}
				}
			}
			return selects.ToArray();
		}

		public int GetInteger(string name) {
			int value;
			int.TryParse(GetString(name), out value);
			return value;
		}

		public double GetNumber(string name) {
			double value ;
			double.TryParse(GetString(name), out value);
			return value;
		}

		public decimal GetCurrency(string name) {
			decimal value ;
			decimal.TryParse(GetString(name), out value);
			return value;
		}

		public DateTime GetDate(string name) {
			DateTime value ;
			DateTime.TryParse(GetString(name), out value);
			return value.Date;
		}

		public HttpPostedFile GetFile(string name) {
			FileUpload fileUpload = editView.FindControl(name) as FileUpload;
			if (!fileUpload.HasFile) return null;
			return fileUpload.PostedFile;
		}

		public Control GetControl(string name) {
			return editView.FindControl(name);
		}
	}
}