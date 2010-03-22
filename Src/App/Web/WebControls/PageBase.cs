//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace Roberta.WebControls
{
	public class PageBase : Page
	{
		public ActionInfo ActionInfo { get; private set; }

//		private Core.Page page {
//			get { return Controller.Page; }
//		}
//
//		public Profile Profile {
//			get { return Profile.Current; }
//		}
//
//		public string Description {
//			get { return string.IsNullOrEmpty(page.Description) ? page.Portal.Description : page.Description; }
//		}
//
//		public string Keywords {
//			get { return string.IsNullOrEmpty(page.Keywords) ? page.Portal.Keywords : page.Keywords; }
//		}
//
//		public Core.Skin Skin {
//			get { return this.page.Skin ?? this.page.Portal.Skin; }
//		}
//
//		private Core.Theme theme {
//			get { return this.Profile.Theme ?? this.page.Portal.Theme; }
//		}

		public void RefreshPage() {
			this.Response.Redirect(this.Request.Path, true);
		}

		protected override void OnPreInit(EventArgs e) {
			base.OnPreInit(e);
			this.Response.Expires = 0;
			this.EnableViewState = true;
            this.ViewStateEncryptionMode = ViewStateEncryptionMode.Never;
//			this.Theme = this.theme.Folder;
//			this.MasterPageFile = this.Skin.Path;
			this.ActionInfo = new ActionInfo(this);
		}

//		protected override void OnInit(EventArgs e) {
//			base.OnInit(e);
//			this.Master.ID = "WP";
//			this.Header.Title = page.Title;
//			this.Header.Controls.Add(new LiteralControl("\r\n"));
//			this.AddMetaTag("description", this.Description);
//			this.AddMetaTag("keywords", this.Keywords);
//			this.AddMetaTag("generator", "WilsonWebPortal (http://www.WilsonWebPortal.com)");
//			if (!string.IsNullOrEmpty(this.page.Portal.FavIcon)) {
//				this.AddLinkTag("shortcut icon", this.ResolveUrl(this.page.Portal.FavIcon));
//			}
//
//			if (this.page.Id != Controller.Portal.HomePage.Id) {
//				this.AddParts(Controller.Portal.HomePage.Parts, true);
//			}
//			this.AddParts(this.page.Parts, false);
//		}
//
//		private void AddParts(IList<Core.Part> parts, bool global) {
//			foreach (Core.Part part in parts) {
//				try {
//					if (global && !part.IsGlobal) continue;
//					string editPartUrl = this.EditPartUrl(part);
//					bool editPart = !string.IsNullOrEmpty(editPartUrl);
//					bool skipPart = (!part.IsActive || !part.IsInRole(this.User));
//					if (skipPart && !editPart) continue;
//
//					Control module = this.InitModule(part, ref skipPart);
//					if (skipPart && !editPart) continue;
//
//					FrameBase frame = this.InitFrame(part, skipPart, editPart, editPartUrl);
//					if (!skipPart) frame.Content.Controls.Add(module);
//					Control zone = this.Master.FindControl(part.Zone.Name);
//					if (zone != null)	zone.Controls.Add(frame);
//				}
//				catch (Exception exception) {
//					try {
//						ErrorEvent.Raise(ErrorEvent.ErrorType.ErrorWithPart, exception, part.Id);
//					}
//					catch { }
//				}
//			}
//		}
//
//		private Control InitModule(Core.Part part, ref bool skipPart) {
//			Control module = null;
//			if (part.Module.Path.EndsWith(".ascx")) {
//				module = this.LoadControl(part.Module.Path);
//			}
//			else {
//				Type type = Types.GetType(part.Module.Path);
//				module = this.LoadControl(type, null);
//			}
//			module.ID = "Part" + part.Id;
//			ModuleBase moduleBase = module as ModuleBase;
//			if (moduleBase != null) {
//				if (moduleBase.Suppress) {
//					skipPart = true;
//				}
//				else {
//					moduleBase.Part = part;
//				}
//			}
//			return module;
//		}
//
//		private FrameBase InitFrame(Core.Part part, bool skipPart, bool editPart, string editPartUrl) {
//			Core.Frame partFrame = (part.Frame ?? this.page.Portal.Frame);
//			FrameBase frame = this.LoadControl(partFrame.Path) as FrameBase;
//			frame.Initialize(part, skipPart, editPart, editPartUrl);
//			return frame;
//		}
//
//		private string EditPartUrl(Core.Part part) {
//			Core.Part partSetup = Core.Portal.Current.PartPart;
//			if (partSetup == null) return string.Empty;
//			if (!partSetup.Page.IsInRole(this.User) || !partSetup.IsInRole(this.User)) return string.Empty;
//			return this.ActionInfo.ActionUrl(partSetup, "edit", part.Id);
//		}
//
//		private void AddMetaTag(string name, string content) {
//			HtmlMeta metaTag = new HtmlMeta();
//			metaTag.Name = name;
//			metaTag.Content = content;
//			this.Header.Controls.Add(metaTag);
//			this.Header.Controls.Add(new LiteralControl("\r\n"));
//		}
//
//		private void AddLinkTag(string rel, string href) {
//			HtmlLink linkTag = new HtmlLink();
//			linkTag.Attributes.Add("rel", rel);
//			linkTag.Href = href;
//			this.Header.Controls.Add(linkTag);
//			this.Header.Controls.Add(new LiteralControl("\r\n"));
//		}

		static public NameValueCollection ParseNameValues(string nameValues) {
		    
			NameValueCollection collection = new NameValueCollection();
            if (nameValues == null)
                return collection;
			string[] pairs = nameValues.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string nameValue in pairs) {
				string[] parts = nameValue.Split(new char[] { '=' }, 2);
				string name = parts[0].ToLower();
				string value = (parts.Length == 2 ? parts[1] : null);
				collection.Add(name, value);
			}
			return collection;
		}
	}
}
