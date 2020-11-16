using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class OpnamRepository : BaseClass, IOpnamRepository
    {

        public OpnamRepository(dbdigiboxEntities db):base(db)
        {

        }

        public string CrateMultiple(ttopnam opnam, tdopnam[] opnamdetail)
        {
            try
            {
                db.ttopnams.Add(opnam);
                foreach (var itm in opnamdetail)
                {
                    db.tdopnams.Add(itm);
                }
                db.SaveChanges();
                return "";
            }catch(Exception e)
            {
                return e.Message;
            }
        }

        public ttopnam Create(ttopnam param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttopnams.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            var dta = db.ttopnams.Find(id);
            dta.isdeleted = true;
            db.SaveChanges();
            return true;
        }

        public bool Delete(ttopnam param)
        {
            try
            {
                var dta = db.ttopnams.Find(param.id);
                dta.isdeleted = true;
                dta.deletedby = param.deletedby;
                dta.deletedat = DateTime.Now;
                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public IQueryable<ttopnam> Find(Expression<Func<ttopnam, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttopnam FindById(Guid id)
        {
            return db.ttopnams.Find(id);
        }

        public IQueryable<ttopnam> GetAll()
        {
            return db.ttopnams.Where(x=>x.isdeleted==false);
        }

        public IQueryable<OpnamListModel> openBySuperAdmin()
        {
            var dta = (from o in db.ttopnams
                       join u in db.tmusers on o.checkedby equals u.id
                       join a in db.tmattributes on o.opnamtype equals a.id
                       where o.isdeleted==false
                       select new OpnamListModel() { 
                       CheckeByName = u.name,
                       checkedby = o.checkedby, 
                       id = o.id,
                       opnamdate = o.opnamdate,
                       OpnameTypeValue = a.attributevalue,
                       opnamtype = o.opnamtype,
                       status   = o.status
                       });
            return dta;
        }

        public IQueryable<OpnamListModel> openByAdmin(Guid adminid)
        {
            var dta = (from o in db.ttopnams
                       join u in db.tmusers on o.checkedby equals u.id
                       join a in db.tmattributes on o.opnamtype equals a.id
                       where o.checkedby == adminid
                       select new OpnamListModel()
                       {
                           CheckeByName = u.name,
                           checkedby = o.checkedby,
                           id = o.id,
                           opnamdate = o.opnamdate,
                           OpnameTypeValue = a.attributevalue,
                           opnamtype = o.opnamtype,
                           status = o.status
                       });
            return dta;
        }

        public bool postOpname(Guid id, string status)
        {
            try
            {
                var dta = db.ttopnams.Find(id);
                var ddta = db.tdopnams.Where(x => x.isdeleted == false && x.opnamid == id);
                foreach (var item in ddta)
                {
                    ttinventory iv = new ttinventory()
                    {
                        amount = item.amount ?? 0,
                        createdat = DateTime.Now,
                        dreplenishid = item.id,
                        id = Guid.NewGuid(),
                        inout = Convert.ToInt16(item.inout),
                        isdeleted = false,
                        rfidcode = item.rfidcode,
                        materialid = item.materialid,
                        replstock = item.amount,
                    };
                    db.ttinventories.Add(iv);
                    
                    tmmaterial material = db.tmmaterials.Find(item.materialid);
                    decimal currentstock = (material.currentstock??0) + (item.inout??1)*(item.amount??0);
                    material.currentstock = currentstock;

                    //update material keluar untuk rfid masuk, di table inventory
                    ttinventory ivn = db.ttinventories.Where(x => x.rfidcode == item.rfidcode && x.inout == 1 && x.replstock > 0).FirstOrDefault();
                    iv = db.ttinventories.Find(ivn.id);
                    var replstock = iv.replstock - item.amount;
                    iv.replstock = replstock;
                }

                //masukin ke data master current stock


                //masukin data ke detail opname
                dta.status = status;
                db.SaveChanges();

                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool Update(ttopnam param)
        {
            throw new NotImplementedException();
        }
    }
}
