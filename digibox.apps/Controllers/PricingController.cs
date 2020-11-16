using AutoMapper;
using digibox.apps.Models;
using digibox.apps.Modules;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.WebSockets;
using System.Windows.Media.Converters;
using WebGrease.Css.Extensions;

namespace digibox.apps.Controllers
{
    public class PricingController : Controller
    {
        private readonly IRoleService _role;
        private readonly IMaterialRepository _material;
        private readonly MapperConfiguration _mapperConfig;
        private readonly IDistributorRepository _distributor;
        private readonly IMaterialPriceRepository _materialPrice;
        private readonly IPricingService _pricingService;
        private readonly IMessageRepository _message;
        private readonly IPriceRepository _price;
        private readonly IPriceDetailRepository _priceDetail;
        private readonly IAttachmentRepository _attachment;

        // GET: Pricing

        public PricingController(IMaterialRepository material, IMaterialPriceRepository materialPrice, IPricingService pricingService, IPriceRepository price, IPriceDetailRepository priceDetail, IAttachmentRepository attachment, IMessageRepository message, IDistributorRepository distributor, IRoleService role, MapperConfiguration mapperConfig) => 
            (_material, _materialPrice, _pricingService, _price, _priceDetail, _attachment, _message, _distributor, _role, _mapperConfig) = (material, materialPrice, pricingService, price, priceDetail, attachment, message, distributor, role, mapperConfig);
        public ActionResult Index()
        {
            return CollectorPrice();
        }

        public ActionResult Detail(Guid id)
        {
            return SetPrice(id);
        }

        #region Collector Price List
        private ActionResult CollectorPrice()
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
            return View("CollectorPrice");
        }

        public ActionResult MaterialByCollector(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)       
        {

            if (page < 1)
                page = 1;
            Expression<Func<MaterialPricingModel, bool>> filter = f => (f.name.Contains(searchString));

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

            var user = myGlobal.currentUser;
            //check apakah role kolektor atau bukan
            var role = _role.getRoleByToken(myGlobal.usertoken);
            IQueryable<MaterialPricingModel> mdl;
            if (role == userRole.COLLECTOR)
            {
                mdl = _pricingService.GetPriceByCollector(user.id);
            }
            else
            {
                mdl = _pricingService.GetPrices();
            }
            mdl = mdl.Where(filter).OrderBy(x => x.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            ViewData["currentPage"] = page;
            return PartialView("_materialByCollector", mdl.ToPagedList(page ?? 1, myGlobal.PageSize));
        }
        #endregion

        #region Admin Price List
        private ActionResult AdminPrice()
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
            return View("AdminPrice");
        }

        public ActionResult MaterialPriceList(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<MaterialPricingModel, bool>> filter = f => (f.name.Contains(searchString));

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

            var user = myGlobal.currentUser;
            var mdl = _pricingService.GetPrices();
            mdl = mdl.Where(filter).OrderBy(x => x.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            //get current price list
            var model = mdl.ToPagedList(page ?? 1, myGlobal.PageSize).ToList();

            //get proposed price list
            var proposed = _pricingService.GetProposedPrices(new string[]{PriceStatus.DRAFT, PriceStatus.POSTEDBYCOLLECTOR, PriceStatus.POSTEDBYADMIN }, myGlobal.currentUser.id);
            List<MaterialPricingModel> dta;
            if (proposed.Count() != 0)
            {
                dta = (from m in model
                           join p in proposed on m.id equals p.id into prop
                           from props in prop.DefaultIfEmpty()
                           select new MaterialPricingModel()
                           {
                               currentprice = m.currentprice,
                               distributor = m.distributor,
                               id = m.id,
                               name = m.name,
                               partno = m.partno,
                               proposedprice = props.proposedprice ?? 0,
                               proposedtime = props.proposedtime,
                               status = props.status,
                               myproposed = m.myproposed
                           }).ToList();

            }
            else
            {
                dta = model.ToList();
            }
            //join current and propose price

            ViewData["currentPage"] = page;
            return PartialView("_materialPricesList", dta);
        }
        #endregion

        #region Manager Price List

        #region Admin Price List
        private ActionResult ManagerPrice()
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
            return View("ManagerPrice");
        }

