using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bll;
using Web;
using Web.WebControls;

/// <summary>
/// Summary description for VDetailsView
/// </summary>
/// 
public delegate void ModeChangeHandler(VDetailsView sender, DetailsViewMode newMode);

public class VDetailsView : UserControl
{
	public event ModeChangeHandler modeChange;

	public virtual Control GetItemTable()
	{
		throw new NotImplementedException("Musisz przeci¹¿yæ GetItemTable.");
	}

	public virtual Control GetEditTable()
	{
		throw new NotImplementedException("Musisz przeci¹¿yæ GetEditTable.");
	}

	public virtual Control GetInsertTable()
	{
		throw new NotImplementedException("Musisz przeci¹¿yæ GetInsertTable.");
	}

	public DetailsViewMode Mode
	{
		get
		{
			if (ViewState["mode"] == null)
				return DetailsViewMode.ReadOnly;

			return (DetailsViewMode)ViewState["mode"];
		}
		private set { ViewState["mode"] = value; }
	}

	public int? Id
	{
		get { return (int?)ViewState["id"]; }
		private set { ViewState["id"] = value; }
	}

	public virtual void InitData<T>(int? id, DetailsViewMode mode) 
	{
		Id = id;
		ChangeMode<T>(mode);
	}

	public virtual void ChangeMode<T>(DetailsViewMode mode) 
	{
		var itemTable = GetItemTable();
		var editTable = GetEditTable();
		var insertTable = GetInsertTable();

		if (mode == DetailsViewMode.Insert)
		{
			itemTable.Visible = false;
			editTable.Visible = false;
			insertTable.Visible = true;
		}
		else if (mode == DetailsViewMode.Edit)
		{
			itemTable.Visible = false;
			insertTable.Visible = false;
			editTable.Visible = true;
		}
		else
		{
			editTable.Visible = false;
			insertTable.Visible = false;
			itemTable.Visible = true;
		}

		Mode = mode;
		SetData<T>(Id, mode);

		if (modeChange != null)
			modeChange(this, mode);
	}

	public T GetEntity<T>() where T:new()
	{
		var entity = default(T);
		if (Id.HasValue)
			entity = Db.Session.Get<T>(Id.Value);

		if (entity == null)
			entity = new T();

		var tab = GetActiveTable(Mode);
		Copier.ToEntity(entity, tab, GetSuffix4Tab(tab));

		return entity;
	}

	protected Control GetActiveTable(DetailsViewMode mode)
	{
		switch (mode)
		{
			case DetailsViewMode.ReadOnly:
				return GetItemTable();
			case DetailsViewMode.Edit:
				return GetEditTable();
			default:
				return GetInsertTable();
		}
	}

	protected virtual void SetData<T>(int? id, DetailsViewMode mode) 
	{
		T entity;
		if(id==null || id==0)
		{
			entity=(T)Activator.CreateInstance(typeof (T));
			Db.Session.Save(entity);
		}
		entity = Db.Session.Get<T>(id.Value) ;
		var tab = GetActiveTable(mode);
		Copier.ToView(entity, tab, GetSuffix4Tab(tab));
	}

	protected string GetSuffix4Tab(Control tab)
	{
		if (tab == GetItemTable())
			return string.Empty;
		if (tab == GetEditTable())
			return "Edit";
		if (tab == GetInsertTable())
			return "Ins";
		return string.Empty;
	}
	public virtual bool Validate<T>()  where T:IValidable, new()
	{
		var errors = new List<string>();
		if (GetEntity<T>().Validate(errors))
		{
			return true;
		}
		foreach (var error in errors)
		{
			var cv = new CustomValidator {ErrorMessage = error, IsValid = false, Visible = false};
			Controls.Add(cv);
		}
		return false;
	}

}
