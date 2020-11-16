using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IDistributorRepository: ICrudRepository<tmdistributor>, IGetRepository<tmdistributor>
    {
        IQueryable<tmdistributor> GetDistributorByCollector(Guid collectorid);
        string CrateMultiple(tmdistributor[] distributor);
    }
}
