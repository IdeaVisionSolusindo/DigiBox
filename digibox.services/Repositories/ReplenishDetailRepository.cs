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
    public class ReplenishDetailRepository : BaseClass, IReplenishDetailRepository
    {

        public ReplenishDetailRepository(dbdigiboxEntities entities):base(entities)
        {

        }
        public tdreplenish Create(tdreplenish param)
        {
            throw new NotImplementedException();
        }

        public bool CreateMultiple(tdreplenish[] data)
        {
            try
            {

                CultureInfo CI = new CultureInfo("id-ID");
                Calendar Cal = CI.Calendar;
                DateTime currentdate = DateTime.Now;
                int week = Cal.GetWeekOfYear(currentdate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                foreach (var itm in data)
                {
                    var material = db.tmmaterials.Find(itm.materialid);
                    itm.id = Guid.NewGuid();
                    itm.createdat = currentdate;
                    itm.isdeleted = false;
                    itm.rfidcode = itm.rfidcode;
                    db.tdreplenishes.Add(itm);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateMultiple(tdreplenish[] olddata, tdreplenish[] data)
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
                    var old = db.tdreplenishes.Where(x => x.materialid == itm.materialid && x.isdeleted==false).FirstOrDefault();
                    if(old!=null)
                    {
                        var prc = db.tdreplenishes.Find(old.id);
                        prc.amount = itm.amount;
                    }
                    else
                    {
                        itm.createdat = DateTime.Now;
                        itm.isdeleted = false;
                        db.tdreplenishes.Add(itm);
                    }
                }

                var cA = changedData.ToArray();


                //hanya ambil id aja
                samedata = (from n in data
                            join o in olddata on n.materialid equals o.materialid
                            select o).ToArray();

                //buang yang tidak dihapus
                /*var staydata = (from n in data
                            join o in olddata on new { n.materialid, n.amount } equals new { o.materialid, o.amount }
                            select o).ToList().Concat(changedData).ToList();*/
                // samedata = samedata.Concat(changedData.ToArray());
                var removedData = olddata.Except(samedata);

                foreach (var itm in removedData)
                {
                    //find existing
                    var old = db.tdreplenishes.Where(x => x.materialid == itm.materialid && x.isdeleted == false).FirstOrDefault();
                    if (old != null)
                    {
                        var prc = db.tdreplenishes.Find(old.id);
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

        public bool Delete(tdreplenish param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdreplenish> Find(Expression<Func<tdreplenish, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tdreplenish FindById(Guid id)
        {
            return db.tdreplenishes.Find(id);
        }

        public IQueryable<tdreplenish> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdreplenish> GetByReplenishId(Guid replanishId)
        {
            return db.tdreplenishes.Where(x => x.isdeleted == false && x.replenishid == replanishId);
        }

        public bool Update(tdreplenish param)
        {
            throw new NotImplementedException();
        }
    }
}
