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
    public class PriceDetailRepository : BaseClass, IPriceDetailRepository
    {
        public PriceDetailRepository(dbdigiboxEntities entities):base(entities)
        {

        }
        public tdprice Create(tdprice param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.tdprices.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(tdprice param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdprice> Find(Expression<Func<tdprice, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tdprice FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdprice> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<tdprice> GetByPriceId(Guid priceid)
        {
            return db.tdprices.Where(x => x.isdeleted == false && x.priceid == priceid);
        }

        public bool Update(tdprice param)
        {
            throw new NotImplementedException();
        }
    }
}
