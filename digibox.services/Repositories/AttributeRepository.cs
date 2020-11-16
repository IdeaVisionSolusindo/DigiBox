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
    public class AttributeRepository : BaseClass, IAttributeRepository
    {
        public AttributeRepository(dbdigiboxEntities db):base(db)
        {

        }
        public tmattribute Create(tmattribute param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.tmattributes.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.tmattributes.Find(id);
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

        public bool Delete(tmattribute param)
        {
            return Delete(param.id);
        }

        public IQueryable<tmattribute> Find(Expression<Func<tmattribute, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmattribute FindById(Guid id)
        {
            return db.tmattributes.Find(id);
        }

        public IQueryable<tmattribute> GetAll()
        {
            return db.tmattributes.Where(x => x.isdeleted == false);
        }

        public IQueryable<tmattribute> GetByName(string name)
        {
            return db.tmattributes.Where(x => x.isdeleted == false && x.attributename == name);
        }

        public bool Update(tmattribute param)
        {
            try
            {
                tmattribute distributor = db.tmattributes.Find(param.id);
                distributor.attributename = param.attributename;
                distributor.attributevalue = param.attributevalue;
                distributor.description = param.description;
                distributor.updatedat = DateTime.Now;
                distributor.updatedby = param.updatedby;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
