using System;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Collections.Generic;
using FreeTextBoxControls;

namespace Web.WebControls
{
	public static class Copier
	{
        
		public static IList<T> GetEntityList<T>(Control contener) 
		{
			var list = new List<T>();
			foreach (Control item in contener.Controls)
			{
				if (item is CheckBox)
				{
					var itemId = int.Parse(item.ID);
					var listItem = Db.Session.Get<T>(itemId);
					list.Add(listItem);
				}
			}
			return list;
		}

		public static void ToView<T>(T entity, Control view, string suffix)
		{
			var entityType = typeof(T);

			var pi = entityType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
			foreach (var info in pi)
			{
				var ctrl = view.FindControl(GetCtrlName(info.Name, suffix));
				if (entity != null)
				{
					if (ctrl is TextBox)
					{
						var val = info.GetValue(entity, null);
						if (val != null)
							((TextBox)ctrl).Text = val.ToString();
					}
					else if (ctrl is Label)
					{
						var val = info.GetValue(entity, null);
						if (val != null)
							((Label)ctrl).Text = val.ToString();
					}
					else if (ctrl is DropDownList)
						if (!info.PropertyType.IsEnum)
						{
							var o = info.GetValue(entity, null);
							if (o != null)
							{
								var pId = info.PropertyType.GetProperty("Id");
								var id = (int)pId.GetValue(o, null);
								((DropDownList)ctrl).SelectedValue = id.ToString();
							}
						}
						else
							((DropDownList)ctrl).SelectedValue = info.GetValue(entity, null).ToString();
				}
				else //entity == null
				{
					if (ctrl is TextBox)
						((TextBox)ctrl).Text = string.Empty;
					else if (ctrl is Label)
						((Label)ctrl).Text = string.Empty;
					else if (ctrl is DropDownList)
					{
						var ddl = (DropDownList)ctrl;
						if (ddl.Items.Count > 0)
							ddl.SelectedIndex = 0;
					}
				}
			}
		}

		public static void ToEntity<T>(T entity, Control view, string suffix)
		{
			var entityType = typeof(T);
			var pi = entityType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
			foreach (var info in pi)
			{
				var ctrl = view.FindControl(GetCtrlName(info.Name, suffix));
				if (ctrl is TextBox)
					info.SetValue(entity, GetTypedValue(info.PropertyType, ((TextBox)ctrl).Text), null);
				else if (ctrl is FreeTextBox)
					info.SetValue(entity, GetTypedValue(info.PropertyType, ((FreeTextBox)ctrl).Text), null);
				else if (ctrl is Label)
					info.SetValue(entity, GetTypedValue(info.PropertyType, ((Label)ctrl).Text), null);
				else if (ctrl is DropDownList)
				{
					if (!info.PropertyType.IsEnum)
					{
						int res ;
						int.TryParse(((DropDownList)ctrl).SelectedValue, out res);

						var fromDb = Db.Session.Get(info.PropertyType, res);
						info.SetValue(entity, fromDb, null);
					}
					else
						info.SetValue(entity, GetTypedValue(info.PropertyType, ((DropDownList)ctrl).SelectedValue), null);
				}
			}
		}

		private static string GetCtrlName(string propertyName, string suffix)
		{
			var sb = new StringBuilder();
			sb.Append(propertyName);
			sb.Append(suffix);
			return sb.ToString();
		}

		private static object GetTypedValue(Type t, string val)
		{
			if (t == typeof(string))
				return val;
			if (t == typeof(int))
			{
				int res;
				return int.TryParse(val, out res) ? res : 0;
			}
			if (t == typeof(decimal))
			{
				decimal res;
				decimal.TryParse(val, out res);
				return res;
			}
			if (t == typeof(DateTime))
			{
				DateTime res;
				return DateTime.TryParse(val, out res) ? res : DateTime.Now;
			}
			if (t.IsEnum)
			{
				return Enum.Parse(t,val);
			}
			throw new ArgumentException(string.Format("The type: {0} is not supported.", t.Name));
		}
	}
}