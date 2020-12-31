using System.ComponentModel.DataAnnotations;

namespace UrfRidersBot.Library
{
    public class ReactionTrackerData
    {
        [Key]
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }

        public ReactionTrackerData(ulong messageId, ulong userId)
        {
            MessageId = messageId;
            UserId = userId;
        }
    }
}