using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace digibox.services.Repositories
{
    public class RoleRepository : BaseClass, IRoleRepository
    {

        public RoleRepository(dbdigiboxEntities transaction) : base(transaction)
        {

        }

        public IQueryable<tmrole> Find(Expression<Func<tmrole, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmrole FindByID(Guid id)
        {
            return db.tmroles.Find(id);
        }

        public IQueryable<tmrole> GetAll()
        {
            return db.tmroles.Where(x => x.isdeleted == false);
        }

        public List<RoleDetailModel> GetDetail(Guid id)
        {
            string sql = $@"select f.id as functionid, f.description as Name, ISNULL(d.id,0x0) as id, ISNULL(d.roleid,0x0) as roleid  from tsfunction f left join tdrole d on d.functionid = f.id and d.roleid = '{id}'";
            var dta = db.Database.SqlQuery<RoleDetailModel>(sql).ToList();
            foreach(var itm in dta)
            {
                itm.isinList = itm.id != Guid.Empty;
            }
            return dta.ToList();
        }

        public IQueryable<tdrole> GetDetailByRoleId(Guid id)
        {
            return db.tdroles.Where(x => x.roleid == id && x.isdeleted == false);
        }

        public bool SaveMultiple(tdrole[] oldrole, tdrole[] newrole)
        {
            //remove same data.
            var tnewrole = from o in oldrole
                           join n in newrole on o.functionid equals n.functionid
                           select n;
            var xnewrole =newrole.Except(tnewrole).ToArray();

            //add
            foreach(var itm in xnewrole)
            {
                tdrole role = new tdrole()
                {
                    id = Guid.NewGuid(),
                    functionid = itm.functionid,
                    roleid = itm.roleid,
                    isdeleted = false
                };
                db.tdroles.Add(role);
            }

            tnewrole = from o in oldrole
                           join n in newrole on o.functionid equals n.functionid
                           select o;
            //delete 
            xnewrole = oldrole.Except(tnewrole).ToArray();
            foreach (var itm in xnewrole)
            {
                tdrole role = db.tdroles.Find(itm.id);
                role.isdeleted = true;
                role.deletedby = itm.deletedby;
            }

            db.SaveChanges();

            return true;
        }
    }
}
