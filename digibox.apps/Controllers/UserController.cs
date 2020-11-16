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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace digibox.apps.Controllers
{
    public class UserController : Controller
    {
        private readonly IRoleRepository _role;
        private readonly IUserRepository _users;
        private readonly IAttributeRepository _attribute;
        private readonly MapperConfiguration _mapperConfig;
        private readonly IRoleService _roleService;
        private readonly IMaterialServices _materialServices;
        private readonly IFunctionRepository _functionRepository;

        // GET: User
        public UserController(IUserRepository users, IMaterialServices materialServices, IRoleRepository role, IRoleService roleService, IFunctionRepository functionRepository, IAttributeRepository attribute, MapperConfiguration mapperConfig) =>
            (_users, _materialServices, _role, _roleService, _functionRepository,_attribute, _mapperConfig) = (users, materialServices, role, roleService, functionRepository, attribute, mapperConfig);
        public ActionResult Index()
        {
            /*var currentrole = _roleService.getRoleByToken(myGlobal.usertoken);

            if (currentrole != userRole.ADMIN)
            {
                return RedirectToAction("UnAuthorize", "ErrorHandler");
            }*/
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Name",
                Value = "name",
                Selected = true
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Role",
                Value = "role"
            });
            ViewData["parameters"] = parameters;
            return View();
        }

        public ActionResult OpenData(string sortOrder = "ASC", String paramType = "", string searchString = "", int? page = 1)
        {

            var mdl = _users.GetUsers();

            if (page < 1)
                page = 1;

            Expression<Func<UserListModel, bool>> filter = f => (f.name.Contains(searchString));

            mdl = mdl.Where(filter).OrderBy(f => f.name);
            int maxpage = Convert.ToInt16(Math.Ceiling(mdl.Count() * 1.0 / myGlobal.PageSize));
            ViewData["maxpage"] = maxpage;
            if (page > maxpage)
            {
                page = maxpage > 0 ? maxpage : page;
            }

            ViewData["currentPage"] = page;
            return PartialView("_openData", mdl.ToPagedList(page ?? 1, myGlobal.PageSize));

        }


        [HttpPost]
        public JsonResult Delete(Guid id)
        {

            ResponseModel response = null;
            try
            {
                var rst = _users.Delete(id);
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

        public ActionResult Add()
        {
            UserModel usm = new UserModel();
            var role = _role.GetAll();
            var optRole = (from r in role
                           select new SelectListItem()
                           {
                               Text = r.name,
                               Value = r.id.ToString()
                           }).ToList();
            ViewData["roles"] = optRole;

            return PartialView("_add", usm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(UserModel model)
        {
            ResponseModel response = null;

            if (ModelState.IsValid)
            {
                try
                {
                    Mapper map = new Mapper(_mapperConfig);
                    var dta = map.Map<tmuser>(model);

                    var user= _users.Create(dta);
                    response = new ResponseModel()
                    {
                        id = user.id,
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


        public ActionResult Edit(Guid id)
        {

            var dta = _users.FindById(id);
            Mapper map = new Mapper(_mapperConfig);
            var model = map.Map<UserModel>(dta);

            var role = _role.GetAll();
            var optRole = (from r in role
                           select new SelectListItem()
                           {
                               Text = r.name,
                               Value = r.id.ToString(),
                               Selected = r.id == model.roleid
                           }).ToList();
            ViewData["roles"] = optRole;

            //FTZ ZONE
            return PartialView("_edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(UserModel model)
        {
            ResponseModel response = null;
            if (ModelState.IsValid)
            {
                try
                {
                    Mapper map = new Mapper(_mapperConfig);
                    var dta = map.Map<tmuser>(model);

                    _users.Update(dta);
                    response = new ResponseModel()
                    {
                        id = model.id,
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

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dta = _users.Login(model.email, model.password);
                //user tidak ada..
                if (dta == null)
                {
                    ViewData["ErrorMsg"] = "Invalid User Name or Password. Please Check";
                    return View(model);
                }
                var usersession = new UserListModel()
                {
                    email = dta.email,
                    id = dta.id,
                    name = dta.name,
                    position = dta.position,
                    logintime = DateTime.Now
                };
                var token = myGlobal.createToken();
                Session["CurrentUser"] = usersession;
                //setting coockies

                HttpCookie cook = new HttpCookie("usertoken");
                cook.Value = token;
                cook.Expires = DateTime.Now.AddHours(1);
                Response.Cookies.Add(cook);

                Session["usertoken"] = token;

                //current Menu
                var myRole = _roleService.getUserRoleMenu(usersession.id).ToList();
                Session["myRole"] = myRole;

                if (_users.updateUserToken(dta.id, token))
                {
                    var rol = _role.FindByID(dta.roleid ?? Guid.NewGuid());

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, token, DateTime.Now, DateTime.Now.AddMinutes(30), false, rol.name);
                    string eticket = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, eticket);
                    Response.Cookies.Add(cookie);


                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ErrorMsg"] = "Invalid User Name or Password. Please Check";
                    return View(model);
                }
                //update user token.
            }
            else
            {
                ViewData["ErrorMsg"] = "Invalid User Name or Password. Please Check";
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            var usr = _users.getByToken(myGlobal.usertoken);
            _users.updateUserToken(usr.id, null);

            Response.Cookies.Clear();
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        public ActionResult ChangePassword(Guid? id)
        {
            if (id == null)
            {
                id = myGlobal.currentUser.id;
            }

            var dta = _users.FindById(id.Value);
            UserChangePasswordModel model = new UserChangePasswordModel();
            model.id = id.Value;
            model.UserName = dta.name;
            return PartialView("_changePassword", model);
        }

        [HttpPost]
        public JsonResult ChangePassword(UserChangePasswordModel dta)
        {
            ResponseModel response = null;
            if (!ModelState.IsValid)
            {
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = "Invalid Data",
                    isSuccess = false
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            //check old password
            var ops = _users.FindById(dta.id).password;
            if (mylib2.md5Encript.EncriptPassword(dta.oldpassword) == ops)
            {
                var hasil = _users.changePassword(dta.id, dta.password);
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = "Password is Changed",
                    isSuccess = true
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }

             response = new ResponseModel()
            {
                id = Guid.Empty,
                Messages = "Invalid User or Password",
                isSuccess = false
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Assignment(Guid? id)
        {
            //parameter pencarian
            List<SelectListItem> parameters = new List<SelectListItem>();
            parameters.Add(new SelectListItem()
            {
                Text = "Part No",
                Value = "partno",
                Selected = true
            });
            parameters.Add(new SelectListItem()
            {
                Text = "Name",
                Value = "name"
            });

            parameters.Add(new SelectListItem()
            {
                Text = "Distributor",
                Value = "distributor"
            });

            ViewData["parameters"] = parameters;


            //khusus untuk role collector
            var kolektor = _role.GetAll().Where(x => x.name == userRole.COLLECTOR).FirstOrDefault();
            var users = _users.GetAll().Where(x => x.roleid == kolektor.id);
            List<SelectListItem> collectors = null;
            if (id != null)
            {
                collectors = (from u in users
                                  select new SelectListItem()
                                  {
                                      Value = u.id.ToString(),
                                      Text = u.name,
                                      Selected = u.id == id
                                  }).ToList();
            }
            else
            {
                collectors = (from u in users
                                  select new SelectListItem()
                                  {
                                      Value = u.id.ToString(),
                                      Text = u.name,
                                  }).ToList();
            }
            ViewData["collectors"] = collectors;
            return View();
        }

        /// <summary>
        /// Assignment material user
        /// </summary>
        /// <param name="id">id user</param>
        /// <param name="ids">id material</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Assign(Guid id, Guid[] ids)
        {
            //check data yang ada dalam usermaterial, bandignkan denga ids
            var lids = ids.ToList();
            var rst = _materialServices.AssignUserMaterial(id, lids);
            var msg = "Assign is success";
            if(!rst)
                msg = "Assign is failes";

            ResponseModel response = new ResponseModel()
            {
                id = id,
                Messages = msg,
                isSuccess = rst
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [Route("User/Role")]
        public ActionResult Role()
        {
            var role = _role.GetAll().ToList();
            Mapper map = new Mapper(_mapperConfig);
            var model = map.Map<List<RoleModel>>(role);
            return View(model);
        }

        public ActionResult RoleDetail(Guid id)
        {
            var role = _role.FindByID(id);
            Mapper map = new Mapper(_mapperConfig);
            var model = map.Map<RoleModel>(role);

            var detail = _role.GetDetail(id);
            model.detail = detail;
            return View(model);
        }

        public ActionResult SaveRoles(Guid id, Guid[] detail)
        {
            ResponseModel response;
            try
            {
                var olddetail = _role.GetDetailByRoleId(id).ToArray(); //Old Data
                var rolf = _functionRepository.GetAll();

                var newdetail = (from d in detail
                                 select new tdrole()
                                 {
                                     id = Guid.NewGuid(),
                                     functionid = d,
                                     roleid = id,
                                 }).ToArray();

                _role.SaveMultiple(olddetail, newdetail);
                response = new ResponseModel()
                {
                    id = Guid.Empty,
                    Messages = "Role is Saved",
                    isSuccess = true
                };
            return Json(response, JsonRequestBehavior.AllowGet);
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
    }
}