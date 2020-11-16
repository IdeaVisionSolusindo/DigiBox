using AutoMapper;
using ClosedXML;
using ClosedXML.Excel;
using digibox.apps.Models;
using digibox.apps.Modules;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace digibox.apps.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IMaterialRepository _material;
        private readonly MapperConfiguration _mapperConfig;
        private readonly IAttributeRepository _attribute;
        private readonly IDistributorRepository _distributor;
        private readonly IMaterialSBURepository _materialSBU;
        private readonly IMaterialServices _materialService;


        // GET: Material

        public MaterialController(IMaterialRepository material, IMaterialSBURepository materialSBU, IMaterialServices materialService, IAttributeRepository attribute, IDistributorRepository distributor, MapperConfiguration mapperConfig) => 
            (_material, _materialSBU, _materialService, _attribute, _distributor, _mapperConfig) = (material, materialSBU, materialService, attribute, distributor, mapperConfig);
        public ActionResult Index()
        {
            //searching parameter type;
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
                Text = "Movement Type",
                Value = "movementtype"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "SBU",
                Value = "sbu"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Material Type",
                Value = "materialtype"
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
                filter = f => (f.distributor== searchString);
            }

            var mdl = _material.GetMaterial();
            mdl = mdl.Where(filter).OrderBy(x => x.name); 
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            //var model = mdl.OrderBy(x => x.name);
            var model = mdl.ToPagedList(page ?? 1, myGlobal.PageSize);
            var materialsbu = _materialSBU.GetTransposeAllMaterialSBU();
            var rst = (from m in model
                       join msbu in materialsbu on m.id equals msbu.materialid
                       select new MaterialListModel()
                       {
                           id = m.id,
                           movementtype = m.movementtype,
                           maxstock = m.maxstock,
                           minstock = m.minstock,
                           name = m.name,
                           partno = m.partno,
                           datecreate = m.datecreate,
                           description = m.description,
                           materialtype = m.materialtype,
                           distributor = m.distributor,
                           location = m.location,
                           binlocation = m.binlocation,
                           unit = m.unit,
                           sbu = msbu.sbu,
                           plant = m.plant,
                           sloc = m.sloc,
                           calhorizon = m.calhorizon
                       }).ToList();            

            ViewData["currentPage"] = page;
            return PartialView("_openData", rst);

        }

        public ActionResult Add()
        {

            var uom = _attribute.GetByName("UOM");

            var uomlist = (from u in uom
                           select new SelectListItem()
                           {
                               Text = u.attributevalue,
                               Value = u.attributevalue
                           }).ToList();
            ViewData["uomlist"] = uomlist;

            var location = _attribute.GetByName("LOCATION");
            var locationlist = (from u in location
                                select new SelectListItem()
                           {
                               Text = u.attributevalue,
                               Value = u.id.ToString()
                           }).ToList();

            ViewData["locationlist"] = locationlist;

            var binlocation = _attribute.GetByName("BIN-LOCATION");
            var binlocationlist = (from u in binlocation
                                   select new SelectListItem()
                                {
                                    Text = u.attributevalue,
                                    Value = u.id.ToString()
                                }).ToList();

            ViewData["binlocationlist"] = binlocationlist;

            var movementtype = _attribute.GetByName("MOVEMENT-TYPE");
            var movementtypelist = (from u in movementtype
                                    select new SelectListItem()
                                   {
                                       Text = u.attributevalue,
                                       Value = u.id.ToString()
                                   }).ToList();

            ViewData["movementtypelist"] = movementtypelist;

            var materialtype = _attribute.GetByName("MATERIAL-TYPE");
            var materialtypelist = (from u in materialtype
                                    select new SelectListItem()
                                    {
                                        Text = u.attributevalue,
                                        Value = u.id.ToString()
                                    }).ToList();

            ViewData["materialtypelist"] = materialtypelist;

            var sbu = _attribute.GetByName("SBU");
            var sbulist = (from u in sbu
                           select new SelectListItem()
                                    {
                                        Text = u.attributevalue,
                                        Value = u.id.ToString()
                                    }).ToList();

            ViewData["sbulist"] = sbulist;

            var distributor = _distributor.GetAll();
            var distributorlist = (from u in distributor
                                   select new SelectListItem()
                                   {
                                       Text = u.name,
                                       Value = u.id.ToString()
                                   }).ToList();

            ViewData["distributorlist"] = distributorlist;

            return View("Add");
        }

        [HttpPost]
        public ActionResult Add(MaterialModel material)
        {
            if (ModelState.IsValid)
            {
                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmmaterial>(material);

                    var rest = _material.Create(dta);
                    _materialSBU.CreateMultiple(rest.id, material.sbu);
                    return RedirectToAction("Index");
            }
            else
            {
                return View(material);
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {

            ResponseModel response = null;
            try
            {
                var rst = _material.Delete(id);
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
            var material = _material.FindById(id);
            var dta = _mapper.Map<MaterialModel>(material);
            var uom = _attribute.GetByName("UOM");

            var uomlist = (from u in uom
                           select new SelectListItem()
                           {
                               Text = u.attributevalue,
                               Value = u.attributevalue,
                               Selected = u.attributevalue==material.unit
                           }).ToList();
            ViewData["uomlist"] = uomlist;

            var location = _attribute.GetByName("LOCATION");
            var locationlist = (from u in location
                                select new SelectListItem()
                                {
                                    Text = u.attributevalue,
                                    Value = u.id.ToString(),
                                    Selected = u.id == material.location
                                }).ToList();

            ViewData["locationlist"] = locationlist;

            var binlocation = _attribute.GetByName("BIN-LOCATION");
            var binlocationlist = (from u in binlocation
                                   select new SelectListItem()
                                   {
                                       Text = u.attributevalue,
                                       Value = u.id.ToString(),
                                       Selected = u.id == material.binlocation
                                   }).ToList();

            ViewData["binlocationlist"] = binlocationlist;

            var movementtype = _attribute.GetByName("MOVEMENT-TYPE");
            var movementtypelist = (from u in movementtype
                                    select new SelectListItem()
                                    {
                                        Text = u.attributevalue,
                                        Value = u.id.ToString(),
                                        Selected = u.id == material.movementtype
                                    }).ToList();

            ViewData["movementtypelist"] = movementtypelist;

            var materialtype = _attribute.GetByName("MATERIAL-TYPE");
            var materialtypelist = (from u in materialtype
                                    select new SelectListItem()
                                    {
                                        Text = u.attributevalue,
                                        Value = u.id.ToString(),
                                        Selected = u.id == material.materialtype
                                    }).ToList();

            ViewData["materialtypelist"] = materialtypelist;

            var sbu = _attribute.GetByName("SBU");
            var sbulist = (from u in sbu
                           select new SelectListItem()
                           {
                               Text = u.attributevalue,
                               Value = u.id.ToString(),
                           }).ToList();

            ViewData["sbulist"] = sbulist;
            var distributor = _distributor.GetAll();
            var distributorlist = (from u in distributor
                                   select new SelectListItem()
                                   {
                                       Text = u.name,
                                       Value = u.id.ToString(),
                                       Selected = u.id == material.distributor
                                   }).ToList();

            ViewData["distributorlist"] = distributorlist;

            return View(dta);
        }


        [HttpGet]
        public JsonResult GetSBU(Guid id)
        {
            var materialsbus = _materialSBU.GetMaterialSBUByMaterialID(id);
            var asbu = (from m in materialsbus
                        select m.sbuid).ToList();
            return Json(asbu, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Edit(MaterialModel material)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmmaterial>(material);
                    _material.Update(dta);

                    //simpan data sbu.
                    //get prior sbu
                    var materialsbus = _materialSBU.GetMaterialSBUByMaterialID(material.id).ToList();

                    var existedmbu = (from m in materialsbus
                                      select m.sbuid).ToList();
                    //data yang ada dalam materialsbus tapi tidak ada dalam material, buang
                    var willbu = existedmbu.Except(material.sbu).ToList();
                    var deletedid = (from m in materialsbus
                                     join ex in willbu on m.sbuid equals ex
                                     select m.id).ToArray();

                    _materialSBU.DeleteMultiple(deletedid);
                    //add yang ga ada dalam original.
                    willbu = material.sbu.Except(existedmbu).ToList();
                    _materialSBU.CreateMultiple(material.id, willbu.ToArray());
                    return RedirectToAction("Index");
                }
                catch
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {

            var uom = _attribute.GetByName("UOM");
                var uomlist = (from u in uom
                               select new SelectListItem()
                               {
                                   Text = u.attributevalue,
                                   Value = u.attributevalue,
                                   Selected = u.attributevalue == material.unit
                               }).ToList();
                ViewData["uomlist"] = uomlist;

                var location = _attribute.GetByName("LOCATION");
                var locationlist = (from u in location
                                    select new SelectListItem()
                                    {
                                        Text = u.attributevalue,
                                        Value = u.id.ToString(),
                                        Selected = u.id == material.location
                                    }).ToList();

                ViewData["locationlist"] = locationlist;

                var binlocation = _attribute.GetByName("BIN-LOCATION");
                var binlocationlist = (from u in binlocation
                                       select new SelectListItem()
                                       {
                                           Text = u.attributevalue,
                                           Value = u.id.ToString(),
                                           Selected = u.id == material.binlocation
                                       }).ToList();

                ViewData["binlocationlist"] = binlocationlist;

                var movementtype = _attribute.GetByName("MOVEMENT-TYPE");
                var movementtypelist = (from u in movementtype
                                        select new SelectListItem()
                                        {
                                            Text = u.attributevalue,
                                            Value = u.id.ToString(),
                                            Selected = u.id == material.movementtype
                                        }).ToList();

                ViewData["movementtypelist"] = movementtypelist;

                var materialtype = _attribute.GetByName("MATERIAL-TYPE");
                var materialtypelist = (from u in materialtype
                                        select new SelectListItem()
                                        {
                                            Text = u.attributevalue,
                                            Value = u.id.ToString(),
                                            Selected = u.id == material.materialtype
                                        }).ToList();

                ViewData["materialtypelist"] = materialtypelist;

                var sbu = _attribute.GetByName("SBU");
                var sbulist = (from u in sbu
                               select new SelectListItem()
                               {
                                   Text = u.attributevalue,
                                   Value = u.id.ToString(),
                               }).ToList();

                ViewData["sbulist"] = sbulist;
                var distributor = _distributor.GetAll();
                var distributorlist = (from u in distributor
                                       select new SelectListItem()
                                       {
                                           Text = u.name,
                                           Value = u.id.ToString(),
                                           Selected = u.id == material.distributor
                                       }).ToList();

                ViewData["distributorlist"] = distributorlist;
                return View(material);
            }
        }

        //tampil semua material yang sudah diassign untuk kolektor dan yang free.
        public ActionResult AssignMaterial(Guid? id, string sortOrder = "ASC", string showparam="All", string paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<MaterialAssignmentModel, bool>> filter = f => (f.name.Contains(searchString));

            if ((paramType == "partno"))
            {
                filter = f => (f.partno.Contains(searchString));
            };

            if (paramType == "name")
            {
                filter = f => (f.name.Contains(searchString));
            }

            if (paramType == "distributor")
            {
                filter = f => (f.distributor.Contains(searchString));
            }
            var mdl = _materialService.MaterialAssignmentList(id ?? Guid.NewGuid());
            if (showparam == "Assigned")
            {
                mdl = mdl.Where(x => x.Assigned);
            }
            if(showparam == "UnAssigned")
            {
                mdl = mdl.Where(x => !x.Assigned);
            }
            mdl = mdl.Where(filter).OrderBy(x => x.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            
            return PartialView("_assignMaterial", mdl.ToPagedList(page ?? 1, myGlobal.PageSize));

        }


        public JsonResult GetAll()
        {
            var materiallist = _material.GetAll().ToList();
            var detail = (from p in materiallist
                          select new MaterialModel()
                          {
                              id = p.id,
                              partno = p.partno,
                              name = p.name,
                              unit = p.unit,
                          }).ToList();
            return Json(materiallist, JsonRequestBehavior.AllowGet);
        }
    
    
        public ActionResult DownloadTemplate()
        {
            string newfilename = Server.MapPath($"~/Content/temp/{myGlobal.currentUser.name}_materialtemplates.xlsx");
            if (System.IO.File.Exists(newfilename))
            {
                System.IO.File.Delete(newfilename);
            }
            var xlfile = Server.MapPath("~/Content/templates/materialtemplates.xlsx");
            string contentType = MimeMapping.GetMimeMapping(xlfile);
            var file = System.IO.File.Open(xlfile,FileMode.Open,FileAccess.ReadWrite,FileShare.Read);

            using (var workbook = new XLWorkbook(file))
            {
                var worksheet = workbook.Worksheet("location");
                int idx = 2;
                var locs = _attribute.GetByName("LOCATION").ToList();
                foreach (var itm in locs)
                {
                    worksheet.Cell(idx, 1).Value = itm.attributevalue;
                    idx++;
                }

                worksheet = workbook.Worksheet("binlocation");
                idx = 2;
                locs = _attribute.GetByName("BIN-LOCATION").ToList();
                foreach (var itm in locs)
                {
                    worksheet.Cell(idx, 1).Value = itm.attributevalue;
                    idx++;
                }

                worksheet = workbook.Worksheet("movementtype");
                idx = 2;
                locs = _attribute.GetByName("MOVEMENT-TYPE").ToList();
                foreach (var itm in locs)
                {
                    worksheet.Cell(idx, 1).Value = itm.attributevalue;
                    idx++;
                }

                worksheet = workbook.Worksheet("materialtype");
                idx = 2;
                locs = _attribute.GetByName("MATERIAL-TYPE").ToList();
                foreach (var itm in locs)
                {
                    worksheet.Cell(idx, 1).Value = itm.attributevalue;
                    idx++;
                }

                worksheet = workbook.Worksheet("SBU");
                idx = 2;
                locs = _attribute.GetByName("SBU").ToList();
                foreach (var itm in locs)
                {
                    worksheet.Cell(idx, 1).Value = itm.attributevalue;
                    idx++;
                }

                worksheet = workbook.Worksheet("Distributor");
                idx = 2;
                var dist = _distributor.GetAll().ToList();
                foreach (var itm in dist)
                {
                    worksheet.Cell(idx, 1).Value = itm.name;
                    idx++;
                }
                
                newfilename = Server.MapPath($"~/Content/temp/{myGlobal.currentUser.name}_materialtemplates.xlsx");
                workbook.SaveAs(newfilename);
                return File(newfilename, contentType);
            }
        }
 
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            ResponseModel response = null;
            int lineno = 3;
            try
            {
                string newfilename = Server.MapPath($"~/Content/temp/{myGlobal.currentUser.name}_upload_materialtemplates.xlsx");
                if (System.IO.File.Exists(newfilename))
                {
                    System.IO.File.Delete(newfilename);
                }
                file.SaveAs(newfilename);
                //reading xl content, return json.
                List<MaterialListModel> content = new List<MaterialListModel>();
                using (var workbook = new XLWorkbook(newfilename))
                {
                    var worksheet = workbook.Worksheet("main");
                    int maxData = worksheet.RowCount();
                    for (int idx = 4; idx < maxData; idx++)
                    {
                        if (idx > 1004)
                        {
                            response = new ResponseModel()
                            {
                                id = Guid.Empty,
                                isSuccess = false,
                                Messages = "Maximum Record is only 1000"
                            };
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }

                        lineno++;
                        if (idx == 4)
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
                                return Json(response, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (worksheet.Cell(idx, 1).Value.ToString() == "")
                            {
                                break;
                            }
                        }

                        //datettime converter
                        DateTime dcreated;
                        if(!DateTime.TryParse(worksheet.Cell(idx, 4).Value.ToString(),out dcreated))
                        {
                            dcreated = DateTime.Now;
                        }

                        decimal maxstock, minstock;
                        if (!decimal.TryParse(worksheet.Cell(idx, 9).Value.ToString(), out maxstock))
                        {
                            maxstock = 1;
                        }

                        if (!decimal.TryParse(worksheet.Cell(idx, 8).Value.ToString(), out minstock))
                        {
                            minstock = 1;
                        }


                        MaterialListModel material = new MaterialListModel()
                        {
                            partno = worksheet.Cell(idx, 1).Value.ToString(),
                            name = worksheet.Cell(idx, 2).Value.ToString(),
                            description = worksheet.Cell(idx, 3).Value.ToString(),
                            datecreate = dcreated,
                            unit = worksheet.Cell(idx, 5).Value.ToString(),
                            location = worksheet.Cell(idx, 6).Value.ToString(),
                            binlocation = worksheet.Cell(idx, 7).Value.ToString(),
                            minstock = minstock,
                            maxstock = maxstock,
                            calhorizon = worksheet.Cell(idx, 10).Value.ToString(),
                            plant = worksheet.Cell(idx, 11).Value.ToString(),
                            movementtype = worksheet.Cell(idx, 12).Value.ToString(),
                            materialtype = worksheet.Cell(idx, 13).Value.ToString(),
                            sbu = worksheet.Cell(idx, 14).Value.ToString(),
                            sloc = worksheet.Cell(idx, 15).Value.ToString(),
                            distributor = worksheet.Cell(idx, 16).Value.ToString(),
                            id = Guid.NewGuid(),
                            collectorid = Guid.NewGuid(),
                            currentstock = 0
                        };
                        content.Add(material);
                    }
                }
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = true,
                    Messages = "data uploaded"
                };

                Session["material"] = content;
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = false,
                    Messages = $"{e.Message} at line {lineno}"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DisplayUploadedData(int? page)
        {
            var content = Session["material"] as List<MaterialListModel>;
            int maxpage = Convert.ToInt16(Math.Ceiling(content.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            //var model = mdl.OrderBy(x => x.name);
            var model = content.ToPagedList(page ?? 1, myGlobal.PageSize);
            ViewData["currentPage"] = page;
            return PartialView("_displayUploadedData", model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveMultiple()
        {
            List<MaterialListModel> material = Session["material"] as List<MaterialListModel>;
            List<tmmaterial> materials = new List<tmmaterial>();
            List<tmmaterialsbu> sbus = new List<tmmaterialsbu>();
            int idx=0;
            foreach(var itm in material)
            {
                idx++;
                tmattribute binloc=null;
                if (itm.binlocation != "")
                {
                    binloc = _attribute.GetByName("BIN-LOCATION").Where(x => x.attributevalue == itm.binlocation).FirstOrDefault();
                }

                tmdistributor dist=null;
                if (itm.distributor != "")
                {
                    dist = _distributor.GetAll().Where(x => x.name == itm.distributor).FirstOrDefault();
                }

                tmattribute loc=null;
                if (itm.location != "")
                {
                    loc = _attribute.GetByName("LOCATION").Where(x => x.attributevalue == itm.location).FirstOrDefault();
                }

                tmattribute mattype = null;
                if (itm.materialtype != "")
                {
                    mattype = _attribute.GetByName("MATERIAL-TYPE").Where(x => x.attributevalue == itm.materialtype).FirstOrDefault();
                    if (mattype == null)
                        continue;
                }

                tmattribute movtype = null;
                if (itm.movementtype != "")
                {
                    movtype = _attribute.GetByName("MOVEMENT-TYPE").Where(x => x.attributevalue == itm.movementtype).FirstOrDefault();
                }

                tmattribute unit = null;
                if (itm.unit != "")
                {
                    unit = _attribute.GetByName("UOM").Where(x => x.attributevalue == itm.unit).FirstOrDefault();
                }

                tmattribute sbu = null;
                if (itm.sbu != "")
                {
                    sbu = _attribute.GetByName("SBU").Where(x => x.attributevalue == itm.sbu).FirstOrDefault();
                }

                tmmaterial mat = new tmmaterial()
                {
                    binlocation = binloc != null ? binloc.id : Guid.Empty,
                    calhorizon = itm.calhorizon,
                    id = Guid.NewGuid(),
                    createdat = DateTime.Now,
                    createdby = myGlobal.currentUser.name,
                    currentstock = 0,
                    datecreate = itm.datecreate,
                    description = itm.description,
                    distributor = dist != null ? dist.id : Guid.Empty,
                    location = loc != null ? loc.id : Guid.Empty,
                    name = itm.name,
                    materialtype = mattype != null ? mattype.id : Guid.Empty,
                    maxstock = itm.maxstock,
                    minstock = itm.minstock,
                    movementtype = movtype != null ? movtype.id : Guid.Empty,
                    partno = itm.partno,
                    plant = itm.plant,
                    sloc = itm.sloc,
                    unit = itm.unit,
                };

                if (sbu != null)
                {
                    tmmaterialsbu sbud = new tmmaterialsbu()
                    {
                        createdat = DateTime.Now,
                        createdby = myGlobal.currentUser.name,
                        materialid = mat.id,
                        id = Guid.NewGuid(),
                        currentstock = 0,
                        isdeleted = false,
                        sbuid = sbu.id
                    };
                    materials.Add(mat);
                    sbus.Add(sbud);
                }
            }

            await _material.CrateMultiple(materials.ToArray(), sbus.ToArray());

            ResponseModel response = new ResponseModel()
            {
                id = Guid.Empty,
                isSuccess = true,
                Messages = "data uploaded"
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}