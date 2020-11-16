﻿using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IReplenishDetailRepository: ICrudRepository<tdreplenish>, IGetRepository<tdreplenish>
    {
         bool CreateMultiple(tdreplenish[] data);
         IQueryable<tdreplenish> GetByReplenishId(Guid replanishId);
        bool UpdateMultiple(tdreplenish[] olddata, tdreplenish[] data);
    }
}
