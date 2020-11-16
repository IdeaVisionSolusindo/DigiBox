using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IDrawDetailRepository: ICrudRepository<tddraw>, IGetRepository<tddraw>
    {
         bool CreateMultiple(tddraw[] data);
         IQueryable<tddraw> GetByDrawId(Guid replanishId);
        bool UpdateMultiple(tddraw[] olddata, tddraw[] data);
    }
}
