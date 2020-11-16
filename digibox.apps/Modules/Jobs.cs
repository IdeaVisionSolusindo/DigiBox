using digibox.data;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace digibox.apps.Modules
{

    public class ScheduledJobRegistry : Registry
    {
        public ScheduledJobRegistry()
        {
            RunSchedule();
        }

        private void RunSchedule()
        {
            Schedule(() => {
                using(dbdigiboxEntities db = new dbdigiboxEntities())
                {
                    ActivatePrice(db);
                    //RemoveOldMessage(db);
                   // BoLNotifaction(db);
                }
                //RunFunction();
            }).ToRunNow().AndEvery(1).Minutes();
        }

        private void ActivatePrice(dbdigiboxEntities db )
        {

            //check based on tanggal.
            var dta = db.tmmaterialprices.Where(x => x.isactive == false && x.isdeleted == false && x.status == PriceStatus.APPROVED && x.datestart<=DateTime.Now && x.dateend>=DateTime.Now).ToList();
            if (dta.Count != 0)
            {
                var ids = (from d in dta
                           select d.id).ToList();
                var sids = $"'{ string.Join("','", ids)}'";
                string sql = $"UPDATE tmmaterialprice set isactive=1 where id in ({sids})";
                db.Database.ExecuteSqlCommand(sql);

                //non activekan 
                ids = (from d in dta
                           select d.materialid).ToList();
                var matids = $"'{ string.Join("','", ids)}'";

                sql = $"UPDATE tmmaterialprice set isactive=0 where (id not in ({sids})) and (materialid in ({matids}))";
                db.Database.ExecuteSqlCommand(sql);
            }

            //inactivekan material lama.

            return;
        }


        //Remove Last 10 Days notification.

/*
        private void RemoveOldMessage(dbdigiboxEntities db)
        {
            IMessageService msg = new MessageService(new MessageRepository(db), db);
            msg.removeOldMessages(7);
        }*/

        /*

        private void BoLNotifaction(dbdigiboxEntities db)
        {
            INotificationService notification = new NotificationService(db);
            var dta = notification.getPostByUserBOL();
            var emailTo = (from d in dta
                           select d.toEmail).ToList();
            List<EmailMassageModel> emls = new List<EmailMassageModel>();
            foreach (var im in emailTo)
            {
                var ct = (from d in dta
                          where d.toEmail == im
                          select d).ToList();

                //getting content for each email.
                string cbod = "Mohon dilakukan proses untuk dokumen berikut:\n";
                foreach (var itm in ct)
                {
                    cbod += $"No BL\t{itm.nobl}\t{itm.tanggal}\t{itm.plants}";
                }

                emls.Add(new EmailMassageModel()
                {
                    EmailAddress = im,
                    Title = "Notification for Process ",
                    EmailBody = cbod
                });
            }

            ISettingRepository setting = new SettingRepository(db);

            EmailService emailService = new EmailService(setting);
            foreach (var itm in emls)
            {
                emailService.AddToAddress(itm.EmailAddress);
                emailService.MailBody = itm.EmailBody;
                emailService.MailSubject = itm.Title;
                emailService.SendMail();
            }

            //sending email
        }
        */
    }

}