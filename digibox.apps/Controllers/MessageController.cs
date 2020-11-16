using digibox.apps.Modules;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IMessageService _message;
        private readonly IUserRepository _user;
        private readonly IRoleService _role;
        private readonly INotificationService _notification;

        public MessagesController(IMessageService message, INotificationService notification, IUserRepository user, IRoleService role) => 
            (_message, _notification, _user, _role) = (message, notification, user, role);

        // GET: Messages

        public ActionResult Index()
        {
            var usr = _user.getByToken(myGlobal.usertoken);
            var model = _message.getMessages(usr.id);
            return View(model);
        }

        public ActionResult showNewMessage()
        {
            var usr = _user.getByToken(myGlobal.usertoken);
            var model = _message.getMessages(usr.id).Take(10).ToList();
            return PartialView("_showNewMessage", model);
        }

        public ActionResult messageCount()
        {
            var usr = _user.getByToken(myGlobal.usertoken);
            var model = _message.getMessages(usr.id).Count();
            return PartialView("_messageCount", model);
        }


        public ActionResult Notification()
        {
            var currentrole = _role.getRoleByToken(myGlobal.usertoken);
            List<NotificationModel> model = new List<NotificationModel>();
            if (currentrole == userRole.COLLECTOR)
            {
                model =  NotificationCollector();
            }
            else
            {
                model = NotificationAdmin();
            }

            /*if (currentrole == userRole.PPJK)
            {
                return WarningListPPJK();
            }

            if (currentrole == userRole.SELLER)
            {
                return WarningListPPJK();
            }

            if (currentrole == userRole.MARINE)
            {
                return WarningListPPJK();
            }

            if (currentrole == userRole.ADMINISTRATOR)
            {
                return null;
            }*/

            return PartialView("_notification",model);
        }

        private List<NotificationModel> NotificationAdmin()
        {
            var dta = new List<NotificationModel>();
            dta.Add(_notification.PriceDueNotification());
            return dta;
        }

        private List<NotificationModel> NotificationCollector()
        {
            var dta = new List<NotificationModel>();
            dta.Add(_notification.PriceCollectorDueNotification(myGlobal.currentUser.id));
            return dta;
        }

        
    }
}