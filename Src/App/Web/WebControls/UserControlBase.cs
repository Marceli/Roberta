//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Roberta.WebControls
{
    public delegate void OnEditRow(int rowId);
    public delegate void OnDeleteRow(int rowId);

    public abstract class UserControlBase : UserControl
    {
        private string sortMember = "";
        private bool sortAscending = true;
        private string filtrValue = "";
        private string filtrMember = "";
        private int pageNumber = 1;

        private event OnEditRow editRow;
        private event OnDeleteRow deleteRow;

		public OnEditRow EditRow
		{
			get { return editRow; }
			set { editRow = value; }
		}

		public OnDeleteRow DeleteRow
		{
			get { return deleteRow; }
			set { deleteRow = value; }
		}

        public string FiltrMember
        {
            get { return filtrMember; }
            set
            {
                if (filtrMember != value)
                {
                    filtrMember = value;
                    this.PageNumber = 1;
                }
            }
        }

        public string FiltrValue
        {
            get { return filtrValue; }
            set
            {
                if (filtrValue != value)
                {
                    filtrValue = value;
                    this.PageNumber = 1;
                }
            }
        }
        
        

       protected int PrevPage
       {
           get { return (PageNumber > 1 ? PageNumber - 1 : 1); }
       }

       protected int NextPage
       {
           get { return (PageNumber < PageCount ? PageNumber + 1 : PageCount); }
       }
       private int pageCount;

       public int PageCount
       {
           get { return pageCount; }
           set { pageCount = value; }
       }
       public new PageBase Page
       {
           get { return base.Page as PageBase; }
       }
/*
        private bool isEditing = false;

        public bool IsEditing
        {
            get { return isEditing; }
            set { isEditing=value; }
        }
 */ 
        protected virtual void SetupEdit(EditSetup editSetup)
        {
            throw new Exception("This method must be overriden in a concrete implementation.");
        }
        protected virtual void DoEdit()
        {
           throw new Exception("This method must be overriden in a concrete implementation.");
        }
        
        public string SortMember
        {
            get { return sortMember; }
            set
            {
                
                if (SortMember == value)
                {
                    SortAscending = !SortAscending;
                }
                else
                {
                    sortMember = value;
                    sortAscending = true;
                }
                
            }
        }
        
        
       
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //isEditing = ViewState[this.ID + "_IsEditing"] == null ? false : (bool)ViewState[this.ID + "_IsEditing"];
            sortMember = ViewState[this.ID + "_SortMember"] == null ? "" : ViewState[this.ID + "_SortMember"].ToString();
            sortAscending = ViewState[this.ID + "_SortAscending"] == null ? true : (bool)ViewState[this.ID + "_SortAscending"];
            filtrValue = ViewState[this.ID + "_FiltrValue"] == null ? "" : ViewState[this.ID + "_FiltrValue"].ToString();
            filtrMember = ViewState[this.ID + "_FiltrMember"] == null ? "" : ViewState[this.ID + "_FiltrMember"].ToString();
            pageNumber = ViewState[this.ID + "_PageNumber"] == null ? 1 : (int)ViewState[this.ID + "_PageNumber"];
           
            

            
            if (this.Action.Equals("edit") && this.ActionId >= 0 )
            {
//                this.isEditing = true;
                this.DoEditAction();

            }else if (this.Action.Equals("sort"))
            {
                SortMember = this.ActionStr;
                this.PageNumber = 1;
                
            }
            else if (this.Action.Equals("delete") && this.ActionId >= 0)
            {
                try
                {
                    this.DeleteEntity(this.ActionId);
                   // this.RefreshPage();
                }
                catch (Exception exception)
                {
                    GetCtrlContainer().Controls.AddAt(0, new LiteralControl(string.Format(
                                                                  "<div align=\"center\" class=\"editWarning\">{0}</div><br />", exception.Message)));
                }
            }
            else
            {
                if (this.Action.Equals("page") && this.ActionId >= 0)
                    PageNumber = this.ActionId;
                
            }

//            if(!isEditing)
//            {
                AddCtrl(this.ListView());                
//            }
          
        }
				
			
        private Control ReadOnlyView()
        {
 	        EditView editView = new EditView();
            editView.ID = "EditView";
            editView.AddControls = new AddEditControls(this.SetupReadOnly);
            editView.SaveEntity = new SaveEditEntity(this.SaveEntity);
            editView.CloseEditView = new CloseEditView(this.CloseEdit);
            return editView;
        }

        protected virtual void SetupReadOnly(EditSetup editSetup)
        {
            throw new NotImplementedException();
        }
    
        protected void DeleteEntity(int id)
        {
            if (deleteRow != null)
                deleteRow(id);
        }

        protected virtual void SetupList(ListSetup listSetup)
        {
            throw new Exception("This method must be overriden in a concrete implementation.");
        }

        protected virtual WebControl GetCtrlContainer()
        {
            return null;
            //throw new NotImplementedException("This method must be overriden in a concrete implementation.");
        }

        protected void DoEditAction()
        {
            if (editRow != null)
                editRow(ActionId);
        }
         
        private Control EditView()
        {
//            isEditing = true;
            EditView editView = new EditView();
            editView.ID = "EditView";
            editView.AddControls = new AddEditControls(this.SetupEdit);
            editView.SaveEntity = new SaveEditEntity(this.SaveEntity);
            editView.CloseEditView = new CloseEditView(this.CloseEdit);
            return editView;
        }

        
        protected virtual string ListAction(ListActionType listAction, string id)
        {
            switch (listAction)
            {
                case ListActionType.Prev: return this.ActionInfo.ActionEvent(this,"Page", PrevPage);
                case ListActionType.Next: return this.ActionInfo.ActionEvent(this,"Page", NextPage);
                case ListActionType.First: return this.ActionInfo.ActionEvent(this,"Page", 1);
                case ListActionType.Last: return this.ActionInfo.ActionEvent(this,"Page", PageCount);
                case ListActionType.AddNew: return this.ActionInfo.ActionEvent(this,"edit", 0);
                case ListActionType.Edit: return this.ActionInfo.ActionEvent(this,"edit", id);
                case ListActionType.Delete: return this.ActionInfo.ActionEvent(this,"delete", id);
               case ListActionType.Sort: return this.ActionInfo.ActionEvent(this,"sort", id);
                
                default: throw new Exception("The specified listaction is not currently configured: " + listAction.ToString());
            }
        }

        protected ListView ListView()
        {
            ListView listView = new ListView();
            listView.ID = "ListView";
        
            listView.SetupList = new SetupListView(SetupList);        
            listView.ActionLink = new ListActionLink(ListAction);
            lista = listView;
            return listView;
        }
        private ListView lista;
        public ListView Lista
        {
            get { return lista; }
        }
        
        public void RefreshPage()
        {
          //  this.Page.RefreshPage();
        }
      
       
        protected ActionInfo ActionInfo
        {
            get { return this.Page.ActionInfo; }
        }

        
       

        public int ActionId
        {
            get { return  this.ActionInfo.ActionId ; }
        }

        public string ActionStr
        {
            get { return this.ActionInfo.ActionString ; }
        }

      

        public bool SortAscending
        {
            get { return sortAscending; }
            set { sortAscending = value; }
        }

       

        
        public int PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; }
        }

      


        public string ActionUrl(string action)
        {
            return this.ActionInfo.ActionUrl(this, action);
        }

        public string ActionUrl(string action, int actionId)
        {
            return this.ActionInfo.ActionUrl(this, action, actionId);
        }

        


        public string DeleteEvent(int actionId, string text)
        {
            string deleteEvent = this.ActionInfo.ActionEvent(this,"delete", actionId);
            if(string.IsNullOrEmpty(deleteEvent)) return deleteEvent;
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Wilson.WebPortal.Delete",
@"function wpConfirmDelete(text, action) {
	if (window.confirm('Are you sure you want to delete: ' + text + ' ?')) {
		eval(action);
	}
}", true);
            return string.Format("javascript:wpConfirmDelete('{0}','{1}');", text,
                deleteEvent.Replace("javascript:", "").Replace("'", "\\'"));
        }
        
		protected virtual void SaveEntity(EditValues editValues) {
			throw new Exception("This method must be overriden in a concrete implementation.");
		}

		private void CloseEdit(bool entitySaved) {
			this.RefreshPage();
		}

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            //Zapisuje stan obiektu jesli jest wyœwietlana lista to ona powinna zapisywaæ ze wzglêdu na kolejnoœæ zdarzeñ 
//            if(IsEditing)
                SaveState();
           
        }
        

        public string Action
        {
            get { return (this.HasAction ? this.ActionInfo.Action : string.Empty); }
        }
        public bool HasAction
        {
            get { return this.ID == this.ActionInfo.ControlID; }
        }


        private void AddCtrl(Control ctrl)
        {
            WebControl c = GetCtrlContainer();
            if (c==null)
                this.Controls.Add(ctrl);
            else
                c.Controls.Add(ctrl);
        }

        internal void SaveState()
        {
//            ViewState[this.ID + "_IsEditing"] = IsEditing;
            ViewState[this.ID + "_SortMember"] = SortMember;
            ViewState[this.ID + "_SortAscending"] = SortAscending;
            ViewState[this.ID + "_FiltrValue"] = FiltrValue;
            ViewState[this.ID + "_FiltrMember"] = FiltrMember;
            ViewState[this.ID + "_PageNumber"] = PageNumber;
        }
    }
}