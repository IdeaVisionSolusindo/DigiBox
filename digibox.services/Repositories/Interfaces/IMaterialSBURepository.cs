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
    public interface IMaterialSBURepository : ICrudRepository<tmmaterialsbu>, IGetRepository<tmmaterialsbu>
    {
        bool CreateMultiple(Guid materialid, Guid[] sbuids);
        bool DeleteMultiple(Guid[] ids);
        List<MaterialSBUModel> GetTransposeAllMaterialSBU();
        IQueryable<tmmaterialsbu> GetMaterialSBUByMaterialID(Guid materialid);
    }
}
