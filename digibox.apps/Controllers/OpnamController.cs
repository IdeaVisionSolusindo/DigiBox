using ClosedXML.Excel;
using digibox.apps.Models;
using digibox.apps.Modules;
using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using PagedList;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using ZXing.QrCode.Internal;

namespace digibox.apps.Controllers
{
    public class OpnamController : Controller
    {
        private readonly IRoleService _role;
        private readonly IOpnamRepository _opnam;
        private readonly IUserRepository _user;
        private readonly IAttributeRepository _attribute;
        private readonly IMaterialRepository _material;
        private readonly IOpnameDetailRepository _opnameDetail;
        private readonly IDistributorRepository _distributor;
        private readonly IInventoryRepository _inventory;

        // GET: Opnam
        public OpnamController(IOpnamRepository opnam, IOpnameDetailRepository opnameDetail, IInventoryRepository inventory, IDistributorRepository distributor, IMaterialRepository material, IAttributeRepository attribute, IUserRepository user, IRoleService role) 
            => (_opnam, _opnameDetail, _inventory, _distributor, _material, _attribute,  _user, _role) = (opnam, opnameDetail, inventory, distributor, material, attribute, user, role);

        public ActionResult Index()
        {
            var role = _role.getRoleByToken(myGlobal.usertoken);
            if (role == userRole.ADMIN)
            {
                return IndexAdmin();
            }
            if (role == userRole.SUPERADMIN)
            {
                return IndexSuperadmin();
            }

            return View();
        }


