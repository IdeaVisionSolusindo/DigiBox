using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IAttachmentRepository:ICrudRepository<ttattachment>, IGetRepository<ttattachment>
    {
        IQueryable<ttattachment> getByReferenceID(Guid referenceid, string type);

        IQueryable<ttattachment> getByReferenceIDDescription(Guid referenceid, string description);
        
        IQueryable<ttattachment>  openByStep(Guid id, string step);
        IQueryable<ttattachment>  openByStep(Guid id, string[] step);

        bool DeleteByReferenceId(Guid referenceid);
    }

}
