//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Roberta.WebControls
{
    public delegate void AddEditControls(EditSetup editSetup);

    public delegate void SaveEditEntity(EditValues editValues);

    public delegate void CloseEditView(bool entitySaved);

    [Themeable(true)]
    [ToolboxData("<{0}:EditView runat=server></{0}:EditView>")]
    public class EditView : Control, INamingContainer
    {
        private AddEditControls addControls;
        private SaveEditEntity saveEntity;
        private CloseEditView closeEditView;

        private int columns = 1;

        private string saveText = "Save";
        private string cancelText = "Cancel";

        private string saveImage;
        private string cancelImage;

        private string cssReadOnly = "editReadOnly";
        private string cssRequired = "editRequired";
        private string cssWarning = "editWarning";

        internal Control focusControl = null;

        [Themeable(false)]
        [Browsable(false)]
        public AddEditControls AddControls
        {
            get { return addControls; }
            set { addControls = value; }
        }

        [Themeable(false)]
        [Browsable(false)]
        public SaveEditEntity SaveEntity
        {
            get { return saveEntity; }
            set { saveEntity = value; }
        }

        [Themeable(false)]
        [Browsable(false)]
        public CloseEditView CloseEditView
        {
            get { return closeEditView; }
            set { closeEditView = value; }
        }

        [Themeable(false)]
        [Category("EditView"), DefaultValue("1"), Description("Number of Columns for EditView")]
        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        [Category("EditView"), DefaultValue("Save"), Description("Text or Alt for Save Button")]
        public string SaveText
        {
            get { return saveText; }
            set { saveText = value; }
        }

        [Category("EditView"), DefaultValue("Cancel"), Description("Text or Alt for Cancel Button")]
        public string CancelText
        {
            get { return cancelText; }
            set { cancelText = value; }
        }

        [Category("EditView"), DefaultValue(null), Description("Image Url for Save Button")]
        public string SaveImage
        {
            get { return saveImage; }
            set { saveImage = value; }
        }

        [Category("EditView"), DefaultValue(null), Description("Image Url Cancel Button")]
        public string CancelImage
        {
            get { return cancelImage; }
            set { cancelImage = value; }
        }

        [Category("Appearance"), DefaultValue("editReadOnly"), Description("ReadOnly Style for EditView")]
        public string CssReadOnly
        {
            get { return cssReadOnly; }
            set { cssReadOnly = value; }
        }

        [Category("Appearance"), DefaultValue("editRequired"), Description("Required Style for EditView")]
        public string CssRequired
        {
            get { return cssRequired; }
            set { cssRequired = value; }
        }

        [Category("Appearance"), DefaultValue("editWarning"), Description("Warning Style for EditView")]
        public string CssWarning
        {
            get { return cssWarning; }
            set { cssWarning = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (AddControls == null)
            {
                throw new ApplicationException("The AddControls delegate must be handled.");
            }
            if (SaveEntity == null)
            {
                throw new ApplicationException("The SaveEntity delegate must be handled.");
            }
            if (CloseEditView == null)
            {
                throw new ApplicationException("The CloseEditView delegate must be handled.");
            }
            AddErrorSummary();
            AddControls(new EditSetup(this));
            AddEditButtons();
            if (focusControl != null)
            {
                Page.SetFocus(focusControl);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Context == null)
            {
                writer.Write("[ - EditView - ]");
                return;
            }

            writer.Write("<!-- Wilson.WebControls.EditView -- Begins Here -- http://www.WilsonDotNet.com -->\r\n");
            writer.Write("<table align=\"center\">\r\n");
            base.Render(writer);
            writer.Write("</table>\r\n");
            writer.Write("<!-- Wilson.WebControls.EditView -- Ends Here -- mailto:Paul@WilsonDotNet.com -->");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    SaveEntity(new EditValues(this));
                    CloseEditView(true);
                }
                catch (Exception exception)
                {
                    Controls.AddAt(0, new LiteralControl(string.Format(
                                                             "<tr><td colspan=\"{0}\" align=\"center\" class=\"{1}\">\r\n{2}</td></tr>\r\n",
                                                             2*Columns, cssWarning, exception.Message)));
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CloseEditView(false);
        }

        private void AddErrorSummary()
        {
            Controls.Add(new LiteralControl(string.Format("<tr><td colspan=\"{0}\" align=\"center\">\r\n\t", 2*Columns)));
            ValidationSummary summary = new ValidationSummary();
            summary.ID = ID + "_Errors";
            summary.ValidationGroup = ID;
            summary.CssClass = cssWarning;
            Controls.Add(summary);
            Controls.Add(new LiteralControl("</td></tr>\r\n"));
        }

        private void AddEditButtons()
        {
            Controls.Add(new LiteralControl(string.Format("<tr><td colspan=\"{0}\" align=\"center\">\r\n\t", 2*Columns)));
            if (string.IsNullOrEmpty(SaveImage))
            {
                Button saveButton = new Button();
                saveButton.ID = "Save_Button";
                saveButton.Text = SaveText;
                saveButton.ValidationGroup = ID;
                saveButton.Click += new EventHandler(SaveButton_Click);
                Controls.Add(saveButton);
            }
            else
            {
                ImageButton saveButton = new ImageButton();
                saveButton.ID = "Save_Button";
                saveButton.ImageUrl = Page.ResolveUrl(SaveImage);
                saveButton.AlternateText = SaveText;
                saveButton.ValidationGroup = ID;
                saveButton.Click += new ImageClickEventHandler(SaveButton_Click);
                Controls.Add(saveButton);
            }
            Controls.Add(new LiteralControl("\r\n\t"));
            if (string.IsNullOrEmpty(CancelImage))
            {
                Button cancelButton = new Button();
                cancelButton.ID = "Cancel_Button";
                cancelButton.Text = CancelText;
                cancelButton.CausesValidation = false;
                cancelButton.Click += new EventHandler(CancelButton_Click);
                Controls.Add(cancelButton);
            }
            else
            {
                ImageButton cancelButton = new ImageButton();
                cancelButton.ID = "Cancel_Button";
                cancelButton.ImageUrl = Page.ResolveUrl(CancelImage);
                cancelButton.AlternateText = CancelText;
                cancelButton.CausesValidation = false;
                cancelButton.Click += new ImageClickEventHandler(CancelButton_Click);
                Controls.Add(cancelButton);
            }
            Controls.Add(new LiteralControl("</td></tr>\r\n"));
        }
    }
}