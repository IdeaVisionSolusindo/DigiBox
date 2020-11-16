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
    public interface IMaterialRepository: ICrudRepository<tmmaterial>, IGetRepository<tmmaterial>
    {
        Task<bool> CrateMultiple(tmmaterial[] param, tmmaterialsbu[] sbus);
        IQueryable<MaterialListModel> GetMaterial();
        IQueryable<MaterialListModel> GetMaterialByCollector(Guid collectorId);
        IQueryable<tmmaterial> GetMaterialByDistributor(Guid distributorid);
        IQueryable<MaterialListModel> MinimumMaterial(Guid? id);
    }
}
