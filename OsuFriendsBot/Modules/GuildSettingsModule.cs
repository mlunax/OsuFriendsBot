﻿using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using OsuFriendsDb.Models;
using OsuFriendsDb.Services;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [RequireContext(ContextType.Guild)]
    [Name("Server settings")]
    [Summary("Change your server settings")]
    public class GuildSettingsModule : ModuleBase<SocketCommandContext>
    {
        private readonly GuildSettingsCacheService _guildSettingsCache;
        private readonly Config _config;
        private readonly ILogger _logger;

        public GuildSettingsModule(GuildSettingsCacheService guildSettingsCache, Config config, ILogger<GuildSettingsModule> logger)
        {
            _guildSettingsCache = guildSettingsCache;
            _config = config;
            _logger = logger;
        }

        [Command("prefix")]
        [Summary("Set custom bot prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixCmd([Summary("If not specified, restores default prefix")] string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix) && prefix.Length > 32)
            {
                await ReplyAsync("Prefix can't be longer than 32 characters!"); // TODO: Use Post-Execution handler
                return;
            }
            GuildSettings settings = _guildSettingsCache.GetOrAddGuildSettings(Context.Guild.Id);
            settings.Prefix = prefix;
            _guildSettingsCache.UpsertGuildSettings(settings);
            await ReplyAsync($"Current prefix: {prefix ?? _config.Prefix}");
        }
    }
}