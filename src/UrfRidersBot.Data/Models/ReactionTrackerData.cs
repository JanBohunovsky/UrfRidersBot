﻿namespace UrfRidersBot.Data
{
    public class ReactionTrackerData
    {
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }

        public ReactionTrackerData(ulong messageId, ulong userId)
        {
            MessageId = messageId;
            UserId = userId;
        }
    }
}