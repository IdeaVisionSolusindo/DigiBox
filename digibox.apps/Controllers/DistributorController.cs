using AutoMapper;
using ClosedXML.Excel;
using digibox.apps.Models;
using digibox.apps.Modules;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Controllers
{
    public class DistributorController : Controller
    {
        private readonly IDistributorRepository _distributor;
        private readonly MapperConfiguration _mapperConfig;

        // GET: Distributor

        public DistributorController(IDistributorRepository distributor, MapperConfiguration mapperConfig)=>
            (_distributor, _mapperConfig) = (distributor, mapperConfig);

           
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OpenData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<tmdistributor, bool>> filter = f => (f.name.Contains(searchString));

            var mdl = _distributor.GetAll();
            mdl = mdl.Where(filter).OrderBy(f=>f.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            Mapper mapper = new Mapper(_mapperConfig);
            var model = mapper.Map<List<DistributorModel>>(mdl);

            ViewData["currentPage"] = page;
            return PartialView("_openData", model.ToPagedList(page ?? 1, myGlobal.PageSize));

        }

        public ActionResult Add()
        {
            return PartialView("_add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(DistributorModel distributor)
        {
            ResponseModel response = null;
            if (ModelState.IsValid)
            {
                try
                {
                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmdistributor>(distributor);

                    var rst = _distributor.Create(dta);

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
                var rst = _distributor.Delete(id);
                response = new ResponseModel()
                {
                    id = id,
                    Messages = "success",
                    isSuccess = true
                };
            }
            catch(Exception ex)
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
            var distributor = _distributor.FindById(id);
            var dta = _mapper.Map<DistributorModel>(distributor);
            return PartialView("_edit", dta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(DistributorModel distributor)
        {
            ResponseModel response = null;

            if (ModelState.IsValid)
            {
                try
                {
                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmdistributor>(distributor);
                    _distributor.Update(dta);
                    response = new ResponseModel()
                    {
                        id = distributor.id,
                        Messages = "success",
                        isSuccess = true
                    };
                }
                catch(Exception ex)
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
            var distributor = _distributor.GetAll();
            var result = (from a in distributor
                          select new SelectListItem()
                          {
                              Text = a.name,
                              Value = a.id.ToString()
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadTemplate()
        {
            var xlfile = Server.MapPath("~/Content/templates/distributor.xlsx");
            string contentType = MimeMapping.GetMimeMapping(xlfile);
            return File(xlfile, contentType);
        }


        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            ResponseModel response = null;
            try
            {
                string newfilename = Server.MapPath($"~/Content/temp/{myGlobal.currentUser.id}_upload_distributor.xlsx");
                if (System.IO.File.Exists(newfilename))
                {
                    System.IO.File.Delete(newfilename);
                }
                file.SaveAs(newfilename);
                //reading xl content, return json.
                List<DistributorModel> content = new List<DistributorModel>();
                using (var workbook = new XLWorkbook(newfilename))
                {
                    var worksheet = workbook.Worksheet("main");
                    int maxData = worksheet.RowCount();
                    for (int idx = 3; idx < maxData; idx++)
                    {
                        if (idx == 3)
                        {
                            //respon error di sini..
                            if (worksheet.Cell(idx, 1).Value.ToString() == "")
                            {
                                response = new ResponseModel()
                                {
                                    id = Guid.Empty,
                                    isSuccess = false,
                                    Messages = "Data kosong"
                                };
                                return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (worksheet.Cell(idx, 1).Value.ToString() == "")
                            {
                                break;
                            }
                        }
                        DistributorModel distributor = new DistributorModel()
                        {
                            name = worksheet.Cell(idx, 1).Value.ToString(),
                            address = worksheet.Cell(idx, 2).Value.ToString(),
                            telp =  worksheet.Cell(idx, 3).Value.ToString(),
                            id = Guid.NewGuid(),
                        };
                        content.Add(distributor);
                    }
                }
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = true,
                    Messages = "data uploaded"
                };

                Session["distributor"] = content;
                return PartialView("_displayUploadedData", content);
            }
            catch (Exception e)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = false,
                    Messages = e.Message
                };
                return Json(new { res = response }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveMultiple()
        {
            ResponseModel response = null;
            try
            {
                List<DistributorModel> distributor = Session["distributor"] as List<DistributorModel>;
                List<tmdistributor> dists = new List<tmdistributor>();
                foreach (var itm in distributor)
                {
                    tmdistributor dist = new tmdistributor()
                    {
                        name = itm.name,
                        id = Guid.NewGuid(),
                        address = itm.address,
                        telp = itm.telp,
                        createdby = myGlobal.currentUser.name
                    };
                    dists.Add(dist);
                }
                var hasil = _distributor.CrateMultiple(dists.ToArray());
                if (hasil == "")
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = "Distributor Added",
                        isSuccess = true
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = hasil,
                        isSuccess = false
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = ex.Message,
                    isSuccess = false
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

    }
}