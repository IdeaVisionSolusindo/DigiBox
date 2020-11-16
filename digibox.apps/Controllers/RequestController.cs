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
using System.Data.Entity.Core.Mapping;
using System.Data.OleDb;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace digibox.apps.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRoleService _role;
        private readonly IRequestRepository _request;
        private readonly IUserRepository _user;
        private readonly IMaterialRepository _material;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IRequestDetailRepository _requestDetail;
        private readonly IMessageRepository _message;
        private readonly IInventoryRepository _inventory;

        public RequestController(IRequestRepository request, IRequestDetailRepository requestDetail, IMaterialRepository material, IInventoryRepository inventory, IUserRepository user, IMessageRepository message, IRoleService role, MapperConfiguration mapperConfiguration) =>
            (_request, _requestDetail, _material, _inventory, _user, _message, _role, _mapperConfiguration) = (request, requestDetail, material, inventory, user, message, role, mapperConfiguration);
        // GET: Request
        public ActionResult Index()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            if (role == userRole.ADMIN)
            {
                return IndexAdmin();
            }
            if (role == userRole.USER)
            {
                return IndexUser();
            }

            return View();
        }

        #region User Request
        private ActionResult IndexUser()
        {
            return View("IndexUser");
        }

        public ActionResult RequestByUser(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<ttrequest, bool>> filter = f => (f.no.Contains(searchString));

            if ((paramType == "no"))
            {
                filter = f => (f.no.Contains(searchString));
            };

            if (paramType == "tanggal")
            {
                filter = f => (f.requestdate.ToString().Contains(searchString));
            }

            var user = myGlobal.currentUser;
            var role = _role.getRoleByToken(myGlobal.usertoken);
            var mdl = _request.GetByUser(user.id);
            mdl = mdl.Where(filter).OrderByDescending(x => x.requestdate);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            var users = _user.GetAll();
            var output = mdl.ToPagedList(page ?? 1, myGlobal.PageSize);
            var model = (from o in output
                         join u in users on o.handedoverby equals u.id into usr
                         from us in usr.DefaultIfEmpty()
                         select new RequestListModel()
                         {
                             id = o.id,
                             requestdate = o.requestdate,
                             no = o.no,
                             status = o.status,
                             RequestedByName = user.name,
                             HandedOverByName = us == null ? "" : us.name,
                             receiveddate = o.receiveddate
                         }).ToList();
            ViewData["currentPage"] = page;
            return PartialView("_requestByUser", model);

        }


        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RequestModel model, string[] materials, decimal[] amount)
        {
            if (ModelState.IsValid)
            {
                Mapper map = new Mapper(_mapperConfiguration);
                var dta = map.Map<ttrequest>(model);
                var usr = myGlobal.currentUser;
                dta.createdby = usr.name;
                dta.status = Status.DRAFT;
                dta.userid = usr.id;
                var result = _request.Create(dta);

                //add detail;
                int idx = 0;
                List<RequestDetailModel> detail = new List<RequestDetailModel>();
                //insert data detail.
                foreach (var itm in materials)
                {
                    detail.Add(new RequestDetailModel()
                    {
                        id = Guid.NewGuid(),
                        amount = amount[idx],
                        materialid = new Guid(itm),
                        requestid = result.id,
                    });
                    idx++;
                }

                //setting price;
                var ddetail = map.Map<List<tdrequest>>(detail).ToArray();
                _requestDetail.CreateMultiple(ddetail);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }


        public ActionResult Edit(Guid id)
        {

            var dta = _request.FindById(id);
            Mapper map = new Mapper(_mapperConfiguration);
            var model = map.Map<RequestModel>(dta);
            return View(model);
        }


        public JsonResult SelectedMaterial(Guid id)
        {
            var dta = _requestDetail.GetByRequestId(id).ToList();
            var material = _material.GetAll().ToList();
            var model = (from d in dta
                         join m in material on d.materialid equals m.id
                         select new RequestDetailListModel()
                         {
                             id = d.materialid,
                             partno = m.partno,
                             name = m.name,
                             unit = m.unit,
                             amount = d.amount,
                             requestid = d.requestid
                         }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RequestModel model, string[] materials, decimal[] amount)
        {
            if (ModelState.IsValid)
            {
                Mapper map = new Mapper(_mapperConfiguration);
                var dta = map.Map<ttrequest>(model);
                _request.Update(dta);

                //update detail;

                List<RequestDetailModel> requestDetail = new List<RequestDetailModel>();
                //insert data detail.
                int idx = 0;
                foreach (var itm in materials)
                {
                    requestDetail.Add(new RequestDetailModel()
                    {
                        id = Guid.NewGuid(),
                        amount = amount[idx],
                        materialid = new Guid(itm),
                        requestid = model.id,
                    });
                    idx++;
                }
                var ddetail = map.Map<List<tdrequest>>(requestDetail).ToArray();
                var olddata = _requestDetail.GetByRequestId(model.id).ToArray();
                _requestDetail.UpdateMultiple(olddata, ddetail);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult PostRequest(Guid id)
        {
            ResponseModel resp = new ResponseModel();
            try
            {
                var res = _request.PostRequest(id, RequestStatus.POSTEDBYUSER);

                string messages = "Request is sent. Please check.";
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
            }
            catch (Exception e)
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

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            ResponseModel response = null;
            try
            {

                var dta = _request.FindById(id);
                dta.deletedby = myGlobal.currentUser.name;
                var rst = _request.Delete(dta);
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

        #endregion


        #region Admin Request

        private ActionResult IndexAdmin()
        {
            return View("IndexAdmin");

        }


        public ActionResult RequestByAdmin(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<ttrequest, bool>> filter = f => (f.no.Contains(searchString));

            if ((paramType == "no"))
            {
                filter = f => (f.no.Contains(searchString));
            };

            if (paramType == "tanggal")
            {
                filter = f => (f.requestdate.ToString().Contains(searchString));
            }

            var user = myGlobal.currentUser;
            var role = _role.getRoleByToken(myGlobal.usertoken);
            var mdl = _request.GetByAdmin().Where(x => x.status != RequestStatus.DRAFT);
            mdl = mdl.Where(filter).OrderByDescending(x => x.requestdate);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            var users = _user.GetAll();
            var output = mdl.ToPagedList(page ?? 1, myGlobal.PageSize);
            var model = (from o in output
                         join u in users on o.userid equals u.id into usr
                         from us in usr.DefaultIfEmpty()
                         select new RequestListModel()
                         {
                             id = o.id,
                             requestdate = o.requestdate,
                             no = o.no,
                             status = o.status,
                             RequestedByName = us == null ? "" : us.name,
                             receiveddate = o.receiveddate
                         }).ToList();
            ViewData["currentPage"] = page;
            return PartialView("_requestByAdmin", model);

        }


        public ActionResult Approve(Guid id)
        {
            var dta = _request.FindById(id);
            Mapper map = new Mapper(_mapperConfiguration);
            var model = map.Map<RequestModel>(dta);

            var usr = _user.FindById(model.userid ?? Guid.NewGuid()).name;
            ViewData["user"] = usr;
            return View(model);
        }


        public ActionResult ApprovalRequestDetail(Guid id)
        {
            var dta = _requestDetail.GetByRequestId(id).ToList();
            var material = _material.GetAll().ToList();
            var model = (from d in dta
                         join m in material on d.materialid equals m.id
                         select new RequestDetailListModel()
                         {
                             materialid = d.materialid,
                             id = d.id,
                             partno = m.partno,
                             name = m.name,
                             unit = m.unit,
                             amount = d.amount,
                             requestid = d.requestid
                         }).ToList();

            return PartialView("_approvalRequestDetail", model);
        }

        [HttpPost]
        public ActionResult Approve(RequestModel model, string[] requestid, string[] inventoryid, decimal[] amount)
        {

            //belum implementasi
            //masukkan data outgoing..
            var idx = 0;
            List<tdoutgoing> outgoigitems=new List<tdoutgoing>();
            foreach (var itm in inventoryid) {
                tdoutgoing outgoing = new tdoutgoing()
                {
                    id=Guid.NewGuid(),
                    amount = amount[idx],
                    inventoryid = new Guid(itm),
                    drequestid = new Guid(requestid[idx])
                };
                outgoigitems.Add(outgoing);
                idx++;
            }

            //save outgoing multiple..

            //save level detail untuk receiveamount.
            //group by requestid
            List<RequestDetailReceiveModel> requestDetail = outgoigitems
                .GroupBy(x => x.drequestid)
                .Select(y => new RequestDetailReceiveModel()
                {
                    receiveamount = y.Sum(x => x.amount) ?? 0,
                    requestid = y.First().drequestid ?? Guid.Empty,
                }).ToList();


            var dta = _request.FindById(model.id);
            dta.handedoverby = myGlobal.currentUser.id;
            dta.status = RequestStatus.RECEIVED;
            _request.Approve(dta, requestDetail, outgoigitems);

            string messages = $"Request for {String.Format("{0: dd MMM yyyy}", dta.requestdate)} is received! ";
            //sending notification to admin..
            var usr = myGlobal.currentUser;
            _message.SendMessageToUser(usr.id, dta.userid ?? Guid.NewGuid(), messages);

            return RedirectToAction("Index");
        }

        public ActionResult AddOutputMaterial(Guid requestid, Guid materialid)
        {
            //ambil material yang ada dalam inventory di join dengan material yang ada, tampilkan rfid dan jumlah sisanya.
            //masukin ke dropdownlist
            //get material yang ada dalam inventory.
            var iv = _inventory.GetByMaterial(materialid).Where(x => x.replstock > 0).ToList();
            var ivs = (from i in iv
                       select new SelectListItem()
                       {
                           Text = $"{i.rfidcode}",
                           Value = i.id.ToString()
                       }).ToList();

            ViewData["materialinventory"] = ivs;
            OutgoingModel model = new OutgoingModel()
            {
                drequestid = requestid
            };

            return PartialView("_addOutputMaterial",model);
        }


        [HttpPost]

        public JsonResult AddOutputMaterial(OutgoingModel model)
        {
            //check jumlah current pada inventory.

            ResponseModel response = new ResponseModel();
            var iv = _inventory.FindById(model.inventoryid ?? Guid.Empty);
            if (iv.replstock <= model.amount)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = false,
                    Messages = $"Total Amount of {iv.rfidcode} is {iv.replstock}"
                };
                return Json(new { res = response });
            }

            response = new ResponseModel()
            {
                id = Guid.Empty,
                isSuccess = true,
                Messages = $"Data Added"
            };

            model.rfidcode = iv.rfidcode;
            model.id = Guid.NewGuid();
            model.materialid = iv.materialid;
            return Json(new { dta = model, res = response });

        }

        #endregion
    
    
        public ActionResult Open(Guid id)
        {
            var dta = _request.FindById(id);
            Mapper map = new Mapper(_mapperConfiguration);
            var model = map.Map<RequestModel>(dta);

            var dtadetil = _requestDetail.GetByRequestId(id).ToList();
            var material = _material.GetAll().ToList();
            var modeldetail = (from d in dtadetil
                         join m in material on d.materialid equals m.id
                         select new RequestDetailListModel()
                         {
                             materialid = d.materialid,
                             id = d.id,
                             partno = m.partno,
                             name = m.name,
                             unit = m.unit,
                             amount = d.amount,
                             requestid = d.requestid,
                             receiveamount = d.receiveamount??0,
                             receivedate = d.receivedate
                         }).ToList();

            //Detail Data
            ViewData["modeldetail"] = modeldetail;
            return View(model);
        }
    
    }
}