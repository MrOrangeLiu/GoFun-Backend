using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.ManyToManyEntities.UserChatRoom;

namespace DivingApplication.Helpers.Extensions
{
    public static class UserChatRoomRoleExtension
    {
        /// <summary>
        /// Three values will be return 0, 1, -1
        /// </summary>
        public static int HasHigherRole(this UserChatRoomRole source, UserChatRoomRole anotherRole)
        {
            if (source == anotherRole) return 0;

            if (source == UserChatRoomRole.Owner) return 1;
            if (anotherRole == UserChatRoomRole.Owner) return -1;

            if (source == UserChatRoomRole.Admin) return 1;
            if (anotherRole == UserChatRoomRole.Admin) return -1;

            // Normal User don't have right
            return 0;
        }
    }
}
