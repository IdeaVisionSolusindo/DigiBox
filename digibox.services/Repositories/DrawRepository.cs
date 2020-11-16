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
    public class DrawRepository:BaseClass, IDrawRepository
    {
        public DrawRepository(dbdigiboxEntities entities):base(entities)
        {

        }

        public ttdraw Create(ttdraw param)
        {
            param.id = Guid.NewGuid();
            param.no = String.Format("{0:yyyy.MM.dd}",DateTime.Now);
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttdraws.Add(param);
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

        public bool Delete(ttdraw param)
        {
            var dta = db.ttdraws.Find(param.id);
            dta.isdeleted = true;
            dta.deletedat = DateTime.Now;
            dta.deletedby = param.deletedby;
            db.SaveChanges();
            return true;
        }

        public IQueryable<ttdraw> Find(Expression<Func<ttdraw, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttdraw FindById(Guid id)
        {
            return db.ttdraws.Find(id);
        }

        public IQueryable<ttdraw> GetAll()
        {
            return db.ttdraws.Where(x => x.isdeleted == false).OrderByDescending(x => x.createdat);
        }

        public IQueryable<ttdraw> GetByDrawer(Guid drawerid)
        {
            return db.ttdraws.Where(x => x.isdeleted == false && x.drawerid == drawerid).OrderByDescending(x=>x.createdat);
        }

        public IQueryable<ttdraw> GetByAdmin()
        {
            return db.ttdraws.Where(x => x.isdeleted == false).OrderByDescending(x=>x.createdat);
        }

        public bool PostReplenish(Guid replenishid, string status)
        {
            try
            {
                var dta = db.ttdraws.Find(replenishid);
                dta.status = status;
                dta.drawdate = DateTime.Now;

                //sending notification to admin..

                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;   
            }
        }

        public bool Received(ttdraw draw, List<tddraw> detail)
        {
            try
            {

                db.ttdraws.Add(draw);

                //Update child first;
                foreach(var itm in detail)
                {
                    db.tddraws.Add(itm);

                    ttinventory inventory = new ttinventory()
                    {
                        id = Guid.NewGuid(),
                        inout = -1,
                        isdeleted = false,
                        amount = itm.receiveamount ?? 0,
                        materialid = itm.materialid,
                        createdat = DateTime.Now,
                        createdby = draw.updatedby,
                        dreplenishid = itm.id, 
                        rfidcode = itm.rfidcode 
                    };
                    db.ttinventories.Add(inventory);

                    inventory = db.ttinventories.Where(x => x.rfidcode == itm.rfidcode && x.inout == 1).FirstOrDefault();
                    var iv = db.ttinventories.Find(inventory.id);
                    iv.replstock = iv.replstock - itm.receiveamount;

                    //masukin ke current stock di material
                    tmmaterial material = db.tmmaterials.Find(itm.materialid);
                    var totalstock = material.currentstock - itm.receiveamount;
                    material.currentstock = totalstock;
                    db.SaveChanges();

                    //update inventory base
                }
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool Update(ttdraw param)
        {
            try
            {
                var dta = db.ttdraws.Find(param.id);
                dta.drawdate = param.drawdate;
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
