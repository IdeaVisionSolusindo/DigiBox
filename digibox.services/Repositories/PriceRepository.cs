using Autofac.Features.GeneratedFactories;
using digibox.data;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class PriceRepository: BaseClass, IPriceRepository
    {
        public PriceRepository(dbdigiboxEntities entities):base(entities)
        {

        }

        public bool ApprovePrice(Guid id,Guid approvedby, string status)
        {
            try
            {

                //cannot approve price check

                var dta = db.ttprices.Find(id);
                dta.status = status;
                dta.updatedat = DateTime.Now;
                dta.approvedby = approvedby;
                db.SaveChanges();

                //activate new material
                var newprice = db.tdprices.Where(x => x.isdeleted == false && x.priceid == id);
                foreach (var itm in newprice)
                {
                    var materialprice = new tmmaterialprice()
                    {
                        id=Guid.NewGuid(),
                        approvedby = approvedby,
                        createdat = DateTime.Now,
                        datestart = dta.startdate??DateTime.Now,
                        dateend = dta.enddate,
                        isactive = false, //status active saat startdate = now
                        price = itm.newprice ?? 0M,
                        materialid = itm.materialid,
                        status = status,
                        isdeleted=false
                    };
                    db.tmmaterialprices.Add(materialprice);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RejectPrice(Guid id, string reason, Guid rejectedby, string status)
        {
            try
            {
                var dta = db.ttprices.Find(id);
                dta.status = status;
                dta.rejectedreason = reason;
                dta.updatedat = DateTime.Now;
                dta.approvedby = rejectedby;
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public ttprice Create(ttprice param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttprices.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dta = db.ttprices.Find(id);
                dta.isdeleted = true;
                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool Delete(ttprice param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ttprice> Find(Expression<Func<ttprice, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttprice FindById(Guid id)
        {
            return db.ttprices.Find(id);

        }

        public IQueryable<ttprice> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool ProposePrice(Guid priceid, string status)
        {
            try
            {
                //check apakah ada dalam price detail yang masih berlaku.. pada tanggal tersebut, kalau ada batal.


                var dta = db.ttprices.Find(priceid);
                dta.status = status;
                dta.ddate = DateTime.Now;
                dta.updatedat = DateTime.Now;

                var detail = db.tdprices.Where(x => x.priceid == priceid);
                var materialprice = db.tmmaterialprices.Where(x => x.isdeleted == true && x.isactive == true);

                var checkdata = from mp in materialprice
                                join d in detail on mp.materialid equals d.materialid
                                where ((mp.datestart >= dta.startdate) && (mp.datestart <= dta.startdate))||((mp.datestart>=dta.enddate) && (mp.dateend<=dta.enddate))
                                select mp;

                if (checkdata.Count() > 0)
                {
                    return false;
                }

                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool Update(ttprice param)
        {
            try
            {
                var dta = db.ttprices.Find(param.id);
                dta.updatedby = param.updatedby;
                dta.updatedat = DateTime.Now;
                dta.no = param.no;
                dta.ddate = param.ddate;
                dta.startdate = param.startdate;
                dta.enddate = param.enddate;
                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

       
    }
}
