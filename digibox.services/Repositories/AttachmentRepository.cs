using digibox.data;
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
    public class AttachmentRepository : BaseClass, IAttachmentRepository
    {
        public AttachmentRepository(dbdigiboxEntities transaction):base(transaction)
        {

        }
        public ttattachment Create(ttattachment param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttattachments.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.ttattachments.Find(id);
                dt.deletedat = DateTime.Now;
                dt.deletedby = "WILL BE CHANGED";
                dt.isdeleted = true;
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }

        }

        public bool Delete(ttattachment param)
        {
            return Delete(param.id);
        }

        public bool DeleteByReferenceId(Guid referenceid)
        {
            try
            {
                var dt = (from a in db.ttattachments
                          where a.referenceid == referenceid 
                          select a.id).ToList();
                foreach (var itm in dt)
                {
                    var dta = db.ttattachments.Find(itm);
                    dta.isdeleted = true;
                }
                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public IQueryable<ttattachment> Find(Expression<Func<ttattachment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttattachment FindById(Guid id)
        {
            return db.ttattachments.Find(id);
        }

        public IQueryable<ttattachment> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ttattachment> getByReferenceID(Guid referenceid, string type)
        {
            return db.ttattachments.Where(x => x.referenceid == referenceid && x.isdeleted==false && x.attachmenttype == type);
        }

        public IQueryable<ttattachment> getByReferenceIDDescription(Guid referenceid, string description)
        {
            return db.ttattachments.Where(x => x.referenceid == referenceid && x.isdeleted == false && x.description == description);
        }

        public IQueryable<ttattachment> openByStep(Guid id, string step)
        {
            return db.ttattachments.Where(x => x.referenceid == id && x.isdeleted == false && x.attachmentstep == step);
        }

        public IQueryable<ttattachment> openByStep(Guid id, string[] step)
        {
            var dta = from at in db.ttattachments
                      join st in step on at.attachmentstep equals st
                      where at.referenceid == id && at.isdeleted == false
                      select at;


            return dta;
        }

        public bool Update(ttattachment param)
        {
            throw new NotImplementedException();
        }
    }
}
