//using System;
//using System.Collections.Generic;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Bll;
//using Roberta.WebControls;
//
///// <summary>
///// Summary description for VDetailsView
///// </summary>
///// 
//public delegate void ModeChangeHandler (VDetailsView sender, DetailsViewMode newMode);
//
//public class VDetailsView: UserControl
//{
//    public event ModeChangeHandler modeChange;
//
//    public virtual Control GetItemTable()
//    {
//        throw new NotImplementedException("Musisz przeci¹¿yæ GetItemTable.");
//    }
//
//    public virtual Control GetEditTable()
//    {
//        throw new NotImplementedException("Musisz przeci¹¿yæ GetEditTable.");
//    }
//
//    public virtual Control GetInsertTable()
//    {
//        throw new NotImplementedException("Musisz przeci¹¿yæ GetInsertTable.");
//    }
//
//    public DetailsViewMode Mode
//    {
//        get 
//        {
//            if (ViewState["mode"] == null)
//                return DetailsViewMode.ReadOnly;
//
//            return (DetailsViewMode)ViewState["mode"]; 
//        }
//        private set { ViewState["mode"] = value;}
//    }
//
//    public int? Id
//    {
//        get { return (int?)ViewState["id"]; }
//        private set { ViewState["id"] = value; }
//    }
//
//    public virtual void InitData<T>(int? id, DetailsViewMode mode) where T : EntityBase
//    {
//        Id = id;
//        ChangeMode<T>(mode);
//    }
//
//    public virtual void ChangeMode<T>(DetailsViewMode mode) where T : EntityBase
//    {
//        Control itemTable = GetItemTable();
//        Control editTable = GetEditTable();
//        Control insertTable = GetInsertTable();
//
//        if (mode == DetailsViewMode.Insert)
//        {
//            itemTable.Visible = false;
//            editTable.Visible = false;
//            insertTable.Visible = true;
//        }
//        else if (mode == DetailsViewMode.Edit)
//        {
//            itemTable.Visible = false;
//            insertTable.Visible = false;
//            editTable.Visible = true;
//        }
//        else
//        {
//            editTable.Visible = false;
//            insertTable.Visible = false;
//            itemTable.Visible = true;
//        }
//
//        Mode = mode;
//        SetData<T>(Id, mode);
//
//        if (modeChange != null)
//            modeChange(this, mode);
//    }
//
//    public T GetEntity<T>() where T: EntityBase, new()
//    {
//        T entity = default(T);
//        if (Id.HasValue)
//            entity = Dm.Retrieve<T>(Id.Value);
//
//        if (entity == null)
//            entity = new T();
//
//        Control tab = GetActiveTable(Mode);
//        Copier.ToEntity(entity, tab, GetSuffix4Tab(tab));
//
//        return entity;
//    }
//
//    protected Control GetActiveTable(DetailsViewMode mode)
//    {
//        if (mode == DetailsViewMode.ReadOnly)
//            return GetItemTable();
//        else if (mode == DetailsViewMode.Edit)
//            return GetEditTable();
//        else
//            return GetInsertTable();
//    }
//
//    protected virtual void SetData<T>(int? id, DetailsViewMode mode) where T : EntityBase
//    {
//        T entity = id.HasValue ? Dm.Retrieve<T>(id.Value) : default(T);
//        Control tab = GetActiveTable(mode);
//        Copier.ToView(entity, tab, GetSuffix4Tab(tab));
//    }
//
//    protected string GetSuffix4Tab(Control tab)
//    {
//		if (tab == GetItemTable())
//			return string.Empty;
//        else if (tab == GetEditTable())
//            return "Edit";
//        else if (tab == GetInsertTable())
//            return "Ins";
//        else
//            return string.Empty;
//    }
//    public virtual bool Validate<T>() where T : EntityBase, IValidable, new()
//    {
//        List<String> errors = new List<string>();
//        if (GetEntity<T>().Validate(errors))
//        {
//            return true;
//        }
//        foreach (string error in errors)
//        {
//            CustomValidator cv = new CustomValidator();
//            cv.ErrorMessage = error;
//            cv.IsValid = false;
//            cv.Visible = false;
//            Controls.Add(cv);
//        }
//        return false;
//    }
//
//}
