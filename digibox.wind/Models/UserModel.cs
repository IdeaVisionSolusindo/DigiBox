using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.wind.Models
{
    public class UserModel
    {
        public System.Guid id { get; set; }
        public string nama { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isactive { get; set; }
        public bool isadmin { get; set; }
        public System.Guid roleid { get; set; }
        public string role { get; set; }
    }

    public class UserLoginModel
    {
        public string email { get; set; }

        public string password { get; set; }
    }
}
