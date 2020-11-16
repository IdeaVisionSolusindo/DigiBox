using digibox.data;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class MessageRepository:BaseClass, IMessageRepository
    {
        public MessageRepository(dbdigiboxEntities transaction):base(transaction)
        {

        }

        public ttmessage Create(ttmessage param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.ttmessages.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var param = db.ttmessages.Find(id);
                param.deletedat = DateTime.Now;
                param.deletedby = "WILL BE CHANGED";
                param.isdeleted = true;
                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }

        public bool Delete(ttmessage param)
        {
            return Delete(param.id);
        }

        public bool DismissedMessage(Guid id)
        {
            try
            {
                var param = db.ttmessages.Find(id);
                param.updatedat = DateTime.Now;
                param.updatedby = "WILL BE CHANGED";
                param.isdismissed = true;
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ttmessage FindById(Guid id)
        {
            return db.ttmessages.Find(id);
        }

        public IQueryable<ttmessage> GetMessagesByReceiver(Guid id)
        {
            return db.ttmessages.Where(x => x.isdeleted == false && x.toid == id);
        }

        public IQueryable<ttmessage> GetMessagesByReceiver(Guid id, bool isDismissed)
        {
            return db.ttmessages.Where(x => x.isdeleted == false && x.toid == id && x.isdismissed==false);
        }

        /// <summary>
        /// Sending Message Notification To Role..
        /// </summary>
        /// <param name="from">Sender User ID </param>
        /// <param name="roleid">Receiver Role ID</param>
        /// <returns></returns>
        public bool SendMessageToRole(Guid from, Guid roleid, string message)
        {
            try
            {
                var receiver = db.tmusers.Where(x => x.roleid == roleid).ToList();
                var sender = db.tmusers.Find(from);
                foreach (var itm in receiver)
                {
                    ttmessage msg = new ttmessage()
                    {
                        id=Guid.NewGuid(),
                        createdby = sender.name,
                        createdat = DateTime.Now,
                        dateposted = DateTime.Now,
                        fromid = from,
                        isdismissed = false,
                        toid = itm.id,
                        messages = message,
                        isdeleted =false
                    };
                    db.ttmessages.Add(msg);
                }
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }

        }

        public bool SendMessageToUser(Guid from, Guid toid, string message)
        {
            try
            {
                var ufrom = db.tmusers.Find(from);
                ttmessage msg = new ttmessage()
                {
                    id=Guid.NewGuid(),
                    createdby = ufrom.name,
                    createdat = DateTime.Now,
                    dateposted = DateTime.Now,
                    fromid = from,
                    isdismissed = false,
                    toid = toid,
                    messages = message,
                    isdeleted = false
                };
                db.ttmessages.Add(msg);
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool Update(ttmessage param)
        {
            throw new NotImplementedException();
        }


    }

}
