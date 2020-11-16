using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace digibox.apps.Modules
{

    public class myAuthorizeAttribute : AuthorizeAttribute
    {
        public string ResourceKey { get; set; }
        
       public string OperationKey { get; set; }


        //Called when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new ContentResult();
            }
            //User isn't logged in
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "User", action = "Login" })
                );
            }
            //User is logged in but has no access
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "ErrorHandler", action = "UnAuthorize" })
                );
            }
        }

        //Core authentication, called before each action
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string strController = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            string strAction = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();


            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            //authCookie.Expires.AddDays(-1);

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                {
                    var roles = authTicket.UserData.Split(',');
                    //open user..
                    if (myGlobal.usertoken == null)
                    {
                        HttpContext.Current.Session["usertoken"] = authTicket.Name;
                        myGlobal.usertoken = authTicket.Name;
                    }
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                }
            }

            if (!httpContext.Request.IsAuthenticated)
                return false;
            else
            {
                //check apakah authorize atau tidak.
                return true;
            }
        }



        //No Annotation, user must be logged in
        //public ActionResult DoSomething( [DataSourceRequest]DataSourceRequest request ) 

        ////Custom authentication request
        //[myAuthorizeAttribute(ResourceKey="SomeResource",OperationKey="SomeAction")]
        //public ActionResult DoSomething( [DataSourceRequest]DataSourceRequest request ) 

        ////No Authentication at all
        //[AllowAnonymous]
        //public ActionResult DoSomething( [DataSourceRequest]DataSourceRequest 
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PreventDuplicateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request["__RequestVerificationToken"] == null)
                return;

            var currentToken = HttpContext.Current.Request["__RequestVerificationToken"].ToString();
            if (currentToken != null)
            {
                myGlobal.usertoken = currentToken;
            }

            if (HttpContext.Current.Session["LastProcessedToken"] == null)
            {
                HttpContext.Current.Session["LastProcessedToken"] = currentToken;
                return;
            }

            lock (HttpContext.Current.Session["LastProcessedToken"])
            {
                var lastToken = HttpContext.Current.Session["LastProcessedToken"].ToString();

                if (lastToken == currentToken)
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError("", "Looks like you accidentally tried to double post.");
                    return;
                }

                HttpContext.Current.Session["LastProcessedToken"] = currentToken;
            }
        }
    }
}