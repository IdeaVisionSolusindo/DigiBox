using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.Common;

namespace digibox.apps.Modules
{
    public static class myGlobal
    {

        public const int PageSize = 10;

        public static string mybase
        {
            get
            {
                return ConfigurationManager.AppSettings["_base"].ToString();
            }
        }

        public static string createToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }

        public static string usertoken
        {
            get
            {

                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("usertoken");
                if (cookie != null) {
                    return cookie.Value;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                HttpCookie cook = new HttpCookie("usertoken");
                cook.Value = value;
                cook.Expires = DateTime.Now.AddHours(1);
                HttpContext.Current.Request.Cookies.Add(cook);
            }
        }

        public static string UserName
        {
            get
            {
                return HttpContext.Current.Session["UserName"] as string;
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }
        }
        public static string UserRole
        {
            get
            {
                return HttpContext.Current.Session["UserRole"] as string;
            }
            set
            {
                HttpContext.Current.Session["UserRole"] = value;
            }
        }

        public static UserListModel currentUser
        {
            get
            {
                try
                {
                    DateTime dlogin = DateTime.MinValue;
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        var ulogin = HttpContext.Current.Session["CurrentUser"] as UserListModel;
                        dlogin = ulogin.logintime;
                    }

                    using (dbdigiboxEntities db = new dbdigiboxEntities())
                    {
                        IUserRepository user = new UserRepository(db);
                        var dta = user.getByToken(myGlobal.usertoken);
                        var usersession = new UserListModel()
                        {
                            email = dta.email,
                            id = dta.id,
                            name = dta.name,
                            position = dta.position,
                            logintime = dlogin
                        };

                        if (HttpContext.Current.Session["myRole"] == null)
                        {
                            IRoleService role = new RoleService(db);
                            var myRole = role.getUserRoleMenu(usersession.id).ToList();
                            HttpContext.Current.Session["myRole"] = myRole;
                        }

                        return usersession;
                    }
                }catch(Exception ex)
                {
                    return null;
                }

            }
        }

