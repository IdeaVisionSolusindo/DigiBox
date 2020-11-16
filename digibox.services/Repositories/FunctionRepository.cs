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
    public class FunctionRepository:BaseClass, IFunctionRepository
    {
        public FunctionRepository(dbdigiboxEntities db):base(db)
        {

        }

        public IQueryable<tsfunction> Find(Expression<Func<tsfunction, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tsfunction> GetAll()
        {
            return db.tsfunctions.Where(x => x.isdeleted == false);
        }
    }
}
