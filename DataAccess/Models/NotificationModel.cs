using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class NotificationModel
    {
        public long NotificationId { get; set; }
        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public int FromUserType { get; set; }
        public int ToUserType { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string AttachmentRelativeUrl { get; set; }
        public string Content { get; set; }
        public bool IsMarkedAsRead { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; }
    }
}
