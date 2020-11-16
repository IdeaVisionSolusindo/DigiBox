using digibox.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services.interfaces
{
    public interface INotificationService
    {
        NotificationModel PriceCollectorDueNotification(Guid Userid);
        NotificationModel PriceDueNotification();

    }
}
