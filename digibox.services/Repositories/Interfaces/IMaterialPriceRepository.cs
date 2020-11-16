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
    public interface IMaterialPriceRepository : ICrudRepository<tmmaterialprice>, IGetRepository<tmmaterialprice>
    {
        tmmaterialprice GetCurrentPrice(Guid materialId);
        IQueryable<tmmaterialprice> Overdue(int days, Guid? user);
    }
}
