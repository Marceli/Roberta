using System;
using Castle.Windsor;
using Core.Data;
using System.Web;
using NHibernate;

namespace Web
{
	public class Global : HttpApplication
	{
		private SessionProvider sessionProvider;
		public override void Init()
		{
			base.Init();
			sessionProvider = new SessionProvider();
			BeginRequest += BeginRequestHandler;
			EndRequest += EndRequestHandler;
		}

		private void EndRequestHandler(object sender, EventArgs e)
		{
			((ITransaction)HttpContext.Current.Items["dbTransaction"]).Commit();
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}
		private static ISession DbSession { get { return (ISession)HttpContext.Current.Items["dbSession"]; } }
		protected void BeginRequestHandler(object sender, EventArgs e)
		{

			HttpContext.Current.Items["dbSession"] = sessionProvider.Factory.OpenSession();
			HttpContext.Current.Items["dbTransaction"] = DbSession.BeginTransaction(); ;
		}



		

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}