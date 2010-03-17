using System;
using System.Web;

namespace Roberta.WebControls
{
    /// <summary>
    /// Informacje globalne dla sesji programu.
    /// </summary>
    public class GlobalSession
    {
        
        /// <summary>
        /// Gets information on current web user.
        /// The information is taken from the HttpContext.Current.User and the database.
        /// </summary>
        public static string CurrentWebUser
        {
            get
            {
                if (HttpContext.Current != null &&
                     HttpContext.Current.User != null)
                {
                    return HttpContext.Current.User.Identity.Name;
                }

/*
                if ( HttpContext.Current != null &&
                     HttpContext.Current.User != null )
                {
                    // czy jest zapamiêtana informacja w sesji?
                    if ( HttpContext.Current.Session[CURRENT_USER] != null )
                    {
                        // czy ta sama co w ciasteczku?
                        UUser cuser = (UUser)HttpContext.Current.Session[CURRENT_USER];
                        if ( cuser.Name == HttpContext.Current.User.Identity.Name )
                            return cuser;
                    }

                    UUser user = UUser.RetrieveScalar( "Name==?", HttpContext.Current.User.Identity.Name );
                    if ( user != null )
                    {
                        if ( HttpContext.Current.Session[CURRENT_USER] != null )
                            HttpContext.Current.Session.Remove(CURRENT_USER);
                        HttpContext.Current.Session[CURRENT_USER] = user;

                        return user;
                    }
                }
*/
                throw new ApplicationException( "B³¹d pobierania informacji o bie¿¹cym u¿ytkowniku aplikacji!" ); 
            }
        }
    }
}
