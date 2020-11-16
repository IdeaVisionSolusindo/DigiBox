using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class UserRepository : BaseClass, IUserRepository
    {

        public UserRepository(dbdigiboxEntities transaction) : base(transaction)
        {

        }

        public bool changePassword(Guid id, string password)
        {
            try
            {
                string pwd = mylib2.md5Encript.EncriptPassword(password);
                var dta = db.tmusers.Find(id);
                dta.password = pwd;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public tmuser Create(tmuser param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = "WILL BE CHANGED";
            param.isdeleted = false;
            param.isdeletable = true;
            db.tmusers.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.tmusers.Find(id);
                dt.isdeleted = true;
                dt.deletedat = DateTime.Now;
                dt.deletedby = "WILL BE CHANGED";
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(tmuser param)
        {
            return Delete(param.id);
        }

        public IQueryable<tmuser> Find(Expression<Func<tmuser, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmuser FindById(Guid id)
        {
            return db.tmusers.Find(id);
        }

        public IQueryable<tmuser> GetAll()
        {
            return db.tmusers.Where(x => x.isdeleted == false);
        }


        public IQueryable<tmuser> getByRole(Guid id, Guid role)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tmuser> getByRole(string role)
        {
            var rl = db.tmroles.Where(x => x.name == role).FirstOrDefault();

            return db.tmusers.Where(x => x.isdeleted == false && x.roleid == rl.id);
        }

        public tmuser getByToken(string token)
        {
            var detail = db.tdusers.Where(x => x.isdeleted == false && x.token == token).FirstOrDefault();
            return db.tmusers.Where(x => x.isdeleted == false && x.id == detail.userid).FirstOrDefault();
        }

        public IQueryable<UserListModel> GetUsers()
        {
            var usr = db.tmusers.Where(x => x.isdeleted == false && x.isdeletable==true);
            var rol = db.tmroles.Where(x => x.isdeleted == false);

            var mdl = (from u in usr
                       join r in rol on u.roleid equals r.id
                       select new UserListModel()
                       {
                           email = u.email,
                           id = u.id,
                           name = u.name,
                           position = u.position,
                           role = r.name,
                       });
            return mdl;

        }

        public tmuser Login(string email, string passsword)
        {
            try
            {
                string pwd = mylib2.md5Encript.EncriptPassword(passsword);
                return db.tmusers.Where(x => x.isdeleted == false && x.email == email && x.password == pwd).FirstOrDefault();
            }catch(Exception ee)
            {
                return null;
            }
        }


        public bool Update(tmuser param)
        {
            try
            {
                var dta = db.tmusers.Find(param.id);
                dta.costcenterid = param.costcenterid;
                dta.email = param.email;
                dta.isldap = param.isldap;
                dta.name = param.name;
                dta.position = param.position;
                dta.roleid = param.roleid;
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool updateUserToken(Guid id, string token)
        {
            try
            {
                var dta = db.tmusers.Find(id);
                //insert into detail
                tduser usr = new tduser()
                {
                    id = Guid.NewGuid(),
                    isdeleted = false,
                    userid = id,
                    timelogin = DateTime.Now,
                    token = token,
                    createdat = DateTime.Now,
                    createdby = dta.name
                };
                db.tdusers.Add(usr);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
