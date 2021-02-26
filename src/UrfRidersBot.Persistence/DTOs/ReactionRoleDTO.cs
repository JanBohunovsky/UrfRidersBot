namespace UrfRidersBot.Persistence.DTOs
{
    public class ReactionRoleDTO
    {
        public ulong MessageId { get; private set; }
        public string Emoji { get; private set; }
        public ulong RoleId { get; private set; }

        public ReactionRoleDTO(ulong messageId, string emoji, ulong roleId)
        {
            MessageId = messageId;
            Emoji = emoji;
            RoleId = roleId;
        }
    }
}