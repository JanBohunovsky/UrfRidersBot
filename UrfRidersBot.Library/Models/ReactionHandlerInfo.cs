﻿using System.ComponentModel.DataAnnotations;

namespace UrfRidersBot.Library
{
    internal class ReactionHandlerInfo
    {
        [Key]
        public ulong MessageId { get; set; }

        public string TypeName { get; set; }

        public ReactionHandlerInfo(ulong messageId, string typeName)
        {
            MessageId = messageId;
            TypeName = typeName;
        }
    }
}