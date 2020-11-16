using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IRequestDetailRepository : ICrudRepository<tdrequest>, IGetRepository<tdrequest>
    {
         bool CreateMultiple(tdrequest[] data);
         IQueryable<tdrequest> GetByRequestId(Guid requestId);
        bool UpdateMultiple(tdrequest[] olddata, tdrequest[] data);
    }
}
