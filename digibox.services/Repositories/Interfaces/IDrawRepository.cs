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
    public interface IDrawRepository: ICrudRepository<ttdraw>, IGetRepository<ttdraw>
    {
        IQueryable<ttdraw> GetByAdmin();
        IQueryable<ttdraw> GetByDrawer(Guid drawerID);
        bool Received(ttdraw draw, List<tddraw> detail);
    }
}
