using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UrfRiders.Modules.Covid19;
using UrfRiders.Util;

namespace UrfRiders.Modules.Settings
{
    public class ServerSettings
    {
        public static ServerData Default = new ServerData();

        public ulong GuildId { get; }

        #region Core

        public string Prefix
        {
            get => _data.Prefix;
            set
            {
                _data.Prefix = value;
                Save();
            }
        }

        #endregion

        #region Server Roles

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

        #endregion

        #region COVID-19 Module

        public ulong? Covid19Channel
        {
            get => _data.Covid19Channel;
            set
            {
                _data.Covid19Channel = value;
                Save();
            }
        }

        public Covid19Data Covid19CachedData
        {
            get => _data.Covid19CachedData;
            set
            {
                _data.Covid19CachedData = value;
                Save();
            }
        }

        #endregion

        #region Reaction Roles Module

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

        #endregion

        #region Auto Voice Module

        public List<ulong> AutoVoiceChannels
        {
            get => _data.AutoVoiceChannels;
            set
            {
                _data.AutoVoiceChannels = value;
                Save();
            }
        }

        #endregion

        #region Clash Module

        public ulong? ClashChannel
        {
            get => _data.ClashChannel;
            set
            {
                _data.ClashChannel = value;
                Save();
            }
        }

        #endregion

        #region Other

        public bool LargeCodeBlock
        {
            get => _data.LargeCodeBlock;
            set
            {
                _data.LargeCodeBlock = value;
                Save();
            }
        }

        #endregion

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