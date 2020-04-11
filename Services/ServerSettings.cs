using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discord.WebSocket;
using LiteDB;
using UrfRiders.Attributes;
using UrfRiders.Data;

namespace UrfRiders.Services
{
    public class ServerSettings
    {
        public static ServerData Default = new ServerData();

        public ulong GuildId { get; }

        public string Prefix
        {
            get => _data.Prefix;
            set
            {
                _data.Prefix = value;
                Save();
            }
        }

        public ulong? AdminRole
        {
            get => _data.AdminRole;
            set
            {
                _data.AdminRole = value;
                Save();
            }
        }

        public ulong? ModeratorRole
        {
            get => _data.ModeratorRole;
            set
            {
                _data.ModeratorRole = value;
                Save();
            }
        }

        public ulong? MemberRole
        {
            get => _data.MemberRole;
            set
            {
                _data.MemberRole = value;
                Save();
            }
        }

        public ulong? Covid19Channel
        {
            get => _data.Covid19Channel;
            set
            {
                _data.Covid19Channel = value;
                Save();
            }
        }

        public ulong? ReactionRolesChannel
        {
            get => _data.ReactionRolesChannel;
            set
            {
                _data.ReactionRolesChannel = value;
                Save();
            }
        }

        public ulong? ReactionRolesMessage
        {
            get => _data.ReactionRolesMessage;
            set
            {
                _data.ReactionRolesMessage = value;
                Save();
            }
        }

        public bool LargeCodeBlock
        {
            get => _data.LargeCodeBlock;
            set
            {
                _data.LargeCodeBlock = value;
                Save();
            }
        }

        public List<ulong> AutoVoiceChannels
        {
            get => _data.AutoVoiceChannels;
            set
            {
                _data.AutoVoiceChannels = value;
                Save();
            }
        }

        private readonly ILiteCollection<ServerData> _collection;
        private ServerData _data;

        public ServerSettings(ulong guildId, LiteDatabase database)
        {
            GuildId = guildId;
            _collection = database.GetCollection<ServerData>();
            _data = _collection.FindById(GuildId) ?? new ServerData();
        }

        public static IEnumerable<ServerSettings> All(DiscordSocketClient client, LiteDatabase database) =>
            client.Guilds.Select(guild => new ServerSettings(guild.Id, database));

        /// <summary>
        /// Manual save. Useful for list properties.
        /// </summary>
        public void Save() => _collection.Upsert(GuildId, _data);

        public void Delete()
        {
            _collection.Delete(GuildId);
            _data = new ServerData();
        }

        public IEnumerable<ValueTuple<string, object>> GetValues()
        {
            foreach (var propertyInfo in _data.GetType().GetProperties())
            {
                if (propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(HiddenAttribute)))
                    continue;
                
                yield return (propertyInfo.Name, propertyInfo.GetValue(_data));
            }
        }

        public PropertyInfo GetProperty(string propertyName)
        {
            foreach (var propertyInfo in _data.GetType().GetProperties())
            {
                if (string.Equals(propertyInfo.Name, propertyName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return propertyInfo;
                }
            }

            return null;
        }

        public object GetPropertyValue(PropertyInfo property) => property.GetValue(_data);

        public void UpdateProperty(PropertyInfo property, object value)
        {
            property.SetValue(_data, value);
        }
                
        public object ResetProperty(PropertyInfo property)
        {
            var value = property.GetValue(Default);
            property.SetValue(_data, value);
            Save();
            return value;
        }
    }
}