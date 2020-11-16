using digibox.data;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IMessageRepository:ICrudRepository<ttmessage>
    {
        IQueryable<ttmessage> GetMessagesByReceiver(Guid id);
        IQueryable<ttmessage> GetMessagesByReceiver(Guid id,bool isDismissed);
        bool DismissedMessage(Guid id);
        bool SendMessageToRole(Guid from, Guid roleid, string message);
        bool SendMessageToUser(Guid from, Guid toid, string message);

    }
}
