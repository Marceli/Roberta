using System;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bll;

using System.Collections.Generic;

namespace Roberta.WebControls
{
    public static class Copier
    {
        
        public static IList<T> GetEntityList<T>(Control contener) where T:EntityBase
        {
            List<T> list = new List<T>();
            foreach (Control item in contener.Controls)
            {
                if (item is CheckBox)
                {
                    int itemId = int.Parse(item.ID);
                    T listItem = Dm.Retrieve<T>(itemId);
                    list.Add(listItem);
                }
            }
            return list;
        }

        public static void ToView<T>(T entity, Control view, string suffix)
        {
            Type entityType = typeof(T);

            PropertyInfo[] pi = entityType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
            foreach (PropertyInfo info in pi)
            {
                Control ctrl = view.FindControl(GetCtrlName(info.Name, suffix));
                if (entity != null)
                {
                    if (ctrl is TextBox)
                    {
                        object val = info.GetValue(entity, null);
                        if (val != null)
                            ((TextBox)ctrl).Text = val.ToString();
                    }
                    else if (ctrl is Label)
                    {
                        object val = info.GetValue(entity, null);
                        if (val != null)
                            ((Label)ctrl).Text = val.ToString();
                    }
                    else if (ctrl is DropDownList)
                        if (!info.PropertyType.IsEnum)
                        {
                            object o = info.GetValue(entity, null);
                            if (o != null)
                            {
                                PropertyInfo pId = info.PropertyType.GetProperty("Id");
                                int id = (int)pId.GetValue(o, null);
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
                        DropDownList ddl = (DropDownList)ctrl;
                        if (ddl.Items.Count > 0)
                            ddl.SelectedIndex = 0;
                    }
                }
            }
        }

        public static void ToEntity<T>(T entity, Control view, string suffix)
        {
            Type entityType = typeof(T);
            PropertyInfo[] pi = entityType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
            foreach (PropertyInfo info in pi)
            {
                Control ctrl = view.FindControl(GetCtrlName(info.Name, suffix));
                if (ctrl is TextBox)
                    info.SetValue(entity, GetTypedValue(info.PropertyType, ((TextBox)ctrl).Text), null);
                else if (ctrl is Label)
                    info.SetValue(entity, GetTypedValue(info.PropertyType, ((Label)ctrl).Text), null);
                else if (ctrl is DropDownList)
                {
                    if (!info.PropertyType.IsEnum)
                    {
                        int res ;
                        int.TryParse(((DropDownList)ctrl).SelectedValue, out res);

                        Object fromDb = Dm.ObjectSpace.GetObject(info.PropertyType, res);
                        info.SetValue(entity, fromDb, null);
                    }
                    else
                        info.SetValue(entity, GetTypedValue(info.PropertyType, ((DropDownList)ctrl).SelectedValue), null);
                }
            }
        }

        private static string GetCtrlName(string propertyName, string suffix)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(propertyName);
            sb.Append(suffix);
            return sb.ToString();
        }

        private static object GetTypedValue(Type t, string val)
        {
            if (t == typeof(string))
                return val;
            else if (t == typeof(int))
            {
                int res;
                if (int.TryParse(val, out res))
                    return res;

                return 0;
            }
            else if (t == typeof(decimal))
            {
                decimal res;
                decimal.TryParse(val, out res);
                return res;
            }
            else if (t == typeof(DateTime))
            {
                DateTime res;
                if (DateTime.TryParse(val, out res))
                    return res;

                return DateTime.Now;
            }
            else if (t.IsEnum)
            {
                    return Enum.Parse(t,val);
            }
            else
                throw new ArgumentException(string.Format("The type: {0} is not supported.", t.Name));
        }
    }
}
