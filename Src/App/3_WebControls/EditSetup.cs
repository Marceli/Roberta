//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Roberta.WebControls
{
	public class EditSetup
	{
		static public readonly string RegexEmail = @"^[\w-\.]+@([\w-]+\.)+[\w]{2,4}$";

		private EditView editView;
		private int column = 1;

		internal EditSetup(EditView editView) {
			this.editView = editView;
		}

		public int Columns {
			get { return editView.Columns; }
			set { editView.Columns = value; }
		}

		private void AddEditLabel(string label, int span) {
			string colspan = (span > 1 ? string.Format(" colspan=\"{0}\"", 2 * span - 1) : string.Empty);
			editView.Controls.Add(new LiteralControl(string.Format("{0}<td>{1}:</td><td{2}>\r\n\t",
				(column == 1 ? "<tr>" : string.Empty), label, colspan)));
		}

		private void EndCurrent(int span) {
			this.editView.Controls.Add(new LiteralControl(string.Format("</td>{0}\r\n",
				(this.column + span > this.Columns ? "</tr>" : string.Empty))));
			this.column += span;
			if (this.column > this.Columns) this.column = 1;
		}

		public void AddReadOnly(string name, string label, string value, int span, int width) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = value;
			textBox.Columns = width;
			textBox.CssClass = this.editView.CssReadOnly;
			textBox.ReadOnly = true;
			this.editView.Controls.Add(textBox);
			this.EndCurrent(span);
		}

		public void AddTextBox(string name, string label, string value, int span, int width, int length, bool optional, string regex, string warning) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = value;
			textBox.Columns = width;
			textBox.MaxLength = length;
			this.editView.Controls.Add(textBox);

			if (!optional) {
				textBox.CssClass = this.editView.CssRequired;
				this.editView.Controls.Add(new LiteralControl("\r\n\t"));
				this.AddRequired(name, warning);
			}

			if (!string.IsNullOrEmpty(regex)) {
				this.editView.Controls.Add(new LiteralControl("\r\n\t"));
				RegularExpressionValidator custom = new RegularExpressionValidator();
				custom.ID = name + "_Regex";
				custom.ControlToValidate = name;
				custom.ValidationGroup = this.editView.ID;
				custom.ValidationExpression = regex;
				custom.Text = "*";
				custom.ErrorMessage = warning;
				custom.CssClass = this.editView.CssWarning;
				this.editView.Controls.Add(custom);
			}

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

		public void AddMemoBox(string name, string label, string value, int span, int width, int rows, bool optional, string warning) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = value;
			textBox.Columns = width;
			textBox.TextMode = TextBoxMode.MultiLine;
			textBox.Rows = rows;
			this.editView.Controls.Add(textBox);

			if (!optional) {
				textBox.CssClass = this.editView.CssRequired;
				this.editView.Controls.Add(new LiteralControl("\r\n\t"));
				this.AddRequired(name, warning);
			}

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

//		public void AddHtmlBox(string name, string label, string value, int span, int width, int rows, bool optional, string warning) {
//			this.AddEditLabel(label, span);
//			FreeTextBox htmlBox = new FreeTextBox();
//			htmlBox.ID = name;
//			htmlBox.Text = value;
//			htmlBox.DownLevelCols = width;
//			htmlBox.DownLevelRows = rows;
//			htmlBox.EnableHtmlMode = true;
//			htmlBox.FormatHtmlTagsToXhtml = true;
//			this.editView.Controls.Add(htmlBox);
//
//			if (!optional) {
//				htmlBox.HtmlModeCssClass = this.editView.CssRequired;
//				this.editView.Controls.Add(new LiteralControl("\r\n\t"));
//				this.AddRequired(name, warning);
//			}
//
//			this.EndCurrent(span);
//			if (this.editView.focusControl == null) {
//				this.editView.focusControl = htmlBox;
//				htmlBox.Focus = true;
//			}
//		}

		public void AddCheckBox(string name, string label, bool value, int span) {
			this.AddEditLabel(label, span);
			CheckBox checkBox = new CheckBox();
			checkBox.ID = name;
			checkBox.Checked = value;
			checkBox.Text = string.Empty;
			checkBox.CssClass = this.editView.CssRequired;
			this.editView.Controls.Add(checkBox);
			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = checkBox;
		}

		public void AddDropDown(string name, string label, int? value, int span, string idMember, string textMember, string nullText, IList source) {
			this.AddEditLabel(label, span);
			DropDownList dropDown = new DropDownList();
			dropDown.ID = name;
			dropDown.DataValueField = idMember;
			dropDown.DataTextField = textMember;
			dropDown.DataSource = source;
			dropDown.DataBind();
			if (nullText != null) dropDown.Items.Insert(0, new ListItem(nullText, "0"));
			dropDown.SelectedValue = (value ?? 0).ToString();
			dropDown.CssClass = this.editView.CssRequired;
			this.editView.Controls.Add(dropDown);
			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = dropDown;
		}

		public void AddSelectList(string name, string label, int[] values, int span, int rows, string idMember, string textMember, IList source) {
			this.AddEditLabel(label, span);
			ListBox listBox = new ListBox();
			listBox.ID = name;
			listBox.SelectionMode = ListSelectionMode.Multiple;
			listBox.Rows = rows;
			listBox.DataValueField = idMember;
			listBox.DataTextField = textMember;
			listBox.DataSource = source;
			listBox.DataBind();
			foreach (int value in values) {
				listBox.Items.FindByValue(value.ToString()).Selected = true;
			}
			this.editView.Controls.Add(listBox);
			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = listBox;
		}

		public void AddIntegerBox(string name, string label, int? value, int span, int width, int length, int min, int max, string warning) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = (value.HasValue ? value.ToString() : string.Empty);
			textBox.Columns = width;
			textBox.MaxLength = length;
			textBox.CssClass = this.editView.CssRequired;
			this.editView.Controls.Add(textBox);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			this.AddRequired(name, warning);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			RangeValidator range = new RangeValidator();
			range.ID = name + "_Range";
			range.ControlToValidate = name;
			range.ValidationGroup = this.editView.ID;
			range.Type = ValidationDataType.Integer;
			range.MinimumValue = min.ToString();
			range.MaximumValue = max.ToString();
			range.Text = "*";
			range.ErrorMessage = warning;
			range.CssClass = this.editView.CssWarning;
			this.editView.Controls.Add(range);

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

		public void AddNumberBox(string name, string label, double? value, int span, int width, int length, double min, double max, string warning) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = (value.HasValue ? value.ToString() : string.Empty);
			textBox.Columns = width;
			textBox.MaxLength = length;
			textBox.CssClass = this.editView.CssRequired;
			this.editView.Controls.Add(textBox);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			this.AddRequired(name, warning);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			RangeValidator range = new RangeValidator();
			range.ID = name + "_Range";
			range.ControlToValidate = name;
			range.ValidationGroup = this.editView.ID;
			range.Type = ValidationDataType.Double;
			range.MinimumValue = min.ToString();
			range.MaximumValue = max.ToString();
			range.Text = "*";
			range.ErrorMessage = warning;
			range.CssClass = this.editView.CssWarning;
			this.editView.Controls.Add(range);

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

		public void AddCurrencyBox(string name, string label, decimal? value, int span, int width, int length, decimal min, decimal max, string warning) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = (value.HasValue ? value.ToString() : string.Empty);
			textBox.Columns = width;
			textBox.MaxLength = length;
			textBox.CssClass = this.editView.CssRequired;
			this.editView.Controls.Add(textBox);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			this.AddRequired(name, warning);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			RangeValidator range = new RangeValidator();
			range.ID = name + "_Range";
			range.ControlToValidate = name;
			range.ValidationGroup = this.editView.ID;
			range.Type = ValidationDataType.Currency;
			range.MinimumValue = min.ToString();
			range.MaximumValue = max.ToString();
			range.Text = "*";
			range.ErrorMessage = warning;
			range.CssClass = this.editView.CssWarning;
			this.editView.Controls.Add(range);

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

		public void AddDatePicker(string name, string label, DateTime? value, int span, string format, DateTime min, DateTime max, string warning) {
			// TODO Picker
			string dateFormat = (!string.IsNullOrEmpty(format) ? format : "MM/dd/yyyy");

			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Text = (value.HasValue ? value.Value.ToString(dateFormat) : string.Empty);
			textBox.Columns = Math.Max(dateFormat.Length, 10);
			textBox.MaxLength = Math.Max(dateFormat.Length, 10);
			textBox.CssClass = this.editView.CssRequired;
			this.editView.Controls.Add(textBox);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			this.AddRequired(name, warning);
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			RangeValidator range = new RangeValidator();
			range.ID = name + "_Range";
			range.ControlToValidate = name;
			range.ValidationGroup = this.editView.ID;
			range.Type = ValidationDataType.Date;
			range.MinimumValue = min.ToString(dateFormat);
			range.MaximumValue = max.ToString(dateFormat);
			range.Text = "*";
			range.ErrorMessage = warning;
			range.CssClass = this.editView.CssWarning;
			this.editView.Controls.Add(range);

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

		public void AddFilePicker(string name, string label, int span, int width, bool optional, string warning) {
			this.AddEditLabel(label, span);
			FileUpload filePicker = new FileUpload();
			filePicker.ID = name;
			filePicker.Attributes["size"] = width.ToString();
			this.editView.Controls.Add(filePicker);

			if (!optional) {
				filePicker.CssClass = this.editView.CssRequired;
				this.editView.Controls.Add(new LiteralControl("\r\n\t"));
				this.AddRequired(name, warning);
			}

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = filePicker;
		}

		public void AddOtherControl(string name, string label, Control control, int span) {
			this.AddEditLabel(label, span);
			control.ID = name;
			this.editView.Controls.Add(control);
			this.EndCurrent(span);
		}

		public void AddPassword(string name, string label, int span, int width, int length, bool optional, string warning) {
			this.AddEditLabel(label, span);
			TextBox textBox = new TextBox();
			textBox.ID = name;
			textBox.Columns = width;
			textBox.MaxLength = length;
			textBox.TextMode = TextBoxMode.Password;
			this.editView.Controls.Add(textBox);

			if (!optional) {
				textBox.CssClass = this.editView.CssRequired;
				this.editView.Controls.Add(new LiteralControl("\r\n\t"));
				this.AddRequired(name, warning);
			}

			this.editView.Controls.Add(new LiteralControl("<br />\r\n\t"));
			TextBox textBox2 = new TextBox();
			textBox2.ID = name + "_Repeat";
			textBox2.Columns = width;
			textBox2.MaxLength = length;
			textBox2.TextMode = TextBoxMode.Password;
			this.editView.Controls.Add(textBox2);
			if (!optional) {
				textBox2.CssClass = this.editView.CssRequired;
			}
			this.editView.Controls.Add(new LiteralControl("\r\n\t"));

			CompareValidator compare = new CompareValidator();
			compare.ID = name + "_Compare";
			compare.ControlToValidate = name;
			compare.ControlToCompare = name + "_Repeat";
			compare.ValidationGroup = this.editView.ID;
			compare.Text = "*";
			compare.ErrorMessage = warning;
			compare.CssClass = this.editView.CssWarning;
			this.editView.Controls.Add(compare);

			this.EndCurrent(span);
			if (this.editView.focusControl == null) this.editView.focusControl = textBox;
		}

		private void AddRequired(string name, string warning) {
			RequiredFieldValidator required = new RequiredFieldValidator();
			required.ID = name + "_Required";
			required.ControlToValidate = name;
			required.ValidationGroup = this.editView.ID;
			required.Text = "*";
			required.ErrorMessage = warning;
			required.CssClass = this.editView.CssWarning;
			this.editView.Controls.Add(required);
		}
	}
}