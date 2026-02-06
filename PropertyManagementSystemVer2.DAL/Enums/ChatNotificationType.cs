using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.DAL.Enums
{
    public enum ChatNotificationType
    {
        NewMessage = 1,         // Tin nhắn mới
        MessageRead = 2,        // Tin nhắn đã được đọc
        UserTyping = 3,         // User đang gõ
        UserOnline = 4,         // User online
        UserOffline = 5,        // User offline
        MessageEdited = 6,      // Tin nhắn đã sửa
        MessageDeleted = 7      // Tin nhắn đã xóa
    }
}
