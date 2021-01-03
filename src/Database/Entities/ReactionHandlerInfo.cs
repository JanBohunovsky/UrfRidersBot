using System.ComponentModel.DataAnnotations;

namespace UrfRidersBot
{
    public class ReactionHandlerInfo
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