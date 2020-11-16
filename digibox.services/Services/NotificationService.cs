using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace digibox.services.Services
{
    public class NotificationService:BaseClass, INotificationService
    {
        public NotificationService(dbdigiboxEntities entities):base(entities)
        {

        }

        public NotificationModel PriceCollectorDueNotification(Guid Userid)
        {
            //a weeks before due
            var sql = $"SELECT count(*) from tmmaterialprice pc inner join tmmaterialuser u on pc.materialid = u.materialid where pc.dateend >= GETDATE() - 7 and u.userid = '{Userid}' ";

            var jlh = db.Database.SqlQuery<int>(sql).FirstOrDefault();
            var messages = $"There {jlh} Material Price will be expired";
            NotificationModel model = new NotificationModel()
            {
                amount = jlh,
                messsages = messages
            };
            return model;
        }

        public NotificationModel PriceDueNotification()
        {
            var sql = $"SELECT count(*) from tmmaterialprice pc inner join tmmaterialuser u on pc.materialid = u.materialid where pc.dateend >= GETDATE() - 7";

            var jlh = db.Database.SqlQuery<int>(sql).FirstOrDefault();
            var messages = $"There {jlh} Material Price will be expired ";
            NotificationModel model = new NotificationModel()
            {
                amount = jlh,
                messsages = messages
            };
            return model;
        }
    }
}