        public static string qrcode(string rfidCode)
        {
            var barcode = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 150,
                    Width = 150,
                    Margin = 1
                }
            };
            var result = barcode.Write(rfidCode);
            using (var ms = new MemoryStream())
            {
                result.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                string imageBase64Data = Convert.ToBase64String(ms.ToArray());
                string qrcodeimage = string.Format("data:image/png;base64,{0}", imageBase64Data);
                return qrcodeimage;
            }
        }

        public static string barcode(string rfidCode)
        {
            var barcode = new BarcodeWriter()
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 150,
                    Margin = 1
                }
            };
            var result = barcode.Write(rfidCode);

            using (var ms = new MemoryStream())
            {
                result.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                string imageBase64Data = Convert.ToBase64String(ms.ToArray());
                string barcodeimage = string.Format("data:image/png;base64,{0}", imageBase64Data);
                return barcodeimage;
            }

        }


        //public static Guid UserID = new Guid("1D39D415-118F-4EAF-B0A4-3C44F10AE0F9"); //untuk user

        public static Guid UserID = new Guid("7A379281-F6FD-404F-839B-F4E90EF50514");//untuk finance

        public static byte[] convertFile(Stream filestream)
        {
            byte[] data = null;

            using (var reader = new BinaryReader(filestream))
            {
                data = reader.ReadBytes((int)filestream.Length);
            }

            return data;
        }

        public static void SaveFileStream(String path, Stream stream)
        {
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);
            fileStream.Dispose();
        }

        public static bool isVisibleMenu(string controller, string action)
        {
            var myRole = HttpContext.Current.Session["myRole"] as List<RoleFunctionModel>;
            if(myRole==null)
            {
                return false;
            }
            var role = myRole.Where(x => x.controller == controller && x.action == action).FirstOrDefault();
            return role != null;
        }

        public static bool isVisibleMenuHeader(string controller)
        {
            var myRole = HttpContext.Current.Session["myRole"] as List<RoleFunctionModel>;
            if (myRole == null)
            {
                return false;
            }
            
            var role = myRole.Where(x => x.controller == controller);
            return role.Count()!=0;
        }

        /*
        public static void sendEmail(string toAddress, string body, string subject)
        {
            using (dbftzEntities db = new dbftzEntities())
            {
                ISettingRepository setting = new SettingRepository(db);

                EmailService emailService = new EmailService(setting);
                emailService.AddToAddress(toAddress);
                emailService.MailBody = body;
                emailService.MailSubject = subject;
                emailService.SendMail();
            }
        }*/
    }

    public abstract class Status
    {
        public static string DRAFT = "DRAFT";
        public static string APPROVED = "APPROVED";
        public static string REJECTED = "REJECTED";
        public static string RECEIVED = "RECEIVED";
    }

    public sealed class PriceStatus:Status
    {
        public static string POSTEDBYCOLLECTOR = "POSTED BY COLLECTOR";
        public static string POSTEDBYADMIN = "POSTED BY ADMIN";
    }

    public sealed class ReplenishStatus:Status
    {
        public static string POSTEDBYCOLLECTOR = "POSTED BY COLLECTOR";
    }

    public sealed class RequestStatus : Status
    {
        public static string POSTEDBYUSER = "POSTED BY USER";
    }


    public sealed class ActiveInActiveStatus
    {
        public static string ACTIVE = "ACTIVE";
        public static string INACTIVE= "INACTIVE";
    }

    public sealed class OpnameType
    {
        public static string INITIALSTOCK = "INITIAL STOCK";
        public static string STOCKOPNAME = "STOCK OPNAME";
    }
    public sealed class AttachmentType
    {
        public const string PRICEPROPOSAL = "PRICEPROPOSAL";
        public const string PRICEAPPROVAL = "PRICEAPPROVAL";
    }

    
    public sealed class orderStatus
    {
        public static string OPENED = "OPENED";
        public static string CART = "CART";
        public static string ORDERED = "ORDERED";
        public static string APPROVED = "APPROVED";
        public static string REJECTED = "REJECTED";
        public static string CANCELED = "CANCELED";
        public static string SHIPPED = "SHIPPED";
        public static string RECEIVED = "RECEIVED";
        public static string CLOSED = "CLOSED";
    }

    public sealed class editType
    {
        public static string NONE = "NONE";
        public static string ADD = "ADD";
        public static string EDIT = "EDIT";
        public static string DELETE = "DELETE";
    }

    public static class FNGlobal
    {
        public static String LabelColor(string param)
        {
            if (param == orderStatus.ORDERED)
            {
                return "label label-success";
            }

            if (param == orderStatus.CANCELED)
            {
                return "label label-warning";
            }

            if (param == orderStatus.REJECTED)
            {
                return "text-danger text-sm";
            }


            if (param == orderStatus.SHIPPED)
            {
                return "label label-danger";
            }

            if (param == orderStatus.RECEIVED)
            {
                return "label label-success";
            }



            //Price Status
            if (param == PriceStatus.DRAFT)
            {
                return "text-default text-sm";
            }

            if (param == PriceStatus.POSTEDBYCOLLECTOR)
            {
                return "text-info text-sm";
            }

            if (param == PriceStatus.POSTEDBYADMIN)
            {
                return "text-success text-sm";
            }

            if (param == PriceStatus.POSTEDBYADMIN)
            {
                return "text-danger text-sm";
            }

            if (param == PriceStatus.APPROVED)
            {
                return "text-primary text-sm";
            }

            //acive in active
            if (param == ActiveInActiveStatus.ACTIVE)
            {
                return "badge bg-success";
            }
            if (param == ActiveInActiveStatus.INACTIVE)
            {
                return "badge bg-info";
            }

            return "";
        }

        public static string[] splitText(string param, char separator)
        {
            return param.Split(separator);
        }
    }

    public sealed class userRole
    {
        public const string SUPERADMIN = "SUPERADMIN";
        public const string MANAGEMENT = "MANAGEMENT";
        public const string MANAGER = "MANAGER";
        public const string ADMIN = "ADMIN";
        public const string COLLECTOR = "COLLECTOR";
        public const string USER = "USER";
    }
}