        public ActionResult ManagerPriceList(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            if (page < 1)
                page = 1;
            Expression<Func<MaterialPricingModel, bool>> filter = f => (f.name.Contains(searchString));

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

            var user = myGlobal.currentUser;
            var mdl = _pricingService.GetPrices();
            mdl = mdl.Where(filter).OrderBy(x => x.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            //get current price list
            var model = mdl.ToPagedList(page ?? 1, myGlobal.PageSize).ToList();

            //get proposed price list
            var proposed = _pricingService.GetProposedPrices(new string[] { PriceStatus.DRAFT, PriceStatus.POSTEDBYCOLLECTOR, PriceStatus.POSTEDBYADMIN }, myGlobal.currentUser.id);
            List<MaterialPricingModel> dta;
            if (proposed.Count() != 0)
            {
                dta = (from m in model
                       join p in proposed on m.id equals p.id into prop
                       from props in prop.DefaultIfEmpty()
                       select new MaterialPricingModel()
                       {
                           currentprice = m.currentprice,
                           distributor = m.distributor,
                           id = m.id,
                           name = m.name,
                           partno = m.partno,
                           proposedprice = props.proposedprice ?? 0,
                           proposedtime = props.proposedtime,
                           status = props.status,
                           myproposed = m.myproposed
                       }).ToList();

            }
            else
            {
                dta = model.ToList();
            }
            //join current and propose price

            ViewData["currentPage"] = page;
            return PartialView("_managerPriceList", dta);
        }
        #endregion


        #endregion

        #region Price Creation
        /// <summary>
        /// Opening All list price for current selected material
        /// </summary>
        /// <param name="id">Material ID</param>
        /// <returns></returns>
        private ActionResult SetPrice(Guid id)
        {
           /* var user = myGlobal.currentUser;
            var mdl = _pricingService.GetPriceByCollector(user.id).Where(x=>x.id==id);
            if (mdl.Count() == 0)
            {
                return RedirectToAction("Index");
            }*/

            var mat = _material.FindById(id);
            var map = new Mapper(_mapperConfig);
            var material = map.Map<MaterialModel>(mat);
            ViewData["material"] = material;

            var distributor = _distributor.FindById(mat.distributor).name;
            ViewData["distributor"] = distributor;
            ViewData["materialid"] = id;

            return View("SetPrice");
        }


        public ActionResult MaterialPrice(Guid id)
        {
            var model = _pricingService.GetMaterialPrices(id).ToList();
            return PartialView("_materialPrice", model);
        }

        #endregion

        #region Admin Price Approval

        /// <summary>
        /// Approval Price Page
        /// </summary>
        /// <param name="id">Material ID</param>
        /// <returns></returns>
        public ActionResult ApprovalPrice(Guid id)
        {
            var user = myGlobal.currentUser;

            var mat = _material.FindById(id);
            var map = new Mapper(_mapperConfig);
            var material = map.Map<MaterialModel>(mat);
            ViewData["material"] = material;

            var distributor = _distributor.FindById(mat.distributor).name;
            ViewData["distributor"] = distributor;
            ViewData["materialid"] = id;

            return View("ApprovalPrice");
        }

        public ActionResult ApprovalMaterialPrice(Guid id)
        {
            var model = _pricingService.GetMaterialPrices(id).ToList();
            return PartialView("_approvalMaterialPrice", model);
        }

        #endregion

        #region Manager Price Approval 

        /// <summary>
        /// Approval Price Page
        /// </summary>
        /// <param name="id">Material ID</param>
        /// <returns></returns>
        public ActionResult ApprovalPriceByManager(Guid id)
        {
            var user = myGlobal.currentUser;

            var mat = _material.FindById(id);
            var map = new Mapper(_mapperConfig);
            var material = map.Map<MaterialModel>(mat);
            ViewData["material"] = material;

            var distributor = _distributor.FindById(mat.distributor).name;
            ViewData["distributor"] = distributor;
            ViewData["materialid"] = id;

            return View("ApprovalPriceByManager");
        }

        public ActionResult ApprovalMaterialPriceByManager(Guid id)
        {
            var model = _pricingService.GetMaterialPrices(id).ToList();
            return PartialView("_approvalMaterialPriceByManager", model);
        }


        #endregion

        #region Approval Function

        /// <summary>
        /// Price Approval
        /// </summary>
        /// <param name="id">Material User ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApproveX(Guid id)
        {
            var resp = new ResponseModel();
            try
            {
                var dta = _materialPrice.FindById(id);
                dta.approvedby = myGlobal.currentUser.id;
                _pricingService.ApprovePrice(dta, PriceStatus.APPROVED);
                resp = new ResponseModel()
                {
                    id = id,
                    Messages = "Price is Approved",
                    isSuccess = true
                };
            }
            catch (Exception ex)
            {
                resp = new ResponseModel()
                {
                    id = id,
                    Messages = "Price Approval Failed" + ex.Message,
                    isSuccess = false
                };
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectX(Guid id)
        {
            var resp = new ResponseModel();
            try
            {
                var dta = _materialPrice.FindById(id);
                dta.approvedby = myGlobal.currentUser.id;
                _pricingService.RejectPrice(dta, PriceStatus.REJECTED);
                resp = new ResponseModel()
                {
                    id = id,
                    Messages = "Price is Rejected",
                    isSuccess = true
                };
            }
            catch (Exception ex)
            {
                resp = new ResponseModel()
                {
                    id = id,
                    Messages = "Price Rejection Failed",
                    isSuccess = false
                };
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Addx(Guid id)
        {
            var mat = _material.FindById(id);
            var map = new Mapper(_mapperConfig);
            var material = map.Map<MaterialModel>(mat);
            ViewData["material"] = material;

            MaterialPriceListModel model = new MaterialPriceListModel()
            {
                materialid = id,
                datestart = DateTime.Now
            };
            //informasi material
            return PartialView("_add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Addx(MaterialPriceListModel model)
        {
            ResponseModel response = null;
            if (ModelState.IsValid)
            {
                try
                {
                    //check whether this item has draft
                    var dtax = _materialPrice.GetAll().Where(x => x.materialid == model.materialid && (x.status == PriceStatus.DRAFT || x.status == PriceStatus.POSTEDBYCOLLECTOR || x.status == PriceStatus.POSTEDBYADMIN)).ToList();
                    if (dtax.Count != 0)
                    {
                        response = new ResponseModel()
                        {
                            id = Guid.Empty,
                            Messages = "New material price is waiting for approval",
                            isSuccess = false
                        };
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }


                    IMapper _mapper = new Mapper(_mapperConfig);
                    var dta = _mapper.Map<tmmaterialprice>(model);
                    dta.createdby = myGlobal.currentUser.name;
                    dta.isactive = false;
                    dta.status = PriceStatus.DRAFT;
                    dta.createdbyid = myGlobal.currentUser.id;
                    var rst = _materialPrice.Create(dta);

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
                var rst = _price.Delete(id);
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
 
    
        #region Pricing Changes

        public ActionResult PriceProposalChanges()
        {
            var currentrole = _role.getRoleByToken(myGlobal.usertoken);
            if (currentrole == userRole.COLLECTOR)
            {
                return PriceCollectorProposalChanges();
            }
            if ((currentrole == userRole.MANAGER)||(currentrole == userRole.ADMIN)||(currentrole == userRole.SUPERADMIN))
            {
                return PriceManagerProposalChanges();
            }
            return View();
        }

        private ActionResult PriceManagerProposalChanges()
        {
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Distributor",
                Value = "distributor"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Tanggal",
                Value = "date"
            });
            ViewData["parameters"] = parameters;
            return View("PriceManagerProposalChanges");
        }

        public ActionResult PriceManagerProposalList(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<PriceProposalListModel, bool>> filter = f => (f.distributor.Contains(searchString));

            if ((paramType == "date"))
             {

                 filter = f => (f.ddate.ToString().Contains(searchString));
             };


             if (paramType == "distributor")
             {
                 filter = f => (f.distributor.Contains(searchString));
             }

            var user = myGlobal.currentUser;
            var status = new[] { PriceStatus.POSTEDBYCOLLECTOR, PriceStatus.REJECTED, PriceStatus.APPROVED};
            var mdl = _pricingService.GetProposalPrice(status, user.id);
            mdl = mdl.Where(filter).OrderByDescending(x => x.ddate);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            ViewData["currentPage"] = page;
            return PartialView("_priceManagerProposalList", mdl.ToPagedList(page ?? 1, myGlobal.PageSize));
        }


        /// <summary>
        /// Collector Price Changes List
        /// </summary>
        /// <returns></returns>
        private ActionResult PriceCollectorProposalChanges()
        {
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Distributor",
                Value = "distributor"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Tanggal",
                Value = "date"
            });
            ViewData["parameters"] = parameters;
            return View("PriceCollectorProposalChanges");
        }

        public ActionResult PriceCollectorProposalList(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<PriceProposalListModel, bool>> filter = f => (f.distributor.Contains(searchString));

            if ((paramType == "date"))
            {

                filter = f => (f.ddate.ToString().Contains(searchString));
            };


            if (paramType == "distributor")
            {
                filter = f => (f.distributor.Contains(searchString));
            }

            var user = myGlobal.currentUser;
            var status = new[] { PriceStatus.DRAFT, PriceStatus.POSTEDBYCOLLECTOR, PriceStatus.REJECTED, PriceStatus.APPROVED};
            var mdl = _pricingService.GetProposalPrice(status,user.id);
            mdl = mdl.Where(filter).OrderByDescending(x => x.ddate);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            ViewData["currentPage"] = page;
            return PartialView("_priceCollectorProposalList", mdl.ToPagedList(page ?? 1, myGlobal.PageSize));
        }


        public ActionResult Add()
        {
            var distributor = _distributor.GetDistributorByCollector(myGlobal.currentUser.id);
            var distributorlist = (from u in distributor
                                   select new SelectListItem()
                                   {
                                       Text = u.name,
                                       Value = u.id.ToString()
                                   }).ToList();
            ViewData["distributorlist"] = distributorlist;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(PriceProposalModel model, HttpPostedFileBase file, Guid[] materials, decimal[] prices)
        {
            if (ModelState.IsValid)
            {
                Mapper map = new Mapper(_mapperConfig);
                var dta = map.Map<ttprice>(model);
                dta.collectorid = myGlobal.currentUser.id;
                dta.createdby = myGlobal.currentUser.name;
                dta.status = PriceStatus.DRAFT;
                dta.ddate = DateTime.Now;
                //add mparent data
                //insert detail data
                var price = _price.Create(dta);
                //attachment
                if (file != null)
                {
                    _attachment.DeleteByReferenceId(model.id);
                    //update attachment;
                    var attachment = new AttachmentModel()
                    {
                        attachment = myGlobal.convertFile(file.InputStream),
                        attachmenttype = AttachmentType.PRICEPROPOSAL,
                        referenceid = price.id,
                        filename = file.FileName,
                    };
                    var attc = map.Map<ttattachment>(attachment);
                    _attachment.Create(attc);
                }

                //crate multiple detail.
                List<PriceProposalDetailModel> priceDetail = new List<PriceProposalDetailModel>();
                int idx = 0;
                foreach(var itm in materials)
                {
                    priceDetail.Add(new PriceProposalDetailModel()
                    {
                        materialid = itm,
                        priceid = dta.id,
                        newprice = prices[idx]
                    });
                    idx++;
                }
                _pricingService.CreateMultipleDetail(priceDetail);
                return RedirectToAction("PriceProposalChanges");
            }
            else
            {
                var distributor = _distributor.GetDistributorByCollector(myGlobal.currentUser.id);
                var distributorlist = (from u in distributor
                                       select new SelectListItem()
                                       {
                                           Text = u.name,
                                           Value = u.id.ToString()
                                       }).ToList();
                ViewData["distributorlist"] = distributorlist;
                return View();
            }
        }

        public ActionResult Edit(Guid id)
        {
            var dta = _price.FindById(id);
            Mapper map = new Mapper(_mapperConfig);
            var distributor = _distributor.FindById(dta.distributorid??Guid.NewGuid()).name;
            ViewData["distributor"] = distributor;
            var model = map.Map<PriceProposalModel>(dta);
            //get prices..

            var attachment = _attachment.getByReferenceID(id, AttachmentType.PRICEPROPOSAL).FirstOrDefault();
            if (attachment != null)
            {
                ViewData["attachment"] = attachment.id.ToString();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PriceProposalModel model, Guid[] materials, HttpPostedFileBase file, decimal?[] currentprice, decimal?[] prices)
        {
            Mapper map = new Mapper(_mapperConfig);
            var dta = map.Map<ttprice>(model);
            dta.updatedby = myGlobal.currentUser.name;
            dta.ddate = DateTime.Now;
            _price.Update(dta);

            //update detail.
            //cari yang lama.. 
            var oDetail = _priceDetail.GetByPriceId(model.id);
            var oldDetail = map.Map<List<PriceProposalDetailModel>>(oDetail);
            //bandingkan yang lama dengan yang baru.
            var newDetail = new List<PriceProposalDetailModel>();
            int idx = 0;

            if (file != null)
            {
                _attachment.DeleteByReferenceId(model.id);
                //update attachment;
                var attachment = new AttachmentModel()
                {
                    attachment = myGlobal.convertFile(file.InputStream),
                    attachmenttype = AttachmentType.PRICEPROPOSAL,
                    referenceid = model.id,
                    filename = file.FileName,
                };
                var attc = map.Map<ttattachment>(attachment);
                _attachment.Create(attc);
            }

            foreach(var itm in materials)
            {
                //iscp = currentprice[idx].HasValue;
                newDetail.Add(new PriceProposalDetailModel()
                {
                    priceid= model.id,
                    materialid = itm,
                    newprice = prices[idx].GetValueOrDefault(0)/*,
                    currentprice = currentprice[idx].GetValueOrDefault(0)*/
                }) ;
                idx++;
            }

            _pricingService.UpdateMultipleDetail(oldDetail, newDetail);

            return RedirectToAction("PriceProposalChanges");
        }

        public ActionResult AddMaterialByDistributor(Guid id)
        {
            return PartialView("_addMaterialByDistributor");
        }

        public JsonResult MaterialPriceByDistributor(Guid id)
        {
            var pricelist = _pricingService.GetMaterialPriceByDistributor(id).ToList();
            var detail = (from p in pricelist 
                          select new MaterialPricingModel()
                          {
                              currentprice = p.currentprice == null ? 0 : p.currentprice,
                              id = p.id,
                              partno = p.partno,
                              name = p.name,
                              proposedprice = p.proposedprice
                          }).ToList();
            return Json(pricelist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// List of Selected Material based on proposal id
        /// </summary>
        /// <param name="priceid">proposal id get from table price</param>
        /// <returns></returns>
        public JsonResult SelectedMaterial(Guid priceid)
        {
            var dtaDetailModel = _priceDetail.GetByPriceId(priceid).ToList();
            var price = _price.FindById(priceid);
            //distributor pricelist
            var pricelist = _pricingService.GetMaterialPriceByDistributor(price.distributorid??Guid.NewGuid()).ToList();

            var detail = (from d in dtaDetailModel
                          join p in pricelist on d.materialid equals p.id
                          select new MaterialPricingModel()
                          {
                              currentprice = p.currentprice==null?0: p.currentprice,
                              id = p.id,
                              partno = p.partno,
                              name = p.name,
                              proposedprice = d.newprice
                          }).ToList();


            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        #endregion Pricing Changes


        #region Approval Pricing
        [HttpPost]
        public JsonResult Propose(Guid id)
        {
            ResponseModel response = null;
            try
            {
                //get attachment
                var attachment = _attachment.getByReferenceID(id, AttachmentType.PRICEPROPOSAL);
                if (attachment.Count() == 0)
                {
                    response = new ResponseModel()
                    {
                        id = id,
                        Messages = "Need Attachment",
                        isSuccess = false
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var price = _price.FindById(id);
                var priceitems = _priceDetail.GetByPriceId(id).ToList() ;
                var material = _material.GetAll();
                var items = (from p in priceitems
                             join m in material on p.materialid equals m.id
                             select m.name).ToArray();

                var messageitems = string.Join(", ", items);
                string messages = $"Price Change on {messageitems} is Posted. Please verify and approved!";

                //get item
                //check role
                var role = _role.getRoleByToken(myGlobal.usertoken);
                var result = _price.ProposePrice(id,PriceStatus.POSTEDBYCOLLECTOR);
                if (result)
                {
                    //sending notification
                    var usr = myGlobal.currentUser;
                    var adminrole = _role.getRoleByName(userRole.ADMIN).First();
                    _message.SendMessageToRole(usr.id, adminrole.id, messages);

                    var managerrole = _role.getRoleByName(userRole.MANAGER).First();
                    _message.SendMessageToRole(usr.id, managerrole.id, messages);

                    var superadminrole = _role.getRoleByName(userRole.SUPERADMIN).First();
                    _message.SendMessageToRole(usr.id, superadminrole.id, messages);

                    response = new ResponseModel()
                    {
                        id = id,
                        Messages = "success",
                        isSuccess = true
                    };
                }
                else
                {
                    response = new ResponseModel()
                    {
                        id = id,
                        Messages = "propose is failed. some data is invalid.",
                        isSuccess = false
                    };
                }
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

        #endregion

        public ActionResult OpenProposal(Guid id)
        {
            var currentrole = _role.getRoleByToken(myGlobal.usertoken);
            if (currentrole == userRole.COLLECTOR)
            {
                return OpenProposalCollector(id);
            }
            else
            {
                return openProposalPrice(id);
            }
        }

        private ActionResult openProposalPrice(Guid id)
        {
            //price
            var price = _price.FindById(id);
            var map = new Mapper(_mapperConfig);
            var model = map.Map<PriceProposalModel>(price);
            //attachment
            var attachment = _attachment.getByReferenceID(id, AttachmentType.PRICEAPPROVAL).FirstOrDefault();
            if (attachment != null)
            {
                ViewData["attachment"] = attachment.id.ToString();
            }

            //distributor 
            var distributor = _distributor.FindById(model.distributorid ?? Guid.NewGuid()).name;
            ViewData["distributor"] = distributor;

            var detail = _priceDetail.GetByPriceId(id).ToList(); //price on detail proposal

            var pricelist = _pricingService.GetMaterialPriceByDistributor(model.distributorid??Guid.NewGuid()).ToList();
            var details = (from p in pricelist
                           join d in detail on p.id equals d.materialid
                           select new MaterialPricingModel()
                           {
                               currentprice = p.currentprice == null ? 0 : p.currentprice,
                               id = p.id,
                               partno = p.partno,
                               name = p.name,
                               proposedprice = d.newprice
                           }).ToList();
            ViewData["details"] = details;

            //attachment 

            return View("openProposalPrice", model);
        }

        private ActionResult OpenProposalCollector(Guid id)
        {
            //price
            var price = _price.FindById(id);
            var map = new Mapper(_mapperConfig);
            var model = map.Map<PriceProposalModel>(price);
            //attachment
            var attachment = _attachment.getByReferenceID(id, AttachmentType.PRICEPROPOSAL);
            ViewData["attachment"] = attachment;

            //distributor 
            var distributor = _distributor.FindById(model.distributorid??Guid.NewGuid()).name;
            ViewData["distributor"] = distributor;

            var detail = _priceDetail.GetByPriceId(id).ToList(); //price on detail proposal

            var pricelist = _pricingService.GetMaterialPriceByDistributor(id).ToList();
            var details = (from p in pricelist
                           join d in detail on p.id equals d.materialid
                          select new MaterialPricingModel()
                          {
                              currentprice = p.currentprice == null ? 0 : p.currentprice,
                              id = p.id,
                              partno = p.partno,
                              name = p.name,
                              proposedprice = p.proposedprice
                          }).ToList();
            ViewData["details"] = details;

            return View("openProposalCollector", model);
        }

        [HttpPost]
        public JsonResult Approve(Guid id, HttpPostedFileBase file)
        {
            var res = new ResponseModel();
            try
            {
                var map = new Mapper(_mapperConfig);

               if (file != null)
                {
                    //remove file lama.
                    _attachment.DeleteByReferenceId(id);
                    //update attachment;
                    var attachment = new AttachmentModel()
                    {
                        attachment = myGlobal.convertFile(file.InputStream),
                        attachmenttype = AttachmentType.PRICEAPPROVAL,
                        referenceid = id,
                        filename = file.FileName,
                    };
                    var attc = map.Map<ttattachment>(attachment);
                    _attachment.Create(attc);


                    var price = _price.FindById(id);
                    var priceitems = _priceDetail.GetByPriceId(id);
                    var material = _material.GetAll();

                    var items = (from p in priceitems
                                 join m in material on p.materialid equals m.id
                                 select m.name).ToArray();

                    var messageitems = string.Join(", ", items);
                    string messages = $"Price {messageitems} is Approved!";


                    //sending notification to collector
                    var usr = myGlobal.currentUser;
                    _message.SendMessageToUser(usr.id, price.collectorid, messages);

                    var adminrole = _role.getRoleByName(userRole.ADMIN).First();
                    _message.SendMessageToRole(usr.id, adminrole.id, messages);

                    var superadminrole = _role.getRoleByName(userRole.SUPERADMIN).First();
                    _message.SendMessageToRole(usr.id, superadminrole.id, messages);


                    //change to approval.
                    _price.ApprovePrice(id,myGlobal.currentUser.id,PriceStatus.APPROVED);
                    res = new ResponseModel()
                    {
                        id = id,
                        isSuccess = true,
                        Messages = "Record is Approved"
                    };
                    return Json(res, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    res = new ResponseModel()
                    {
                        id = id,
                        isSuccess = false,
                        Messages = "Please set attached file"
                    };
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception e)
            {
               
                var resp = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = e.Message,
                    isSuccess = false
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Reject(Guid id)
        {
            ViewData["id"] = id.ToString();
            return PartialView("_reject");
        }

        [HttpPost]
        public ActionResult Reject(Guid id, string reason)
        {
            ResponseModel res = new ResponseModel();
            if (reason.Trim().Length == 0)
            {
                res = new ResponseModel()
                {
                    id = id,
                    isSuccess = false,
                    Messages = "Need your reason"
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var dta = _price.RejectPrice(id,reason,myGlobal.currentUser.id,PriceStatus.REJECTED);
                if (dta)
                {
                    res = new ResponseModel()
                    {
                        id = id,
                        isSuccess = true,
                        Messages = "Record is Rejected"
                    };
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    res = new ResponseModel()
                    {
                        id = id,
                        isSuccess = false,
                        Messages = "Record is not Rejected"
                    };
                    return Json(res, JsonRequestBehavior.AllowGet);
                }

            }
            catch(Exception ex)
            {
                res = new ResponseModel()
                {
                    id = Guid.NewGuid(),
                    isSuccess = false,
                    Messages = ex.Message
                };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

    }

}