using digibox.data;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class RequestDetailRepository : BaseClass, IRequestDetailRepository
    {

        public RequestDetailRepository(dbdigiboxEntities entities):base(entities)
        {

        }
        public tdrequest Create(tdrequest param)
        {
            throw new NotImplementedException();
        }

        public bool CreateMultiple(tdrequest[] data)
        {
            try
            {
                DateTime currentdate = DateTime.Now;
                foreach (var itm in data)
                {
                    var material = db.tmmaterials.Find(itm.materialid);
                    itm.id = Guid.NewGuid();
                    itm.createdat = currentdate;
                    itm.isdeleted = false;
                    db.tdrequests.Add(itm);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateMultiple(tdrequest[] olddata, tdrequest[] data)
        {
            try
            {

                var samedata = (from n in data
                               join o in olddata on new { n.materialid, n.amount } equals new { o.materialid, o.amount }
                               select n).ToArray();

                //ambil data yang tidak sama..
                var changedData = data.Except(samedata).ToList();

                //buang yang dihapus.
                foreach (var itm in changedData)
                {
                    //find existing
                    var old = db.tdrequests.Where(x => x.materialid == itm.materialid && x.isdeleted==false).FirstOrDefault();
                    if(old!=null)
                    {
                        var prc = db.tdrequests.Find(old.id);
                        prc.amount = itm.amount;
                    }
                    else
                    {
                        itm.createdat = DateTime.Now;
                        itm.isdeleted = false;
                        db.tdrequests.Add(itm);
                    }
                }

                var cA = changedData.ToArray();


                //hanya ambil id aja
                samedata = (from n in data
                            join o in olddata on n.materialid equals o.materialid
                            select o).ToArray();

                //buang yang tidak dihapus
                
                var removedData = olddata.Except(samedata);

                foreach (var itm in removedData)
                {
                    //find existing
                    var old = db.tdrequests.Where(x => x.materialid == itm.materialid && x.isdeleted == false).FirstOrDefault();
                    if (old != null)
                    {
                        var prc = db.tdrequests.Find(old.id);
                        prc.isdeleted= true;
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(tdrequest param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdrequest> Find(Expression<Func<tdrequest, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tdrequest FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdrequest> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdrequest> GetByRequestId(Guid requestid)
        {
            return db.tdrequests.Where(x => x.isdeleted == false && x.requestid== requestid);
        }

        public bool Update(tdrequest param)
        {
            throw new NotImplementedException();
        }
    }
}
