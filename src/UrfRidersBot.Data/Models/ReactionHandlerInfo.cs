namespace UrfRidersBot.Data
{
    public class ReactionHandlerInfo
    {
        public ulong MessageId { get; set; }
        public string TypeName { get; set; }

        public ReactionHandlerInfo(ulong messageId, string typeName)
        {
            MessageId = messageId;
            TypeName = typeName;
        }
    }
}