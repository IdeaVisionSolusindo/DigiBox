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
    public interface IOpnameDetailRepository:ICrudRepository<tdopnam>, IGetRepository<tdopnam>
    {
        IQueryable<tdopnam> GetByOpnameID(Guid opnameid);
        void saveMultiple(tdopnam[] prior, tdopnam[] current);
    }
}
