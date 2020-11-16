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
    public class DistributorRepository : BaseClass, IDistributorRepository
    {
        public DistributorRepository(dbdigiboxEntities db):base(db)
        {
        }

        public string CrateMultiple(tmdistributor[] distributor)
        {
            try
            {
                foreach (var param in distributor)
                {
                    param.id = Guid.NewGuid();
                    param.createdat = DateTime.Now;
                    param.createdby = param.createdby;
                    param.isdeleted = false;
                    db.tmdistributors.Add(param);
                }
                db.SaveChanges();
                return "";
            }catch(Exception e)
            {
                return e.Message;
            }
        }

        public tmdistributor Create(tmdistributor param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.tmdistributors.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.tmdistributors.Find(id);
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

        public bool Delete(tmdistributor param)
        {
            return Delete(param.id);
        }

        public IQueryable<tmdistributor> Find(Expression<Func<tmdistributor, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmdistributor FindById(Guid id)
        {
            return db.tmdistributors.Find(id);
        }

        public IQueryable<tmdistributor> GetAll()
        {
            return db.tmdistributors.Where(x => x.isdeleted == false);
        }

        public IQueryable<tmdistributor> GetDistributorByCollector(Guid collectorid)
        {
            var material = db.tmmaterials.Where(x => x.isdeleted == false);
            var materialuser = db.tmmaterialusers.Where(x => x.userid == collectorid && x.isdeleted == false);
            var distributor = (from d in db.tmdistributors
                               join m in material on d.id equals m.distributor
                               join mu in materialuser on m.id equals mu.materialid
                               select d).Distinct();
            return distributor;
        }

        public bool Update(tmdistributor param)
        {
            try
            {
                tmdistributor distributor = db.tmdistributors.Find(param.id);
                distributor.name = param.name;
                distributor.address = param.address;
                distributor.telp = param.telp;
                distributor.updatedat = DateTime.Now;
                distributor.updatedby = "WILL BE CHANGED";
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
