using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Modules
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString IsActive(this HtmlHelper htmlHelper, string action, string controller, string activeClass, string inActiveClass = "")
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"].ToString();
            var routeAction = routeData.Values["action"].ToString();

            var returnActive = (controller == routeController && action == routeAction);

            return new MvcHtmlString(returnActive ? activeClass : inActiveClass);
        }

        public static MvcHtmlString IsGroupActive(this HtmlHelper htmlHelper, string[] action, string[] controller, string activeClass, string inActiveClass = "")
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"].ToString();

            var routeAction = routeData.Values["action"].ToString();
            bool isActive = false;
            for (int j = 0; j < controller.Length; j++)
            {
                for (int i = 0; i < action.Length; i++)
                {
                    isActive = (controller[j] == routeController && action[i] == routeAction);
                    if (isActive)
                        break;
                }
                if (isActive)
                    break;
            }
            return new MvcHtmlString(isActive ? activeClass : inActiveClass);
        }
    }
}