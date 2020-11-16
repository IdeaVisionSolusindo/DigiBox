using digibox.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services.interfaces
{
    public interface IMessageService
    {
        void sendMessage(Guid from, Guid to, Guid bolid, string message);
        IQueryable<MessageListModel> getMessages(Guid userid);
        bool removeOldMessages(int days);
    }
}
