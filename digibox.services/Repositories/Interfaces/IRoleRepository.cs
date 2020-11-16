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
    public interface IRoleRepository : IGetRepository<tmrole>
    {
        tmrole FindByID(Guid id);
        List<RoleDetailModel> GetDetail(Guid id);
        IQueryable<tdrole> GetDetailByRoleId(Guid id);
        bool SaveMultiple(tdrole[] oldrole, tdrole[] newrole);
    }
}

