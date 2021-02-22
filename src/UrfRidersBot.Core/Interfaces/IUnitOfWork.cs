﻿using System;
using System.Threading.Tasks;

namespace UrfRidersBot.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IAutoVoiceSettingsRepository AutoVoiceSettings { get; }
        Task CompleteAsync();
    }
}