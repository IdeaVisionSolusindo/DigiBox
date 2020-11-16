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
    public interface IRequestRepository : ICrudRepository<ttrequest>, IGetRepository<ttrequest>
    {
        IQueryable<ttrequest> GetByUser(Guid collectorID);
        bool PostRequest(Guid requestid, string status);
        IQueryable<ttrequest> GetByAdmin();
        bool Approve(ttrequest request, List<RequestDetailReceiveModel> detail , List<tdoutgoing> outgoingitems);
    }
}
