//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Roberta.WebControls
{
    public class ActionInfo
    {
        private NameValueCollection actionValues = null;
        private string controlId = "";
        private string action = string.Empty;
        private int actionId = -1;
        private string actionString = string.Empty;
        private Page page;
       


        public ActionInfo(Page page)
        {
            this.page = page;
            ParseAction();
        }


        public string ControlID
        {
            get { return controlId; }
        }
        public string Action
        {
            get { return action; }
        }

        public int ActionId
        {
            get { return actionId; }
        }

        public string ActionString
        {
            get { return actionString; }
        }

        public string ActionUrl(Control control, string pAction)
        {
            //if (!part.IsInAction(page.User, action)) return string.Empty;
            return page.ResolveUrl(string.Format("{0}?part={1}&action={2}",
                                                 control.Page.ResolveUrl(string.Empty), control.ID, pAction.ToLower()));
        }

        public string ActionUrl(Control control, string pAction, int pActionId)
        {
            //if (!part.IsInAction(page.User, action)) return string.Empty;
            string hash = page.Server.UrlEncode(HashAction(control.ID, pAction.ToLower(), pActionId));
//            return page.ResolveUrl(string.Format("{0}?part={1}&action={2}&id={3}&key={4}",
//                                     control.Page.ResolveUrl(string.Empty), control.ID, pAction.ToLower(), pActionId, hash));
            string result = string.Format("{4}?part={0}&action={1}&id={2}&key={3}",
                                                  control.ID, pAction.ToLower(), pActionId, hash,page.Request.Path);
            return result;
        }

        public string ActionEvent(Control control, string pAction)
        {
            string formArg = string.Format("part={0}&action={1}", control.ID, pAction.ToLower());
            return page.ClientScript.GetPostBackClientHyperlink(page, formArg);
        }

        public string ActionEvent(Control control, string pAction, int pActionId)
        {
            return ActionEvent(control, pAction, pActionId.ToString());
        }

        public string ActionEvent(Control control, string pAction, string pActionStr)
        {
           
            string hash = HashAction(control.ID, pAction.ToLower(), pActionStr);
            string formArg = string.Format("part={0}&action={1}&id={2}&key={3}",
                                           control.ID, pAction.ToLower(), pActionStr, hash);
            return page.ClientScript.GetPostBackClientHyperlink(page, formArg);
        }

        private void ParseAction()
        {
            
            actionValues = GetActionValues();
            controlId=actionValues["part"] ?? "";
           
            action = (actionValues["action"] ?? string.Empty).ToLower();
            actionString = actionValues["id"] ?? string.Empty;

            if (controlId.Length > 0 && !string.IsNullOrEmpty(action))
            {
                int lActionId;
                int.TryParse(actionValues["id"] ?? "-1", out lActionId);
                string hash = actionValues["key"] ?? string.Empty;
                string key = HashAction(controlId, action, lActionId);
                
                if (!string.IsNullOrEmpty(hash) && key.Equals(hash.Substring(0,key.Length)))
                {
                    actionId = lActionId;
                }
            }
        }

        private NameValueCollection GetActionValues()
        {
            NameValueCollection lActionValues = null;
            if (page.IsPostBack)
            {
               
                string eventArgument = page.Request.Form["__EVENTARGUMENT"];
                lActionValues = PageBase.ParseNameValues(eventArgument);
                if (!string.IsNullOrEmpty(lActionValues["part"]) && !string.IsNullOrEmpty(lActionValues["action"]))
                {
                    //page.ClientScript.RegisterHiddenField("__EVENTARGUMENT", eventArgument);
                }
                else
                {
                    lActionValues = null;
                }
            }
            if (lActionValues == null)
            {
                if (!string.IsNullOrEmpty(Query))
                {
                    lActionValues = PageBase.ParseNameValues(Query);
                }
                else
                {
                    lActionValues = page.Request.QueryString;
                }
            }
            return lActionValues;
        }

        private string HashAction(string partId, string pAction, int pActionId)
        {
            return HashAction(partId, pAction, pActionId.ToString());
        }

        private string HashAction(string partId, string pAction, string pActionStr)
        {
            string key = partId + pAction.ToLower() + pActionStr;
            byte[] saltedKey = Encoding.ASCII.GetBytes(key);
            byte[] hash = HashAlgorithm.Create("MD5").ComputeHash(saltedKey);
            return Convert.ToBase64String(hash);
        }
 
        public static string Query
        {
            get { return HttpContext.Current.Items[KEY_Query] as string; }
    //        private set { HttpContext.Current.Items[KEY_Query] = value; }
        }



        //private static readonly string KEY_Page = "Wilson.WebPortal.Page";
        private static readonly string KEY_Query = "Wilson.WebPortal.Query";
        // private static readonly string KEY_Status = "Wilson.WebPortal.Status";


    }
}