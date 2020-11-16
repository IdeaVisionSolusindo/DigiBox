using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace digibox.services.Repositories
{
    public class OpnameDetailRepository : BaseClass, IOpnameDetailRepository
    {

        public OpnameDetailRepository(dbdigiboxEntities db):base(db)
        {

        }
        public tdopnam Create(tdopnam param)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(tdopnam param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdopnam> Find(Expression<Func<tdopnam, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tdopnam FindById(Guid id)
        {
            return db.tdopnams.Find(id);
        }

        public IQueryable<tdopnam> GetAll()
        {
            return db.tdopnams.Where(x => x.isdeleted == false);
        }

        public IQueryable<tdopnam> GetByOpnameID(Guid opnameid)
        {
            return db.tdopnams.Where(x => x.opnamid == opnameid && x.isdeleted == false);
        }

        public void saveMultiple(tdopnam[] prior, tdopnam[] current)
        {
            //Data yang sama;
            var samedata = (from n in current
                            join o in prior on new { n.id, n.amount } equals new { o.id, o.amount }
                            select o).ToArray<tdopnam>();
            //data yang berubah.
            var changedData = prior.Except(samedata).ToList();
            foreach(var itm in changedData)
            {
                tdopnam opd = db.tdopnams.Find(itm.id);
                opd.amount = itm.amount;
            }

            //data yang hilang
            samedata = (from o in prior
                        join n in current on new {o.id} equals new {n.id}
                        select o).ToArray<tdopnam>();
            var removedData = prior.Except(samedata).ToList();

            foreach (var itm in removedData)
            {
                tdopnam opd = db.tdopnams.Find(itm.id);
                opd.isdeleted = true;
            }

            db.SaveChanges();

        }

        public bool Update(tdopnam param)
        {
            throw new NotImplementedException();
        }
    }
}
