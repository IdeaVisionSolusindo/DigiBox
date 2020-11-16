using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class UserModel
    {
        public Guid id { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string name { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Position")]
        public string position { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }
        public Nullable<bool> isldap { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        [Display(Name = "Role")]
        [Required]
        public Nullable<System.Guid> roleid { get; set; }
    }

    public class UserListModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string position { get; set; }
        public string role { get; set; }
        public DateTime logintime { get; set; }
    }

    public class UserLoginModel
    {
        [Required(ErrorMessage = "user name is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password Is required")]
        [Display(Name = "Password")]
        public string password { get; set; }
    }

    public class UserChangePasswordModel
    {
        public Guid id { get; set; }
        [Display(Name = "User Name")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Old Password")]
        [Required]
        public string oldpassword { get; set; }


        [Display(Name = "Password")]
        [Required]
        public string password { get; set; }
        [Display(Name = "Re Type Password")]
        [Required]
        public string repassword { get; set; }
    }
}