        #region Superadmin
        private ActionResult IndexSuperadmin()
        {
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Date",
                Value = "date"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Name",
                Value = "name",
                Selected = true
            });
            ViewData["parameters"] = parameters;
            return View("IndexSuperadmin");
        }

        public ActionResult OpenDataBySuperadmin(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<OpnamListModel, bool>> filter = f => (f.CheckeByName.Contains(searchString));

            if ((paramType == "partno"))
            {
                filter = f => (f.opnamdate.ToString().Contains(searchString));
            };

            if (paramType == "name")
            {
                filter = f => (f.CheckeByName.Contains(searchString));
            }

                //filter by user collector;
            var mdl = _opnam.openBySuperAdmin();


            var hasil = mdl.Where(filter).OrderBy(x => x.CheckeByName);//.ToList();
            int maxpage = Convert.ToInt16(Math.Ceiling(hasil.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            //var model = mdl.OrderBy(x => x.name);
            var model = hasil.ToPagedList(page ?? 1, myGlobal.PageSize);
            ViewData["currentPage"] = page;
            return PartialView("_openDataBySuperadmin", model);
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
                string newfilename = Server.MapPath($"~/Content/temp/{myGlobal.currentUser.id}_upload_stockOpname.xlsx");
                if (System.IO.File.Exists(newfilename))
                {
                    System.IO.File.Delete(newfilename);
                }
                file.SaveAs(newfilename);

                List<OpnameDetail> content = new List<OpnameDetail>();
                using (var workbook = new XLWorkbook(newfilename))
                {
                    var worksheet = workbook.Worksheet("main");
                    //Header
                    string opnameType = worksheet.Cell(3, 2).Value.ToString();
                    //check if exists
                    var opname = _attribute.GetByName("OPNAM-TYPE").Where(c=>c.attributevalue==opnameType).FirstOrDefault();
                    if (opname == null)
                    {

                        response = new ResponseModel()
                        {
                            id = Guid.Empty,
                            isSuccess = false,
                            Messages = "Invalid Opname Type"
                        };
                        return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                    }
                    //var user
                    string user = worksheet.Cell(2, 2).Value.ToString();
                    //check if user exist
                    var usr = _user.GetAll().Where(x => x.name == user).FirstOrDefault();
                    if (usr == null)
                    {

                        response = new ResponseModel()
                        {
                            id = Guid.Empty,
                            isSuccess = false,
                            Messages = "Name is not found"
                        };
                        return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                    }

                    var ddate = worksheet.Cell(1, 2).Value;

                    OpnamListModel header = new OpnamListModel()
                    {
                        opnamdate = Convert.ToDateTime(ddate),
                        opnamtype = opname.id,
                        OpnameTypeValue = opname.attributevalue,
                        CheckeByName = usr.name,
                        checkedby = usr.id,
                    };
                    
                    int maxData = worksheet.RowCount();
                    for (int idx = 6; idx < maxData; idx++)
                    {
                        if (idx == 6)
                        {
                            //respon error di sini..
                            int nStartcol = opname.attributevalue==OpnameType.INITIALSTOCK?2:1;
                                //initial stock, hanya baca kolom kedua, kolom pertama diabaikan
                            if (worksheet.Cell(idx, nStartcol).Value.ToString() == "")
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
                            int nStartcol = opname.attributevalue==OpnameType.INITIALSTOCK?2:1;
                            if (worksheet.Cell(idx, nStartcol).Value.ToString() == "")
                            {
                                break;
                            }
                        }

                        //for opname, it needs to have rfid on inventory
                        if (opname.attributevalue == OpnameType.STOCKOPNAME)
                        {
                            var rfidCode = worksheet.Cell(idx, 1).Value.ToString();
                            var iv = _inventory.GetByRFIDCode(rfidCode);
                            if (iv.Count() == 0)
                            {
                                response = new ResponseModel()
                                {
                                    id = Guid.Empty,
                                    isSuccess = false,
                                    Messages = $"{rfidCode} is not found"
                                };
                                return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        //material.
                        var partno = worksheet.Cell(idx, 2).Value.ToString();
                        var material = _material.GetAll().Where(x => x.partno == partno).FirstOrDefault();
                        if (material == null)
                        {
                            response = new ResponseModel()
                            {
                                id = Guid.Empty,
                                isSuccess = false,
                                Messages = worksheet.Cell(idx, 2).Value.ToString() + " Name is not found"
                            };
                            return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                        }

                        var inout = worksheet.Cell(idx, 4).Value.ToString();

                        //converting amount
                        var samount = worksheet.Cell(idx, 5).Value.ToString();
                        decimal amount;
                        if(!Decimal.TryParse(samount,out amount))
                        {
                            response = new ResponseModel()
                            {
                                id = Guid.Empty,
                                isSuccess = false,
                                Messages = " Invalid Amount " + samount + " on Row " + idx
                            };
                            return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                        }


                        OpnameDetail opnameDetail = new OpnameDetail()
                        {
                            rfidcode = worksheet.Cell(idx, 1).Value.ToString(),
                            partno = worksheet.Cell(idx, 2).Value.ToString(),
                            materialid = material.id,
                            materialName = material.name,
                            inout = inout == "IN" ? 1 : -1,
                            inOutName = inout,
                            amount = amount,
                            id = Guid.NewGuid(),
                            description= worksheet.Cell(idx, 6).Value.ToString(),
                        };
                        content.Add(opnameDetail);
                    }
                    Session["opnamedetail"] = content;
                    Session["opnamemaster"] = header;
                    ViewData["opnameType"] = opname.attributevalue;
                }
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = true,
                    Messages = "data uploaded"
                };


                return PartialView("_displayUploadedData");
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


        public ActionResult DownloadTemplate()
        {
            var xlfile = Server.MapPath("~/Content/templates/opnameStockTemplate.xlsx");
            string contentType = MimeMapping.GetMimeMapping(xlfile);
            return File(xlfile, contentType);
        }

        [HttpPost]
        public ActionResult SaveMultiple()
        {
            ResponseModel response = null;
            try
            {

                var header = Session["opnamemaster"] as OpnamListModel;
                var content = Session["opnamedetail"] as List<OpnameDetail>;

                if (header == null)
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = "Invalid Data",
                        isSuccess = false
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                if (content.Count == 0)
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = "Content Empty",
                        isSuccess = false
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                //saving header
                ttopnam opnam = new ttopnam()
                {
                    checkedby = header.checkedby,
                    id = Guid.NewGuid(),
                    isdeleted = false,
                    opnamdate = header.opnamdate,
                    opnamtype = header.opnamtype,
                    status = Status.DRAFT,
                    createdby = myGlobal.currentUser.name
                };

                var opnameDetail = new List<tdopnam>();

                CultureInfo CI = new CultureInfo("id-ID");
                System.Globalization.Calendar Cal = CI.Calendar;
                DateTime currentdate = opnam.opnamdate??DateTime.Now;
                int week = Cal.GetWeekOfYear(currentdate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                string precode = string.Format("{0:yy}{1}", currentdate, week);

                foreach (var itm in content)
                {
                    var rfidcode = $"{precode}{itm.partno}";
                    tdopnam dopname = new tdopnam()
                    {
                        amount = itm.amount,
                        inout = itm.inout,
                        createdat = DateTime.Now,
                        isdeleted = false,
                        materialid= itm.materialid,
                        opnamid = opnam.id,
                        createdby = myGlobal.currentUser.name,
                        rfidcode = rfidcode,
                        id=Guid.NewGuid(),
                        description = itm.description
                    };
                    opnameDetail.Add(dopname);
                }
                var hasil = _opnam.CrateMultiple(opnam,opnameDetail.ToArray());
                if (hasil == "")
                {
                    response = new ResponseModel()
                    {
                        id = Guid.Empty,
                        Messages = "Opname Added",
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
            catch (Exception ex)
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

        public ActionResult Edit(Guid id)
        {
            var opname = _opnam.FindById(id);
            var opnamedetail = _opnameDetail.GetByOpnameID(id).ToList();
            var opnameType = _attribute.FindById(opname.opnamtype??Guid.Empty);
            var usr = _user.FindById(opname.checkedby ?? Guid.Empty);

            OpnamListModel header = new OpnamListModel()
            {
                opnamdate = opname.opnamdate,
                opnamtype = opname.id,
                OpnameTypeValue = opnameType.attributevalue,
                CheckeByName = usr.name,
                checkedby = usr.id,
                id = opname.id,
                status = opname.status
            };

            var material = _material.GetAll().ToList();

            var content = (from od in opnamedetail
                           join m in material on od.materialid equals m.id
                                select new OpnameDetail
                                {
                                    amount = od.amount,
                                    description = od.description,
                                    id = od.id,
                                    inout = od.inout,
                                    inOutName = od.inout == -1 ? "OUT" : "IN",
                                    materialid = od.materialid,
                                    materialName = m.name,
                                    opnamid = od.opnamid,
                                    partno = m.partno,
                                    rfidcode = od.rfidcode
                                }).ToList();
            Session["opnamedetail"] = content;
            Session["opnamemaster"] = header;
            return View();
        }

        public ActionResult removeDetail(Guid id)
        {
            var content = Session["opnamedetail"] as List<OpnameDetail>;
            var removed = content.Where(x => x.id == id).FirstOrDefault();
            content.Remove(removed);
            return PartialView("_removeDetail");
        }

        public ActionResult SaveEdit()
        {
            var response = new ResponseModel();
            try
            {
                var header = Session["opnamemaster"] as OpnamListModel;
                var opnamedetail = _opnameDetail.GetByOpnameID(header.id).ToList();
                var content = Session["opnamedetail"] as List<OpnameDetail>;
                var current = from c in content
                              select new tdopnam()
                              {
                                  id = c.id,
                                  amount = c.amount,
                              };
                _opnameDetail.saveMultiple(opnamedetail.ToArray(), current.ToArray());

                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = "Data is Changed",
                    isSuccess = true
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }catch(Exception e)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = "Change is Failed",
                    isSuccess = false
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Open(Guid id)
        {
            var opname = _opnam.FindById(id);
            var opnamedetail = _opnameDetail.GetByOpnameID(id).ToList();
            var opnameType = _attribute.FindById(opname.opnamtype ?? Guid.Empty);
            var usr = _user.FindById(opname.checkedby ?? Guid.Empty);

            OpnamListModel header = new OpnamListModel()
            {
                opnamdate = opname.opnamdate,
                opnamtype = opname.id,
                OpnameTypeValue = opnameType.attributevalue,
                CheckeByName = usr.name,
                checkedby = usr.id,
                id = opname.id,
                status = opname.status
            };

            var material = _material.GetAll().ToList();

            var content = (from od in opnamedetail
                           join m in material on od.materialid equals m.id
                           select new OpnameDetail
                           {
                               amount = od.amount,
                               description = od.description,
                               id = od.id,
                               inout = od.inout,
                               inOutName = od.inout == -1 ? "OUT" : "IN",
                               materialid = od.materialid,
                               materialName = m.name,
                               opnamid = od.opnamid,
                               partno = m.partno,
                               rfidcode = od.rfidcode
                           }).ToList();
            Session["opnamedetail"] = content;
            Session["opnamemaster"] = header;
            return View();
        }

        public ActionResult Post(Guid id)
        {
            var result = _opnam.postOpname(id,Status.APPROVED);
            ResponseModel response = new ResponseModel()
            {
                id = Guid.Empty,
                Messages = "Change is Failed",
                isSuccess = result
            };
            if (!result)
            {
                response.Messages = "Post is Failed";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Guid id)
        {
            ResponseModel response = new ResponseModel()
            {
                id = Guid.Empty,
                Messages = "Data is Deleted",
                isSuccess = true
            };
            try
            {
                var opnam = _opnam.FindById(id);
                opnam.deletedby = myGlobal.currentUser.name;
                _opnam.Delete(opnam);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = e.Message,
                    isSuccess = false
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

            #endregion

            public ActionResult PrintRFID(Guid id)
        {
            var detail = _opnameDetail.FindById(id);
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
            model.barcodeimage= myGlobal.barcode(detail.rfidcode);

            return new Rotativa.ViewAsPdf("PrintRFID", model);
        }

        public ActionResult PrintAll(Guid id)
        {
            var opnamedetail = _opnameDetail.GetByOpnameID(id).ToList();
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
                             description = o.description,
                             qrimage = myGlobal.qrcode(o.rfidcode),
                             barcodeimage = myGlobal.barcode(o.rfidcode),
                         }).ToList();
            return new Rotativa.ViewAsPdf("PrintAll", model);
        }

        #region Admin
        private ActionResult IndexAdmin()
        {
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Date",
                Value = "date"
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Name",
                Value = "name",
                Selected = true
            });
            ViewData["parameters"] = parameters;
            return View("IndexAdmin");
        }

        public ActionResult OpenDataByAdmin(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {
            if (page < 1)
                page = 1;
            Expression<Func<OpnamListModel, bool>> filter = f => (f.CheckeByName.Contains(searchString));

            if ((paramType == "partno"))
            {
                filter = f => (f.opnamdate.ToString().Contains(searchString));
            };

            if (paramType == "name")
            {
                filter = f => (f.CheckeByName.Contains(searchString));
            }

            var user = myGlobal.currentUser;
            //filter by user collector;
            var mdl = _opnam.openByAdmin(user.id);


            var hasil = mdl.Where(filter).OrderBy(x => x.CheckeByName);//.ToList();
            int maxpage = Convert.ToInt16(Math.Ceiling(hasil.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }
            //var model = mdl.OrderBy(x => x.name);
            var model = hasil.ToPagedList(page ?? 1, myGlobal.PageSize);
            ViewData["currentPage"] = page;
            return PartialView("_openDataByAdmin", model);
        }
        public ActionResult UploadByAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadByAdmin(HttpPostedFileBase file)
        {
            ResponseModel response = null;
            try
            {
                string newfilename = Server.MapPath($"~/Content/temp/{myGlobal.currentUser.id}_upload_stockOpname.xlsx");
                if (System.IO.File.Exists(newfilename))
                {
                    System.IO.File.Delete(newfilename);
                }
                file.SaveAs(newfilename);

                List<OpnameDetail> content = new List<OpnameDetail>();
                using (var workbook = new XLWorkbook(newfilename))
                {
                    var worksheet = workbook.Worksheet("main");
                    //Header
                    string opnameType = worksheet.Cell(3, 2).Value.ToString();
                    //check if exists
                    var opname = _attribute.GetByName("OPNAM-TYPE").Where(c => c.attributevalue == opnameType).FirstOrDefault();
                    if (opname == null)
                    {

                        response = new ResponseModel()
                        {
                            id = Guid.Empty,
                            isSuccess = false,
                            Messages = "Invalid Opname Type"
                        };
                        return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                    }
                    //var user
                    string user = worksheet.Cell(2, 2).Value.ToString();
                    //check if user exist
                    var usr = _user.GetAll().Where(x => x.name == user).FirstOrDefault();
                    
                    if (usr == null)
                    {

                        response = new ResponseModel()
                        {
                            id = Guid.Empty,
                            isSuccess = false,
                            Messages = "Name is not found"
                        };
                        return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                    }
                    if (usr.id != myGlobal.currentUser.id)
                    {
                        response = new ResponseModel()
                        {
                            id = Guid.Empty,
                            isSuccess = false,
                            Messages = "Please Use your user name on the file import"
                        };
                        return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                    }
                    var ddate = worksheet.Cell(1, 2).Value;

                    OpnamListModel header = new OpnamListModel()
                    {
                        opnamdate = Convert.ToDateTime(ddate),
                        opnamtype = opname.id,
                        OpnameTypeValue = opname.attributevalue,
                        CheckeByName = usr.name,
                        checkedby = usr.id,
                    };

                    CultureInfo CI = new CultureInfo("id-ID");
                    System.Globalization.Calendar Cal = CI.Calendar;
                    DateTime currentdate = header.opnamdate ?? DateTime.Now;
                    int week = Cal.GetWeekOfYear(currentdate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                    int maxData = worksheet.RowCount();
                    for (int idx = 6; idx < maxData; idx++)
                    {
                        if (idx == 6)
                        {
                            if(opnameType== OpnameType.INITIALSTOCK)
                            {
                                if (worksheet.Cell(idx, 2).Value.ToString() == "")
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
                                    response = new ResponseModel()
                                    {
                                        id = Guid.Empty,
                                        isSuccess = false,
                                        Messages = "Data kosong"
                                    };
                                    return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            //respon error di sini..
                        }
                        else
                        {
                            if (worksheet.Cell(idx, 1).Value.ToString() == "")
                            {
                                break;
                            }
                        }

                        //material.
                        var partno = worksheet.Cell(idx, 2).Value.ToString();
                        var material = _material.GetAll().Where(x => x.partno == partno).FirstOrDefault();
                        if (material == null)
                        {
                            response = new ResponseModel()
                            {
                                id = Guid.Empty,
                                isSuccess = false,
                                Messages = worksheet.Cell(idx, 1).Value.ToString() + " Name is not found"
                            };
                            return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                        }

                        var inout = worksheet.Cell(idx, 4).Value.ToString();

                        //converting amount
                        var samount = worksheet.Cell(idx, 5).Value.ToString();
                        decimal amount;
                        if (!Decimal.TryParse(samount, out amount))
                        {
                            response = new ResponseModel()
                            {
                                id = Guid.Empty,
                                isSuccess = false,
                                Messages = " Invalid Amount " + samount + " on Row " + idx
                            };
                            return Json(new { res = response }, JsonRequestBehavior.AllowGet);
                        }

                        string precode = string.Format("{0:yy}{1}", header.opnamdate, week);
                        var rfidcode = opnameType==OpnameType.INITIALSTOCK?$"{precode}{worksheet.Cell(idx, 2).Value.ToString()}": worksheet.Cell(idx, 1).Value.ToString();
                        OpnameDetail opnameDetail = new OpnameDetail()
                        {
                            partno = worksheet.Cell(idx, 2).Value.ToString(),
                            materialid = material.id,
                            materialName = material.name,
                            inout = inout == "IN" ? 1 : -1,
                            inOutName = inout,
                            amount = amount,
                            id = Guid.NewGuid(),
                            description = worksheet.Cell(idx, 5).Value.ToString(),
                            rfidcode = rfidcode
                        };
                        content.Add(opnameDetail);
                    }
                    Session["opnamedetail"] = content;
                    Session["opnamemaster"] = header;
                }
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    isSuccess = true,
                    Messages = "data uploaded"
                };

                return PartialView("_displayUploadedData");
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
        #endregion


    }
}