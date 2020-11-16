using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services
{
    public class MessageService : BaseClass, IMessageService
    {
        private readonly IMessageRepository _message;
        public MessageService(IMessageRepository message, dbdigiboxEntities entities) : base(entities)
        {
            _message = message;
        }
        public void sendMessage(Guid fromid, Guid toid, Guid bolid, string message)
        {
            var msg = new ttmessage()
            {
                fromid = fromid,
                toid = toid,
                messages = message,
                dateposted = DateTime.Now,
                isdismissed = false
            };
            _message.Create(msg);
        }

        public IQueryable<MessageListModel> getMessages(Guid userid)
        {
            var dta = (from mgs in db.ttmessages
                       join usr in db.tmusers on mgs.fromid equals usr.id
                       join usrt in db.tmusers on mgs.toid equals usrt.id
                       where mgs.isdeleted == false && mgs.isdismissed == false && mgs.toid == userid
                       orderby mgs.createdat descending
                       select new MessageListModel()
                       {
                           fromName = usr.name,
                           id = mgs.id,
                           isdismissed = mgs.isdismissed,
                           messages = mgs.messages,
                           toName = usrt.name,
                           dateposted = mgs.dateposted
                       });
            return dta;
        }

        public bool removeOldMessages(int days)
        {
            try
            {
                var last5Date = DateTime.Now.AddDays(-days);
                string sql = $"UPDATE ttmessages set isdeleted=1 where dateposted <= '{last5Date.ToString("yyyy-MM-dd")}'";
                db.Database.ExecuteSqlCommand(sql);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }

}
