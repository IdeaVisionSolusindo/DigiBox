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
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Ast.Selectors;

namespace digibox.apps.Controllers
{
    public class ReplenishController : Controller
    {
        private readonly IRoleService _role;
        private readonly MapperConfiguration _mapperConfig;
        private readonly IReplenishRepository _replenish;
        private readonly IUserRepository _user;
        private readonly IDistributorRepository _distributor;
        private readonly IMaterialRepository _material;
        private readonly IReplenishDetailRepository _replenishDetail;
        private readonly IMaterialServices _materialServices;
        private readonly IMessageRepository _message;

        public ReplenishController(IReplenishRepository replenish, IReplenishDetailRepository replenishDetail, IDistributorRepository distributor, IMaterialRepository material, IMaterialServices materialServices, IMessageRepository message, IUserRepository user, IRoleService role, MapperConfiguration mapperConfig) =>
            (_replenish, _replenishDetail, _distributor, _material, _materialServices, _message, _user, _role, _mapperConfig) = (replenish, replenishDetail, distributor, material, materialServices, message, user, role, mapperConfig);
        // GET: Replenish
        public ActionResult Index()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            if (role == userRole.ADMIN)
            {
                return AdminIndex();
            }
            if (role == userRole.COLLECTOR)
            {
                return IndexCollector();
            }

            return View();
        }

        private ActionResult IndexCollector()
        {
            return View("IndexCollector");
        }

        public ActionResult ReplenishByCollector(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<ttreplenish, bool>> filter = f => (f.no.Contains(searchString));

            if ((paramType == "no"))
            {
                filter = f => (f.no.Contains(searchString));
            };

            if (paramType == "tanggal")
            {
                filter = f => (f.indate.ToString().Contains(searchString));
            }

            var user = myGlobal.currentUser;
            var role = _role.getRoleByToken(myGlobal.usertoken);
            var mdl = _replenish.GetByCollector(user.id);
            mdl = mdl.Where(filter).OrderByDescending(x => x.indate);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            var users = _user.GetAll();
            var output = mdl.ToPagedList(page ?? 1, myGlobal.PageSize);
            var model = (from o in output
                         join u in users on o.receivedbyid equals u.id into usr
                         from us in usr.DefaultIfEmpty()
                         select new ReplenishModelListModel()
                         {
                             id = o.id,
                             indate = o.indate,
                             no = o.no,
                             status = o.status,
                             receiverName = us == null ? "" : us.name,
                             receiveddate = o.receiveddate
                         }).ToList();
            ViewData["currentPage"] = page;
            return PartialView("_replenishByCollector", model);

        }

        public ActionResult Add()
        {
            var distributor = _distributor.GetDistributorByCollector(myGlobal.currentUser.id);
            var distributors = (from d in distributor
                                select new SelectListItem()
                                {
                                    Value = d.id.ToString(),
                                    Text = d.name
                                }).ToList();
            ViewData["distributors"] = distributors;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ReplenishModel model, string[] materials, decimal[] amount)
        {
            if (ModelState.IsValid)
            {
                var map = new Mapper(_mapperConfig);
                var dta = map.Map<ttreplenish>(model);
                var usr = myGlobal.currentUser;
                dta.createdby = usr.name;
                dta.collectorid = usr.id;
                dta.status = Status.DRAFT;
                var result = _replenish.Create(dta);
                int idx = 0;
                List<ReplenishDetailModel> replenishDetail = new List<ReplenishDetailModel>();
                //insert data detail.
                foreach (var itm in materials)
                {
                    replenishDetail.Add(new ReplenishDetailModel()
                    {
                        id = Guid.NewGuid(),
                        amount = amount[idx],
                        materialid = new Guid(itm),
                        replenishid = result.id,
                    });
                    idx++;
                }

                //setting price;
                var materialcurrentprice = _materialServices.GetMaterialCurrentPrice().ToList();
                var detail = (from rd in replenishDetail
                              join mp in materialcurrentprice on rd.materialid equals mp.id
                              select new ReplenishDetailModel()
                              {
                                  amount = rd.amount,
                                  id = rd.id,
                                  materialid = rd.materialid,
                                  price = mp.price,
                                  replenishid = rd.replenishid
                              }).ToList();
                var ddetail = map.Map<List<tdreplenish>>(detail).ToArray();
                _replenishDetail.CreateMultiple(ddetail);
                return RedirectToAction("Index");
            }
            else
            {
                var distributor = _distributor.GetDistributorByCollector(myGlobal.currentUser.id);
                var distributors = (from d in distributor
                                    select new SelectListItem()
                                    {
                                        Value = d.id.ToString(),
                                        Text = d.name
                                    }).ToList();
                ViewData["distributors"] = distributors;
                return View();
            }
        }

