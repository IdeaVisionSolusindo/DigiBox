using digibox.apps.Modules;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new myAuthorizeAttribute());
        }
    }
}
