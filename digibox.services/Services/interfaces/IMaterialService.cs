using digibox.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services.interfaces
{
    public interface IMaterialServices
    {
        IQueryable<MaterialAssignmentModel> MaterialAssignmentList(Guid id);
        bool AssignUserMaterial(Guid id, List<Guid> guids);
        IQueryable<CurrentMaterialStatus> GetMaterialCurrentPrice();

    }
}