        public ActionResult Edit(Guid id)
        {

            var dta = _replenish.FindById(id);
            Mapper map = new Mapper(_mapperConfig);
            var model = map.Map<ReplenishModel>(dta);

            var distributor = _distributor.GetDistributorByCollector(myGlobal.currentUser.id);
            var distributors = (from d in distributor
                                select new SelectListItem()
                                {
                                    Value = d.id.ToString(),
                                    Text = d.name
                                }).ToList();

            ViewData["distributors"] = distributors;

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReplenishModel model, string[] materials, decimal[] amount)
        {
            if (ModelState.IsValid)
            {
                var dta = _replenish.FindById(model.id);
                Mapper map = new Mapper(_mapperConfig);
                var output = map.Map<ttreplenish>(dta);
                _replenish.Update(output);
                //detail..

                List<ReplenishDetailModel> replenishDetail = new List<ReplenishDetailModel>();
                //insert data detail.
                int idx = 0;
                foreach (var itm in materials)
                {
                    replenishDetail.Add(new ReplenishDetailModel()
                    {
                        id = Guid.NewGuid(),
                        amount = amount[idx],
                        materialid = new Guid(itm),
                        replenishid = model.id,
                    });
                    idx++;
                }

                //setting price;
                var materialcurrentprice = _materialServices.GetMaterialCurrentPrice();
                var detail = (from rd in replenishDetail
                              select new ReplenishDetailModel()
                              {
                                  amount = rd.amount,
                                  id = rd.id,
                                  materialid = rd.materialid,
                                  replenishid = rd.replenishid
                              }).ToList();
                var ddetail = map.Map<List<tdreplenish>>(detail).ToArray();


                var olddata = _replenishDetail.GetByReplenishId(model.id).ToList();
                var material = _material.GetAll();
                var tmpdetail = (from d in olddata 
                             select new ReplenishDetailModel()
                             {
                                 amount = d.amount,
                                 materialid = d.materialid,
                                 id=d.id,
                                 replenishid = d.replenishid
                             }).Distinct().ToList();
                var olddetail = map.Map<List<tdreplenish>>(tmpdetail).ToArray();
                _replenishDetail.UpdateMultiple(olddetail, ddetail);


                return RedirectToAction("Index");
            }
            else
            {

                var distributor = _distributor.GetDistributorByCollector(myGlobal.currentUser.id);
                var distributors = (from d in distributor
                                    select new SelectListItem()
                                    {
                                        Value = d.id.ToString(),
                                        Text = d.name
                                    }).ToList();
                return View(model);
            }


        }
        public JsonResult ReplenishDetailById(Guid id)
        {
            var dta = _replenishDetail.GetByReplenishId(id).ToList();
            var material = _material.GetAll();
            var model = (from d in dta
                         join m in material on d.materialid equals m.id
                         select new DetailReplenishListModel()
                         {
                             id=d.id,
                             amount = d.amount,
                             materialid = d.materialid,
                             material = m.name,
                             partno = m.partno,
                             unit = m.unit,
                             rfidcode = d.rfidcode
                         }).ToArray();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult addItem(Guid distributorid)
        {
            var material = _material.GetMaterialByDistributor(distributorid).ToList();
            var materials = (from m in material
                       select new SelectListItem()
                       {
                           Text = $"{m.partno} - {m.name}",
                           Value = m.id.ToString()
                       }).ToList();

            ViewData["materials"] = materials;
            return PartialView("_addItem");
        }

        [HttpPost]
        public JsonResult addItem(DetailReplenishModel dta)
        {
            ResponseModel resp = null;
            if (ModelState.IsValid)
            {

                var material = _material.FindById(dta.materialid);
                DetailReplenishListModel output = new DetailReplenishListModel()
                {
                    id=material.id,
                    amount = dta.amount,
                    material = material.name,
                    partno = material.partno,
                    unit = material.unit
                };

                resp = new ResponseModel()
                {
                    id = Guid.NewGuid(),
                    Messages = "success",
                    isSuccess = true
                };
                return Json(new { model = output, response = resp }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                resp = new ResponseModel()
                {
                    id = Guid.NewGuid(),
                    Messages = messages,
                    isSuccess = false
                };
                return Json(new { response = resp }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PostReplenish(Guid id)
        {
            ResponseModel resp = new ResponseModel();
            try {
                var res = _replenish.PostReplenish(id,ReplenishStatus.POSTEDBYCOLLECTOR);

                string messages = "Replenish is sent. Please check.";
                //sending notification to admin..
                var usr = myGlobal.currentUser;
                var adminrole = _role.getRoleByName(userRole.ADMIN).First();
                _message.SendMessageToRole(usr.id, adminrole.id, messages);


                if (res)
                {
                    resp = new ResponseModel()
                    {
                        id = id,
                        isSuccess = true,
                        Messages = "Data is Posted"
                    };
                }
                else
                {
                    resp = new ResponseModel()
                    {
                        id = id,
                        isSuccess = false,
                        Messages = "Fail to posted"
                    };
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }catch(Exception e)
            {
                resp = new ResponseModel()
                {
                    id = id,
                    isSuccess = false,
                    Messages = e.Message
                };
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Open(Guid id)
        {
            var dta = _replenish.FindById(id);
            Mapper map = new Mapper(_mapperConfig);
            var model = map.Map<ReplenishModel>(dta);

            var usr = _user.FindById(model.collectorid??Guid.NewGuid()).name;
            ViewData["user"] = usr;
            return View(model);
        }


# region Admin Digibox Receive Function

        private ActionResult AdminIndex()
        {
            return View("IndexAdmin");
        }

        public ActionResult ReplenishByAdmin(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<ttreplenish, bool>> filter = f => (f.no.Contains(searchString));

            if ((paramType == "no"))
            {
                filter = f => (f.no.Contains(searchString));
            };

            if (paramType == "tanggal")
            {
                filter = f => (f.indate.ToString().Contains(searchString));
            }

            var user = myGlobal.currentUser;
            var role = _role.getRoleByToken(myGlobal.usertoken);
            var mdl = _replenish.GetByAdmin();
            mdl = mdl.Where(filter).OrderByDescending(x => x.indate);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            var users = _user.GetAll();
            var output = mdl.ToPagedList(page ?? 1, myGlobal.PageSize);
            var model = (from o in output
                         join u in users on o.collectorid equals u.id into usr
                         from us in usr.DefaultIfEmpty()
                         select new ReplenishModelListModel()
                         {
                             id = o.id,
                             indate = o.indate,
                             no = o.no,
                             status = o.status,
                             collectorName = us == null ? "" : us.name,
                             receiveddate = o.receiveddate
                         }).ToList();
            ViewData["currentPage"] = page;
            return PartialView("_replenishByAdmin", model);

        }

        public ActionResult Receive(Guid id)
        {
            var dta = _replenish.FindById(id);
            Mapper map = new Mapper(_mapperConfig);
            var model = map.Map<ReplenishModel>(dta);

            var usr = _user.FindById(model.collectorid ?? Guid.NewGuid()).name;
            ViewData["user"] = usr;
            return View(model);
        }


        [HttpPost]
        public ActionResult Receive(ReplenishModel model, string[] ids, decimal[] receive)
        {
            List<ReplenishDetailReceiveModel> replenishDetail = new List<ReplenishDetailReceiveModel>();
            //insert data detail.
            int idx = 0;
            foreach (var itm in ids)
            {
                replenishDetail.Add(new ReplenishDetailReceiveModel()
                {
                    id = new Guid(itm),
                    receiveamount = receive[idx],
                    replenishid = model.id,
                });
                idx++;
            }

            var dta = _replenish.FindById(model.id);
            dta.receivedbyid = myGlobal.currentUser.id;
            dta.status = ReplenishStatus.RECEIVED;
            _replenish.Received(dta,replenishDetail);

            string messages = $"Replenish for {String.Format("{0: dd MMM yyyy}", dta.indate)} is received! ";
            //sending notification to admin..
            var usr = myGlobal.currentUser;
            _message.SendMessageToUser(usr.id, dta.collectorid??Guid.NewGuid(), messages);

            return RedirectToAction("Index");
        }

        public JsonResult ReplenishItemsById(Guid id)
        {
            var dta = _replenishDetail.GetByReplenishId(id).ToList();
            var material = _material.GetAll();
            var model = (from d in dta
                         join m in material on d.materialid equals m.id
                         select new DetailReplenishListModel()
                         {
                             amount = d.amount,
                             materialid = d.materialid,
                             id=d.id,
                             material = m.name,
                             partno = m.partno,
                             unit = m.unit
                         }).ToArray();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        #endregion

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            ResponseModel response = null;
            try
            {

                var dta = _replenish.FindById(id);
                dta.deletedby = myGlobal.currentUser.name;
                var rst = _replenish.Delete(dta);
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

        public ActionResult PrintRFID(Guid id)
        {
            var detail = _replenishDetail.FindById(id);
            var material = _material.FindById(detail.materialid);
            var distributor = _distributor.FindById(material.distributor);
            var model = new OpnameDetail()
            {
                partno = material.partno,
                materialid = material.id,
                materialName = material.name,
                rfidcode = detail.rfidcode,
                inOutName = distributor.name,
                description = material.description,
            };
            model.qrimage = myGlobal.qrcode(detail.rfidcode);
            model.barcodeimage = myGlobal.barcode(detail.rfidcode);

            return new Rotativa.ViewAsPdf("PrintRFID", model);
        }

        public ActionResult PrintAll(Guid id)
        {
            var opnamedetail = _replenishDetail.GetByReplenishId(id).ToList();
            var material = _material.GetAll().ToList();
            var distributor = _distributor.GetAll().ToList();

            var model = (from o in opnamedetail
                         join m in material on o.materialid equals m.id
                         join d in distributor on m.distributor equals d.id
                         select new OpnameDetail()
                         {
                             partno = m.partno,
                             materialid = o.materialid,
                             materialName = m.name,
                             rfidcode = o.rfidcode,
                             inOutName = d.name,
                             description = m.description,
                             qrimage = myGlobal.qrcode(o.rfidcode),
                             barcodeimage = myGlobal.barcode(o.rfidcode),
                         }).ToList();
            return new Rotativa.ViewAsPdf("PrintAll", model);
        }

    }
}