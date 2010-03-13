//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Roberta.WebControls
{
    public delegate void SetupListView(ListSetup listSetup);
   
    public delegate string ListActionLink(ListActionType listAction, string entityId);   

    public enum ListActionType
    {
        AddNew,
        Edit,
        Delete,
        Prev,
        Next,
        First,
        Last,
        Sort
    }
    
    
    [Themeable(true)]
    [ToolboxData("<{0}:ListView runat=server></{0}:ListView>")]
    public class ListView : Control
    {




        protected string PageLink(int pageNumber)
        {
            return Page.ClientScript.GetPostBackClientHyperlink(this, "page=" + pageNumber.ToString());
        }
     
        private SetupListView setupList;
        private ListActionLink actionLink;
       // private ListSortActionLink sortActionLink;
        private IList source = null;
        private string idMember = "Id";
        private string textMember = "Name";
        private IList<ListColumn> columns = new List<ListColumn>();

        private string addNewText = "Add New";
        private string editText = "Edit";
        private string deleteText = "Delete";

        private string addNewImage;
        private string editImage;
        private string deleteImage;

        private string trueImage;
        private string falseImage;

        private string cssListBox = "listBox";
        private string cssHeader = "listHeader";
        private string cssMainRow = "listMainRow";
        private string cssAltRow = "listAltRow";
        private string cssHoverRow = "listHoverRow";
        
        
        

        private bool oddItem = false;
       
       

      
        
        public SetupListView SetupList
        {
            get { return setupList; }
            set { setupList = value; }
        }

        [Themeable(false)]
        [Browsable(false)]
        public ListActionLink ActionLink
        {
            get { return actionLink; }
            set { actionLink = value; }
        }



        [Themeable(false)]
        [Browsable(false)]
        public IList Source
        {
            get { return source; }
            set { source = value; }
        }

        private string firstPageLink;

        [Themeable(false)]
        [Browsable(false)]
        public string FirstPageLink
        {
            get { return firstPageLink; }
            set { firstPageLink = value; }
        }

        private string prevPageLink;

        [Themeable(false)]
        [Browsable(false)]
        public string PrevPageLink
        {
            get { return prevPageLink; }
            set { prevPageLink = value; }
        }

        private string nextPageLink;

        [Themeable(false)]
        [Browsable(false)]
        public string NextPageLink
        {
            get { return nextPageLink; }
            set { nextPageLink = value; }
        }

        private string lastPageLink;

        [Themeable(false)]
        [Browsable(false)]
        public string LastPageLink
        {
            get { return lastPageLink; }
            set { lastPageLink = value; }
        }


        [Themeable(false)]
        [Category("ListView"), DefaultValue("Id"), Description("Entity Primary Key Member")]
        public string IdMember
        {
            get { return idMember; }
            set { idMember = value; }
        }

        [Themeable(false)]
        [Category("ListView"), DefaultValue("Name"), Description("Entity Unique Text Member")]
        public string TextMember
        {
            get { return textMember; }
            set { textMember = value; }
        }

        [Themeable(false)]
        [Browsable(false)]
        public IList<ListColumn> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        [Category("ListView"), DefaultValue("Add New"), Description("Text or Alt for Add New")]
        public string AddNewText
        {
            get { return addNewText; }
            set { addNewText = value; }
        }

        [Category("ListView"), DefaultValue("Edit"), Description("Text or Alt for Edit")]
        public string EditText
        {
            get { return editText; }
            set { editText = value; }
        }

        private string firstPageImage;

        [Category("ListView"), DefaultValue(null), Description("Image Url for FirstPage")]
        public string FirstPageImage
        {
            get { return firstPageImage; }
            set { firstPageImage = value; }
        }

        private string firstPageText;

        [Category("ListView"), DefaultValue("<<"), Description("Text or Alt for FirstPage")]
        public string FirstPageText
        {
            get { return firstPageText; }
            set { firstPageText = value; }
        }

        private string prevPageImage;

        [Category("ListView"), DefaultValue(null), Description("Image Url for PrevPage")]
        public string PrevPageImage
        {
            get { return prevPageImage; }
            set { prevPageImage = value; }
        }

        private string prevPageText;

        [Category("ListView"), DefaultValue("<"), Description("Text or Alt for PrevPage")]
        public string PrevPageText
        {
            get { return prevPageText; }
            set { prevPageText = value; }
        }

        private string nextPageImage;

        [Category("ListView"), DefaultValue(null), Description("Image Url for NextPage")]
        public string NextPageImage
        {
            get { return nextPageImage; }
            set { nextPageImage = value; }
        }

        private string nextPageText;

        [Category("ListView"), DefaultValue(">"), Description("Text or Alt for NextPage")]
        public string NextPageText
        {
            get { return nextPageText; }
            set { nextPageText = value; }
        }

        private string lastPageText;

        [Category("ListView"), DefaultValue(">>"), Description("Text or Alt for LastPage")]
        public string LastPageText
        {
            get { return lastPageText; }
            set { lastPageText = value; }
        }

        private string lastPageImage;

        [Category("ListView"), DefaultValue(null), Description("Image Url for LastPage")]
        public string LastPageImage
        {
            get { return lastPageImage; }
            set { lastPageImage = value; }
        }

        [Category("ListView"), DefaultValue("Delete"), Description("Text or Alt for Delete")]
        public string DeleteText
        {
            get { return deleteText; }
            set { deleteText = value; }
        }

        [Category("ListView"), DefaultValue(null), Description("Image Url for Add New")]
        public string AddNewImage
        {
            get { return addNewImage; }
            set { addNewImage = value; }
        }

        [Category("ListView"), DefaultValue(null), Description("Image Url for Edit")]
        public string EditImage
        {
            get { return editImage; }
            set { editImage = value; }
        }

        [Category("ListView"), DefaultValue(null), Description("Image Url for Delete")]
        public string DeleteImage
        {
            get { return deleteImage; }
            set { deleteImage = value; }
        }

        [Category("ListView"), DefaultValue(null), Description("Image Url for True")]
        public string TrueImage
        {
            get { return trueImage; }
            set { trueImage = value; }
        }

        [Category("ListView"), DefaultValue(null), Description("Image Url for False")]
        public string FalseImage
        {
            get { return falseImage; }
            set { falseImage = value; }
        }

        [Category("Appearance"), DefaultValue("listBox"), Description("Table Style for ListView")]
        public string CssListBox
        {
            get { return cssListBox; }
            set { cssListBox = value; }
        }

        [Category("Appearance"), DefaultValue("listHeader"), Description("Header Style for ListView")]
        public string CssHeader
        {
            get { return cssHeader; }
            set { cssHeader = value; }
        }

        [Category("Appearance"), DefaultValue("listMainRow"), Description("Main Row Style for ListView")]
        public string CssMainRow
        {
            get { return cssMainRow; }
            set { cssMainRow = value; }
        }

        [Category("Appearance"), DefaultValue("listAltRow"), Description("Alt Row Style for ListView")]
        public string CssAltRow
        {
            get { return cssAltRow; }
            set { cssAltRow = value; }
        }

        [Category("Appearance"), DefaultValue("listHoverRow"), Description("Hover Row Style for ListView")]
        public string CssHoverRow
        {
            get { return cssHoverRow; }
            set { cssHoverRow = value; }
        }

        private bool pagerBottom;

        [Category("Appearance"), DefaultValue("True"), Description("True if peger shoud be on bottom")]
        public bool PagerBottom
        {
            get { return pagerBottom; }
            set { pagerBottom = value; }
        }

        private string pagerHorizontalAlign;

        [Category("Appearance"), DefaultValue("center"), Description("shoud  be left center right")]
        public string PagerHorizontalAlign
        {
            get { return pagerHorizontalAlign; }
            set { pagerHorizontalAlign = value; }
        }

        
        

       
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (ActionLink == null)
            {
                throw new ApplicationException("The ActionLink delegate must be handled.");
            }
            if (SetupList != null)
            {
                SetupList(new ListSetup(this));
            }
            FindUserCtrl().SaveState();
           
            RegisterDelete();


        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Context == null)
            {
                writer.Write("[ - ListView - ]");
                return;
            }

            StringBuilder list = new StringBuilder();
            list.AppendFormat("\r\n<table class=\"{0}\" width=\"100%\">", CssListBox);

            UserControlBase ucb = FindUserCtrl();

            if (ucb != null && ucb.PageCount > 1 && !PagerBottom)
                list = AddPagerRow(list);
            list.AppendFormat("\r\n<tr class=\"{0}\">", CssHeader);
            string addNewLink = AddNewLink();
            if (string.IsNullOrEmpty(addNewLink))
            {
                list.Append("\r\n\t<td>&nbsp;</td>");
            }
            else
            {
                list.AppendFormat("\r\n\t<td><a href=\"{0}\">{1}</a></td>",
                                  AddNewLink(), GetAction(AddNewText, AddNewImage));
            }
            for (int index = 0; index < Columns.Count; index++)
            {
                string sortLink = SortLink(Columns[index].Member);
                string sortImg = string.Empty;
                if (ucb.SortMember == Columns[index].Member)
                    sortImg = string.Format("<img src=\"./Data/{0}.gif\" />", ucb.SortAscending ? "goraB" : "dolB");
                list.Append("\r\n\t<td");
                if (!string.IsNullOrEmpty(Columns[index].Class))
                    list.AppendFormat(" class=\"{0}\"", Columns[index].Class);
                list.AppendFormat("><a ID=\"{0}\" href=\"{1}\">{0}{2}</a></td>", Columns[index].Label, sortLink, sortImg);
            }
            list.Append("\r\n</tr>");

            foreach (object entity in Source)
            {
                oddItem = !oddItem;
                string rowStyle = (oddItem ? CssMainRow : CssAltRow);
                list.AppendFormat("\r\n<tr class=\"{0}\" onMouseOver=\"this.className = '{1}';\"" +
                                  " onMouseOut=\"this.className = '{0}';\">", rowStyle, CssHoverRow);
                string editLink = EditLink((int)GetValue(entity, IdMember, TypeColumn.Text));
                if (string.IsNullOrEmpty(editLink))
                {
                    list.Append("\r\n\t<td width=\"1px\" align=\"center\">&nbsp;");
                }
                else
                {
                    list.AppendFormat("\r\n\t<td width=\"1px\" align=\"center\"><a href=\"{0}\">{1}</a>",
                                      editLink, GetAction(EditText, EditImage));
                }
                string deleteLink = DeleteLink((int)GetValue(entity, IdMember, TypeColumn.Text),
                                               (string)GetValue(entity, TextMember, TypeColumn.Text));
                if (string.IsNullOrEmpty(deleteLink))
                {
                    list.Append("\r\n\t\t&nbsp;</td>");
                }
                else
                {
                    list.AppendFormat("\r\n\t\t<a href=\"{0}\">{1}</a></td>",
                                      deleteLink, GetAction(DeleteText, DeleteImage));
                }
                for (int index = 0; index < Columns.Count; index++)
                {
                    list.AppendFormat("\r\n\t<td{0}>{1}</td>", GetAlign(Columns[index].Align),
                                      GetValue(entity, Columns[index].Member, Columns[index].Type));
                }


                list.Append("\r\n</tr>");
            }
            if (ucb != null && ucb.PageCount > 1 && PagerBottom)
                list = AddPagerRow(list);
            list.Append("\r\n</table>");
            writer.Write(list.ToString());
        }

        private UserControlBase FindUserCtrl()
        {
            Control ctrl = Parent;
            while (ctrl != null && !(ctrl is UserControlBase))
                ctrl = ctrl.Parent;

            if (ctrl == null)
                throw new InvalidOperationException("Nie znaleziono UserControlBase.");

            return ctrl as UserControlBase;
        }

        private StringBuilder AddPagerRow(StringBuilder list)
        {
            // tu dodamy paging
            list.AppendFormat("\r\n<tr class=\"{0}\">", CssHeader);
            list.AppendFormat("\r\n\t<td align=\"{3}\" colspan=\"{2}\">\r\n\t<a name=\"First\" id=\"First\" href=\"{0}\">{1}</a>", FirstPageAction()
                              , GetAction(FirstPageText, FirstPageImage), Columns.Count + 1,
                              PagerHorizontalAlign);
            list.AppendFormat("\r\n\t<a name=\"Prev\" id=\"Prev\" href=\"{0}\">{1}</a>",
                              PrevPageAction(), GetAction(PrevPageText, PrevPageImage));
            
            UserControlBase ucb = FindUserCtrl();
            list.AppendFormat("\r\n\t\t<span id=\"CurrentPage\" name=\"CurrentPage\">Strona {0} z {1}</span>", ucb.PageNumber, ucb.PageCount);
            list.AppendFormat("\r\n\t<a name=\"Next\" id=\"Next\" href=\"{0}\">{1}</a>",
                              NextPageAction(), GetAction(NextPageText, NextPageImage));
            list.AppendFormat("\r\n\t<a name=\"Last\" id=\"Last\" href=\"{0}\" urn=\"{0}\">{1}</a>",
                              LastPageAction(), GetAction(LastPageText, LastPageImage));
            list.Append("\r\n\t</td>\r\n</tr>");
            return list;
        }


        private string GetAction(string text, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return text;
            return string.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />", Page.ResolveUrl(imageUrl), text);
        }

        private string GetAlign(HorizontalAlign align)
        {
            switch (align)
            {
                case HorizontalAlign.Center:
                    return " align=\"center\"";
                case HorizontalAlign.Right:
                    return " align=\"right\"";
                default:
                    return string.Empty;
            }
        }

        private object GetValue(object entity, string member, TypeColumn type)
        {
            object value = entity;
            string[] memberParts = member.Split('.');
            for (int index = 0; index < memberParts.Length; index++)
            {
                value = value.GetType().GetProperty(memberParts[index]).GetValue(value, null);
                if (value == null) break;
            }
            if (type == TypeColumn.Bool)
            {
                if (Convert.ToBoolean(value))
                {
                    if (string.IsNullOrEmpty(TrueImage))
                    {
                        return "<input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\" />";
                    }
                    else
                    {
                        string path = Page.ResolveUrl(TrueImage);
                        return string.Format("<img src=\"{0}\" alt=\"{1}\" />", path, bool.TrueString);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(FalseImage))
                    {
                        return "<input type=\"checkbox\" disabled=\"disabled\" />";
                    }
                    else
                    {
                        string path = Page.ResolveUrl(FalseImage);
                        return string.Format("<img src=\"{0}\" alt=\"{1}\" />", path, bool.FalseString);
                    }
                }
            }
            else if (type == TypeColumn.Image)
            {
                if(value!=null)
                {
                    string path = Page.ResolveUrl(value.ToString());
                    return string.Format("<img src=\"{0}\" alt=\"{1}\" />", path, value);                    
                }
            }
            return value;
        }

        private string AddNewLink()
        {
            return ActionLink(ListActionType.AddNew, "0");
        }

        private string EditLink(int id)
        {
            return ActionLink(ListActionType.Edit, id.ToString());
        }

        private string SortLink(string member)
        {
            return ActionLink(ListActionType.Sort,member);
        }

        private string FirstPageAction()
        {
            return ActionLink(ListActionType.First, "0");
        }
        private string PrevPageAction()
        {
            return ActionLink(ListActionType.Prev, "0");
        }
        private string NextPageAction()
        {
            return ActionLink(ListActionType.Next, "0");
        }
        private string LastPageAction()
        {
            return ActionLink(ListActionType.Last, "0");
        }

        private string DeleteLink(int id, string text)
        {
            string lActionLink = ActionLink(ListActionType.Delete, id.ToString());
            if (string.IsNullOrEmpty(lActionLink)) return lActionLink;
            return string.Format("javascript:wlvConfirmDelete('{0}','{1}');", text,
                                 lActionLink.Replace("javascript:", "").Replace("'", "\\'"));
        }

        private void RegisterDelete()
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "Vulcan.WebControls.Delete",
                                                        @"function wlvConfirmDelete(text, action) {
	if (window.confirm('Czy napewno chcesz skasowaæ : ' + text + ' ?')) {
		eval(action);
	}
}",
                                                        true);
        }
    }
}