using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.Contract
{
    public interface INotificationDAL
    {
        List<NotificationModel> GetAllNotificationsForUser(int userId);
        int InsertNotification(NotificationModel notification);
    }
}
