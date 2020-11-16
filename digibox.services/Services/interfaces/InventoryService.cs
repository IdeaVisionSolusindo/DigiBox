using digibox.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services.interfaces
{
    public interface IInventoryService
    {
        IQueryable<ttinventory> GetInventoryByMaterial(Guid materialid);
    }
}
