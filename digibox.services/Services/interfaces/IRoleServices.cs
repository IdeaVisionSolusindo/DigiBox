using digibox.data;
using digibox.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services.interfaces
{
    public interface IRoleService
    {
        string getUserRole(Guid userId);
        string getRoleByToken(string token);

        IQueryable<tmrole> getRoleByName(string role);

        IQueryable<RoleFunctionModel> getUserRoleMenu(Guid userId);
    }
}
