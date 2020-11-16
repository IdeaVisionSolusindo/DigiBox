using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class ReplenishRepository:BaseClass, IReplenishRepository
    {
        public ReplenishRepository(dbdigiboxEntities entities):base(entities)
        {

        }

        public ttreplenish Create(ttreplenish param)
        {
            param.id = Guid.NewGuid();
            param.no = String.Format("{0:yyyy.MM.dd}",DateTime.Now);
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttreplenishes.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            var dta = db.tdreplenishes.Find(id);
            dta.isdeleted = true;
            dta.deletedat = DateTime.Now;
            db.SaveChanges();
            return true;
        }

        public bool Delete(ttreplenish param)
        {
            var dta = db.ttreplenishes.Find(param.id);
            dta.isdeleted = true;
            dta.deletedat = DateTime.Now;
            dta.deletedby = param.deletedby;
            db.SaveChanges();
            return true;
        }

        public IQueryable<ttreplenish> Find(Expression<Func<ttreplenish, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttreplenish FindById(Guid id)
        {
            return db.ttreplenishes.Find(id);
        }

        public IQueryable<ttreplenish> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ttreplenish> GetByCollector(Guid collectorID)
        {
            return db.ttreplenishes.Where(x => x.isdeleted == false && x.collectorid == collectorID).OrderByDescending(x=>x.createdat);
        }

        public IQueryable<ttreplenish> GetByAdmin()
        {
            return db.ttreplenishes.Where(x => x.isdeleted == false).OrderByDescending(x=>x.createdat);
        }

        public bool PostReplenish(Guid replenishid, string status)
        {
            try
            {
                var dta = db.ttreplenishes.Find(replenishid);
                dta.status = status;
                dta.indate = DateTime.Now;

                //sending notification to admin..

                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;   
            }
        }

        public bool Received(ttreplenish replenish, List<ReplenishDetailReceiveModel> detail)
        {
            try
            {
                //Update child first;
                foreach(var itm in detail)
                {
                    var dta = db.tdreplenishes.Find(itm.id);
                    dta.receiveamount = itm.receiveamount;
                    dta.receivedate = DateTime.Now;

                    ttinventory inventory = new ttinventory()
                    {
                        id=Guid.NewGuid(),
                        inout = 1,
                        isdeleted = false,
                        amount = dta.receiveamount ?? 0,
                        materialid = dta.materialid,
                        createdat = DateTime.Now,
                        createdby = replenish.updatedby,
                        dreplenishid = itm.id,
                        replstock = dta.receiveamount??0
                    };
                    db.ttinventories.Add(inventory);

                    //masukin ke current stock di material
                    tmmaterial material = db.tmmaterials.Find(dta.materialid);
                    var totalstock = material.currentstock + dta.receiveamount;
                    material.currentstock = totalstock;
                }

                var rpl = db.ttreplenishes.Find(replenish.id);
                rpl.receiveddate = DateTime.Now;
                rpl.receivedbyid = replenish.receivedbyid;
                rpl.status = replenish.status;

                //masukin data ke inventory

                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool Update(ttreplenish param)
        {
            try
            {
                var dta = db.ttreplenishes.Find(param.id);
                dta.indate = param.indate;
                dta.no = param.no;
                param.updatedat = DateTime.Now;
                param.updatedby = param.updatedby;
                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
    }
}
