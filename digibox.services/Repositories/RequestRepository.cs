using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class RequestRepository : BaseClass, IRequestRepository
    {
        public RequestRepository(dbdigiboxEntities entities):base(entities)
        {

        }

        public ttrequest Create(ttrequest param)
        {
            param.id = Guid.NewGuid();
            param.no = String.Format("{0:yyyy.MM.dd}",DateTime.Now);
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttrequests.Add(param);
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

        public bool Delete(ttrequest param)
        {
            var dta = db.ttrequests.Find(param.id);
            dta.isdeleted = true;
            dta.deletedat = DateTime.Now;
            dta.deletedby = param.deletedby;
            db.SaveChanges();
            return true;
        }

        public IQueryable<ttrequest> Find(Expression<Func<ttrequest, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttrequest FindById(Guid id)
        {
            return db.ttrequests.Find(id);
        }

        public IQueryable<ttrequest> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ttrequest> GetByAdmin()
        {
            return db.ttrequests.Where(x => x.isdeleted == false).OrderByDescending(x=>x.createdat);
        }

        public IQueryable<ttrequest> GetByUser(Guid collectorID)
        {
            return db.ttrequests.Where(x => x.isdeleted == false && x.userid == collectorID).OrderByDescending(x => x.createdat);
        }

        public bool PostRequest(Guid requestid, string status)
        {
            try
            {
                var dta = db.ttrequests.Find(requestid);
                dta.status = status;
                dta.requestdate = DateTime.Now;

                //sending notification to admin..

                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;   
            }
        }

        public bool Approve(ttrequest replenish, List<RequestDetailReceiveModel> detail, List<tdoutgoing> outgoings)
        {
            try
            {
                //Update child first;
                foreach(var itm in detail)
                {
                    var dta = db.tdrequests.Find(itm.requestid);
                    dta.receiveamount = itm.receiveamount;
                    dta.receivedate = DateTime.Now;

                    ttinventory inventory = new ttinventory()
                    {
                        id=Guid.NewGuid(),
                        inout = -1,
                        isdeleted = false,
                        amount = dta.receiveamount ?? 0,
                        materialid = dta.materialid,
                        createdat = DateTime.Now,
                        createdby = replenish.updatedby
                    };
                    db.ttinventories.Add(inventory);

                    //masukin ke current stock di material
                    tmmaterial material = db.tmmaterials.Find(dta.materialid);
                    var totalstock = material.currentstock + dta.receiveamount;
                    material.currentstock = totalstock;
                }

                //masukin detil data pada outgoing
                foreach(var itm in outgoings)
                {
                    itm.id = Guid.NewGuid();
                    itm.isdeleted = false;
                    db.tdoutgoings.Add(itm);
                }

                //update inventoryid where data come from. 
                foreach(var itm in outgoings)
                {
                    ttinventory iv = db.ttinventories.Find(itm.inventoryid);
                    var nstock = iv.replstock - itm.amount;
                    iv.replstock = nstock;
                }

                var rpl = db.ttrequests.Find(replenish.id);
                rpl.receiveddate = DateTime.Now;
                rpl.handedoverby = replenish.handedoverby;
                rpl.status = replenish.status;

                //masukin data ke inventory

                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool Update(ttrequest param)
        {
            try
            {
                var dta = db.ttrequests.Find(param.id);
                dta.requestdate = param.requestdate;
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
