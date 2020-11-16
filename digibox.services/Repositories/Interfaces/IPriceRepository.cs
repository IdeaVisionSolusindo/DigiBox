using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IPriceRepository: ICrudRepository<ttprice>, IGetRepository<ttprice>
    {
        bool ProposePrice(Guid priceid, string status);
        bool ApprovePrice(Guid id, Guid approvedby, string status);
        bool RejectPrice(Guid id, string reason, Guid rejectedby, string status);
    }
}
