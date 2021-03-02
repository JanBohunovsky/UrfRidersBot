﻿using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IColorRoleRepository
    {
        Task<DiscordRole?> GetByMemberAsync(DiscordMember member);
        Task AddAsync(DiscordRole role, DiscordMember member);
        void Remove(DiscordRole role, DiscordMember member);
    }
}