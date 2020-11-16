using AutoMapper;
using digibox.apps.Modules;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IRoleService _role;
        private readonly IMaterialRepository _material;
        private readonly IMaterialServices _materialServices;
        private readonly IInventoryRepository _inventory;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IDistributorRepository _distributor;

        // GET: Inventory

        public InventoryController(IMaterialRepository material, IMaterialServices materialServices, IInventoryRepository inventory, IDistributorRepository distributor, IRoleService role, MapperConfiguration mapperConfiguration) => 
            (_material, _materialServices, _inventory, _distributor, _role, _mapperConfiguration) = (material, materialServices, inventory, distributor, role, mapperConfiguration);
        public ActionResult Index()
        {
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Part No",
                Value = "partno"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Material Name",
                Value = "name",
                Selected = true
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Distributor",
                Value = "distributor"
            });

            ViewData["parameters"] = parameters;
            return View();
        }



        public ActionResult OpenData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<MaterialListModel, bool>> filter = f => (f.name.Contains(searchString));

            if ((paramType == "partno"))
            {
                filter = f => (f.partno.Contains(searchString));
            };

            if (paramType == "name")
            {
                filter = f => (f.name.Contains(searchString));
            }

            if (paramType == "movementtype")
            {
                filter = f => (f.movementtype == searchString);
            }

            if (paramType == "sbu")
            {
                filter = f => (f.sbu.Contains(searchString));
            }

            if (paramType == "materialtype")
            {
                filter = f => (f.materialtype == searchString);
            }

            if (paramType == "distributor")
            {
                filter = f => (f.distributor == searchString);
            }

            var role = _role.getRoleByToken(myGlobal.usertoken);
            IQueryable<MaterialListModel> mdl = null; 
            if (role == userRole.COLLECTOR)
            {

                //filter by user collector;
                 mdl = _material.GetMaterialByCollector(myGlobal.currentUser.id);
            }
            else
            {
                mdl = _material.GetMaterial();
            }

            mdl = mdl.Where(filter).OrderBy(x => x.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            //var model = mdl.OrderBy(x => x.name);
            var model = mdl.ToPagedList(page ?? 1, myGlobal.PageSize);
            var rst = (from m in model
                       select new MaterialListModel()
                       {
                           id = m.id,
                           maxstock = m.maxstock,
                           minstock = m.minstock,
                           name = m.name,
                           partno = m.partno,
                           description = m.description,
                           distributor = m.distributor,
                           unit = m.unit,
                           currentstock = m.currentstock
                       }).ToList();

            ViewData["currentPage"] = page;
            return PartialView("_openData", rst);

        }


        public ActionResult History(Guid id)
        {
            var material = _material.FindById(id);
            Mapper map = new Mapper(_mapperConfiguration);
            var model = map.Map<MaterialModel>(material);
            var distributor = _distributor.FindById(model.distributor??Guid.NewGuid());
            ViewData["distributor"] = distributor.name;
            return View(model);
        }
        public ActionResult DetailHistory(Guid id, string sortOrder = "ASC", int? page = 1)
        {
            if (page < 1)
                page = 1;

            var mdl = _inventory.GetByMaterial(id);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            ViewData["currentPage"] = page;
            var model = from m in mdl
                        select new InventoryModel()
                        {
                            amount = m.amount,
                            id = m.id,
                            inout = m.inout,
                            materialid = m.materialid,
                            createdat = m.createdat
                        };

            return PartialView("_detailHistory", model.ToPagedList(page ?? 1, myGlobal.PageSize));

        }
    }
}