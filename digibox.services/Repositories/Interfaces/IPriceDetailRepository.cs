using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IPriceDetailRepository: ICrudRepository<tdprice>, IGetRepository<tdprice>
    {
        IQueryable<tdprice> GetByPriceId(Guid priceid);
    }
}
