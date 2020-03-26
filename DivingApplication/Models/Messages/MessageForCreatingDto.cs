﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Messages
{
    public class MessageForCreatingDto
    {
        public string MessageType { get; set; }
        public string Content { get; set; }
        public Guid BelongChatRoomId { get; set; }
    }
}
