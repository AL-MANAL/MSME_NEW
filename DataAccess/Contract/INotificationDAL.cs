using DataAccess.Models;
using System.Collections.Generic;

namespace DataAccess.Contract
{
    public interface INotificationDAL
    {
        List<NotificationModel> GetAllNotificationsForUser(int userId);

        int InsertNotification(NotificationModel notification);
    }
}