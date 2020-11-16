using ftz.services.Repositories.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace digibox.apps.Modules
{
    public class ewsEmail
    {

        private ExchangeService _exService;

        private String _sMailUser;
        private String _sMailPassword;
        private EmailMessage _oMainMail;
        private string _sSubject;
        private string _sBody;
        String _sSMTP;


        public ewsEmail(ISettingRepository set)
        {
            _sMailUser = set.getBySettingID("PRP01").value;
            _sMailPassword = set.getBySettingID("PRP04").value;
            _sSMTP = set.getBySettingID("PRP02").value;

            _exService = new ExchangeService(ExchangeVersion.Exchange2010);
            _exService.TraceEnabled = true;
            _exService.TraceFlags = TraceFlags.All;
            _exService.Credentials = new WebCredentials(_sMailUser, _sMailPassword);
            _exService.Url = new Uri(_sSMTP);
            _oMainMail = new EmailMessage(_exService);
        }

        public void AddToAddress(string emailAddr)
        {
            if (emailAddr != null)
            {
                _oMainMail.ToRecipients.Add(emailAddr);
            }
        }


        public void ClearToAddress()
        {
            _oMainMail.ToRecipients.Clear();
        }


        public void addAttachment(string sfilename)
        {
            try
            {
                if (sfilename != null)
                    _oMainMail.Attachments.AddFileAttachment(sfilename);
            }
            catch (Exception ex)
            {
            }
        }

        public void setMailFrom(string emailAddr)
        {
            _oMainMail.From = new EmailAddress(emailAddr);
        }

        public string MailSubject
        {
            set
            {
                _sSubject = value;
            }
            get
            {
                return _sSubject;
            }
        }
        public string MailBody
        {
            set
            {
                _sBody = value;
            }
            get
            {
                return _sBody;
            }
        }


        public string MailUser
        {
            set
            {
                _sMailUser = value;
            }
            get
            {
                return _sMailUser;
            }
        }

        public string MailPassword
        {
            set
            {
                _sMailPassword = value;
            }
            get
            {
                return _sMailPassword;
            }
        }

        public void SendMail()
        {
            _oMainMail.Subject = _sSubject;
            _sBody = _sBody + "<p/>";
            _oMainMail.Body = new MessageBody(_sBody);
            _oMainMail.Body.BodyType = BodyType.HTML;
            _oMainMail.Send();
        }
    }
}