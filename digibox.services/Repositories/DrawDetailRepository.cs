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
    public class DrawDetailRepository : BaseClass, IDrawDetailRepository
    {

        public DrawDetailRepository(dbdigiboxEntities entities):base(entities)
        {

        }
        public tddraw Create(tddraw param)
        {
            throw new NotImplementedException();
        }

        public bool CreateMultiple(tddraw[] data)
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
                    db.tddraws.Add(itm);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateMultiple(tddraw[] olddata, tddraw[] data)
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
                    var old = db.tddraws.Where(x => x.materialid == itm.materialid && x.isdeleted==false).FirstOrDefault();
                    if(old!=null)
                    {
                        var prc = db.tddraws.Find(old.id);
                        prc.amount = itm.amount;
                    }
                    else
                    {
                        itm.createdat = DateTime.Now;
                        itm.isdeleted = false;
                        db.tddraws.Add(itm);
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
                    var old = db.tddraws.Where(x => x.materialid == itm.materialid && x.isdeleted == false).FirstOrDefault();
                    if (old != null)
                    {
                        var prc = db.tddraws.Find(old.id);
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

        public bool Delete(tddraw param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tddraw> Find(Expression<Func<tddraw, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tddraw FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tddraw> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<tddraw> GetByDrawId(Guid drawId)
        {
            return db.tddraws.Where(x => x.isdeleted == false && x.drawid == drawId);
        }

        public bool Update(tddraw param)
        {
            throw new NotImplementedException();
        }
    }
}
