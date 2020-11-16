using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IReplenishRepository: ICrudRepository<ttreplenish>, IGetRepository<ttreplenish>
    {
        IQueryable<ttreplenish> GetByAdmin();
        IQueryable<ttreplenish> GetByCollector(Guid collectorID);
        bool PostReplenish(Guid replenishid, string status);
        bool Received(ttreplenish replenish, List<ReplenishDetailReceiveModel> detail);
    }
}
