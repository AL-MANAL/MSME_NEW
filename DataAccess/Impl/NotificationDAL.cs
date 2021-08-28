using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DataAccess.Contract;
using DataAccess.Models;
using HA.HALoG5BWService.DatabaseAccess.Impl;
using MySql.Data.MySqlClient;

namespace DataAccess.Impl
{
    public class NotificationDAL : GenericRepository<NotificationModel>, INotificationDAL
    {
        private readonly string _connectionString;

        #region Constructor

        public NotificationDAL() : base(Constants.DbConnectionString)
        {
            this._connectionString = Constants.DbConnectionString;
        }
        #endregion

        //#region DB methods

        public List<NotificationModel> GetAllNotificationsForUser(int userId)
        {
            object parameters = new
            {
                pUserId = userId
            };
            var transaction = base.GetAll(parameters, "get_all_notifications_for_user_id");

            return transaction;
        }

        public int InsertNotification(NotificationModel notification)
        {
            object parameters = new
            {
                pFromUserId = notification.FromUserId,
                pFromUserType = notification.FromUserType,
                pToUserId = notification.ToUserId,
                pToUserType = notification.ToUserType,
                pType = notification.Type,
                pUrl = notification.Url,
                pTitle = notification.Title,
                pAttachmentRelativeUrl = notification.AttachmentRelativeUrl,
                pContent = notification.Content,
                pIsMarkedAsRead = notification.IsMarkedAsRead,
                pCreatedDate = notification.CreatedDate,
                pCreatedBy = notification.CreatedBy
            };

            return (int)base.Insert(parameters, "insert_user_notification");
        }
        //#endregion
    }
}
