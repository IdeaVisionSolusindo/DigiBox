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
    public interface IOpnamRepository:ICrudRepository<ttopnam>, IGetRepository<ttopnam>
    {
        IQueryable<OpnamListModel> openBySuperAdmin();
        string CrateMultiple(ttopnam opnam, tdopnam[] opnamdetail);
        bool postOpname(Guid id, string status);
        IQueryable<OpnamListModel> openByAdmin(Guid adminid);
    }
}
