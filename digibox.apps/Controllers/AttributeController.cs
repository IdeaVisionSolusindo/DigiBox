using AutoMapper;
using digibox.apps.Models;
using digibox.apps.Modules;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Controllers
{
    public class AttributeController : Controller
    {
        private readonly IAttributeRepository _attribute;
        private readonly MapperConfiguration _mapperConfig;

        // GET: Attribute

        public AttributeController(IAttributeRepository attribute, MapperConfiguration mapperConfig) =>
             (_attribute, _mapperConfig) = (attribute, mapperConfig);

        public ActionResult Index()
        {
            return View();
        }


        #region UOM
        //GET UOM Atribute

        [Route("Master/UOM")]
        public ActionResult UOM()
        {
            return View();
        }

        
        public ActionResult OpenUOMData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<tmattribute, bool>> filter = f => (f.attributevalue.Contains(searchString));

            var mdl = _attribute.GetByName("UOM");
            mdl = mdl.Where(filter).OrderBy(f => f.attributevalue);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            Mapper mapper = new Mapper(_mapperConfig);
            var model = mapper.Map<List<AttributeModel>>(mdl);

            ViewData["currentPage"] = page;
            return PartialView("_openUOMData", model.ToPagedList(page ?? 1, myGlobal.PageSize));

        }

        #endregion

        #region Movement Type

        [Route("Master/MovementType")]
        public ActionResult MovementType()
        {
            return View();
        }


        public ActionResult OpenMovementTypeData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<tmattribute, bool>> filter = f => (f.attributevalue.Contains(searchString));

            var mdl = _attribute.GetByName("MOVEMENT-TYPE");
            mdl = mdl.Where(filter).OrderBy(f => f.attributevalue);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            Mapper mapper = new Mapper(_mapperConfig);
            var model = mapper.Map<List<AttributeModel>>(mdl);

            ViewData["currentPage"] = page;
            return PartialView("_openMovementTypeData", model.ToPagedList(page ?? 1, myGlobal.PageSize));

        }
        #endregion

        #region Material Type

        [Route("Master/MaterialType")]
        public ActionResult MaterialType()
        {
            return View();
        }


        public ActionResult OpenMaterialTypeData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<tmattribute, bool>> filter = f => (f.attributevalue.Contains(searchString));

            var mdl = _attribute.GetByName("MATERIAL-TYPE");
            mdl = mdl.Where(filter).OrderBy(f => f.attributevalue);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            Mapper mapper = new Mapper(_mapperConfig);
            var model = mapper.Map<List<AttributeModel>>(mdl);

            ViewData["currentPage"] = page;
            return PartialView("_openMaterialTypeData", model.ToPagedList(page ?? 1, myGlobal.PageSize));

        }
        #endregion

        #region SBU

        [Route("Master/SBU")]
        public ActionResult SBU()
        {
            return View();
        }


        public ActionResult OpenSBUData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<tmattribute, bool>> filter = f => (f.attributevalue.Contains(searchString));

            var mdl = _attribute.GetByName("SBU");
            mdl = mdl.Where(filter).OrderBy(f => f.attributevalue);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            Mapper mapper = new Mapper(_mapperConfig);
            var model = mapper.Map<List<AttributeModel>>(mdl);

            ViewData["currentPage"] = page;
            return PartialView("_openSBUData", model.ToPagedList(page ?? 1, myGlobal.PageSize));

        }
        #endregion


        public ActionResult Add(string attrName)
        {

            AttributeModel model = new AttributeModel()
            {
                attributename = attrName
            };
            return PartialView("_add",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(AttributeModel attribute)
        {
            ResponseModel response = null;
            if (ModelState.IsValid)
            {
                try
                {
                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmattribute>(attribute);
                    dta.createdby = "USER";
                    dta.isshown = true;
                    var rst = _attribute.Create(dta);

                    response = new ResponseModel()
                    {
                        id = rst.id,
                        Messages = "success",
                        isSuccess = true
                    };


                }
                catch (Exception ex)
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = ex.Message,
                        isSuccess = false
                    };
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = messages,
                    isSuccess = false
                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {

            ResponseModel response = null;
            try
            {
                var rst = _attribute.Delete(id);
                response = new ResponseModel()
                {
                    id = id,
                    Messages = "success",
                    isSuccess = true
                };
            }
            catch (Exception ex)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = ex.Message,
                    isSuccess = false
                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(Guid id)
        {
            IMapper _mapper = new Mapper(_mapperConfig);
            var attribute = _attribute.FindById(id);
            var dta = _mapper.Map<AttributeModel>(attribute);
            return PartialView("_edit", dta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(AttributeModel attribute)
        {
            ResponseModel response = null;

            if (ModelState.IsValid)
            {
                try
                {
                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmattribute>(attribute);
                    dta.isshown = true;
                    dta.updatedby = "USER";
                    _attribute.Update(dta);
                    response = new ResponseModel()
                    {
                        id = attribute.id,
                        Messages = "success",
                        isSuccess = true
                    };
                }
                catch (Exception ex)
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = ex.Message,
                        isSuccess = false
                    };
                }
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                         .SelectMany(x => x.Errors)
                                         .Select(x => x.ErrorMessage));

                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = messages,
                    isSuccess = false
                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListSelectItem(string attrName)
        {
            var attribute = _attribute.GetByName(attrName);
            var result = (from a in attribute
                          select new SelectListItem()
                          {
                              Text = a.attributevalue,
                              Value = a.id.ToString()
                          }).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}