using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services
{
    public class RoleService : BaseClass, IRoleService
    {
        public RoleService(dbdigiboxEntities transaction) : base(transaction)
        {

        }

        public IQueryable<tmrole> getRoleByName(string role)
        {
            return db.tmroles.Where(x => x.isdeleted == false && x.name == role);
        }

        public string getRoleByToken(string token)
        {
            var role = (from u in db.tmusers
                        join d in db.tdusers on u.id equals d.userid
                        join r in db.tmroles on u.roleid equals r.id
                        where d.token == token
                        select r.name).FirstOrDefault();
            return role;
        }

        public string getUserRole(Guid userId)
        {
            var role = (from u in db.tmusers
                        join r in db.tmroles on u.roleid equals r.id
                        where u.id == userId
                        select r.name).FirstOrDefault();
            return role;

        }

        public IQueryable<RoleFunctionModel> getUserRoleMenu(Guid userId)
        {
            var role = (from u in db.tmusers
                        join r in db.tmroles on u.roleid equals r.id
                        join d in db.tdroles on r.id equals d.roleid
                        join f in db.tsfunctions on d.functionid equals f.id
                        where u.id == userId
                        select new RoleFunctionModel()
                        {
                            action = f.action,
                            controller = f.controller
                        });
            return role;
        }
    }
}
