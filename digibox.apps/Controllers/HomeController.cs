using digibox.apps.Modules;
using digibox.data;
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
    public class HomeController : Controller
    {
        private readonly IRoleService _role;
        public IMessageService _message;
        private readonly IMaterialRepository _material;
        private readonly IMaterialPriceRepository _materialPrice;

        public HomeController(IRoleService role, IMessageService message, IMaterialRepository material, IMaterialPriceRepository materialPrice)
        {
            _role = role;
            _message = message;
            _material = material; 
            _materialPrice = materialPrice;
        }
        public ActionResult Index()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            if (role == userRole.COLLECTOR)
            {
                return IndexCollector();
            }
            if (role == userRole.ADMIN)
            {
                return IndexAdmin();
            }
            if (role == userRole.MANAGER)
            {
                return IndexManager();
            }
            return View();
        }

        private ActionResult IndexManager()
        {
            return View("IndexCollector");
        }

        private ActionResult IndexAdmin()
        {
            return View("IndexCollector");
        }

        private ActionResult IndexCollector()
        {
            return View("IndexCollector");
        }

        #region Common

        public ActionResult MessageCount()
        {
            var user = myGlobal.currentUser;
            var message = _message.getMessages(user.id).Count();
            return PartialView("_messageCount", message);
        }

        public ActionResult MaterialCount()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            int hasil=0;
            if (role == userRole.COLLECTOR)
            {
                var material = _material.GetMaterialByCollector(myGlobal.currentUser.id);
                hasil = material.Count();
            }
            else
            {
                var material = _material.GetAll();
                hasil = material.Count();
            }
            return PartialView("_materialCount", hasil);
        }

        public ActionResult MinimumMaterial()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            IQueryable<MaterialListModel> material;

            if (role == userRole.COLLECTOR)
            {
                 material = _material.MinimumMaterial(myGlobal.currentUser.id);
            }
            else
            {
                 material = _material.MinimumMaterial(null);
            }
            return PartialView("_minimumMaterial", material.ToList());
        }

        public ActionResult MinimumMaterialCount()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            IQueryable<MaterialListModel> material;

            if (role == userRole.COLLECTOR)
            {
                material = _material.MinimumMaterial(myGlobal.currentUser.id);
            }
            else
            {
                material = _material.MinimumMaterial(null);
            }
            return PartialView("_minimumMaterialCount", material.Count());
        }

        public ActionResult MaterialOverdueCount()
        {
            IQueryable<tmmaterialprice> material;
            var role = _role.getRoleByToken(myGlobal.usertoken);
            if (role == userRole.COLLECTOR)
            {
                material = _materialPrice.Overdue(7,myGlobal.currentUser.id);
            }
            else
            {
                material = _materialPrice.Overdue(7,null);
            }
            return PartialView("_materialOverdueCount", material.Count());
        }

        #endregion

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}