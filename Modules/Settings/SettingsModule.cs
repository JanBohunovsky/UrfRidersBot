using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UrfRiders.Util;

namespace UrfRiders.Modules.Settings
{
    [Name("Settings")]
    [Group("settings")]
    [RequireContext(ContextType.Guild)]
    [RequireLevel(PermissionLevel.Admin)]
    public class SettingsModule : BaseModule
    {
        private const string NotFound = "Setting not found.";

        public ILogger<SettingsModule> Logger { get; set; }

        [Command]
        public async Task ListAll()
        {
            var sb = new StringBuilder();
            foreach (var (name, value) in Settings.GetValues())
            {
                sb.AppendLine($"{name}: `{PropertyValueToString(value)}`");
            }

            var embed = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle($"Current settings for {Context.Guild.Name}")
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .AddField("Settings", sb.ToString())
                .AddField("Reset", $"`{Settings.Prefix}settings reset [setting name]`")
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command]
        public async Task Detail(string propertyName)
        {
            var largeCode = Settings.LargeCodeBlock;

            // Base embed
            var embedBuilder = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle("Settings detail");

            // Find property
            var property = Settings.GetProperty(propertyName);
            if (property == null || property.CustomAttributes.Any(x => x.AttributeType == typeof(HiddenAttribute)))
            {
                await ReplyAsync(embed: embedBuilder.WithError(NotFound).Build());
                return;
            }


            // Check if property is type of Nullable
            Type realType;
            bool isNullable = false;
            if ((realType = Nullable.GetUnderlyingType(property.PropertyType)) != null)
                isNullable = true;
            else
                realType = property.PropertyType;

            var typeText = isNullable ? $"Nullable<{realType.Name}>" : property.PropertyType.Name;

            // Basic information
            embedBuilder
                .AddField("Name", property.Name.ToCode(largeCode), true)
                .AddField("Type", typeText.ToCode(largeCode), true)
                .AddField("Value",
                    PropertyValueToString(Settings.GetPropertyValue(property)).ToCode(largeCode),
                    true)
                .AddField("Update command", $"{Settings.Prefix}settings {property.Name.ToLower()} <{realType.Name}>".ToCode(largeCode));

            // Description
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            if (description != null)
                embedBuilder.AddField("Description", description.Description);

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command]
        [Priority(1)]
        public async Task Edit(string propertyName, [Remainder] string value)
        {
            // Base embed
            var embedBuilder = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle("Update settings");

            // Find property
            var property = Settings.GetProperty(propertyName);
            if (property == null || property.CustomAttributes.Any(x => x.AttributeType == typeof(HiddenAttribute)))
            {
                await ReplyAsync(embed: embedBuilder.WithError(NotFound).Build());
                return;
            }

            var readOnly = property.GetCustomAttribute<ReadOnlyAttribute>();
            if (readOnly != null && readOnly.IsReadOnly)
            {
                await ReplyAsync(embed: embedBuilder.WithError("This setting is read-only.").Build());
                return;
            }

            // Try to update value
            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            try
            {
                Settings.UpdateProperty(property, Convert.ChangeType(value, targetType));
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                await ReplyAsync(embed: embedBuilder.WithError("Invalid value.").Build());
                return;
            }
            Settings.Save();

            var newValue = PropertyValueToString(Settings.GetPropertyValue(property));
            await ReplyAsync(embed: embedBuilder.WithSuccess($"{property.Name} has been set to `{newValue}`.").Build());
        }

        [Command("reset")]
        [Priority(2)]
        public async Task Reset(string propertyName = null)
        {
            // Base embed
            var embedBuilder = new EmbedBuilder()
                .WithColor(Program.Color)
                .WithTitle("Reset settings");

            if (propertyName != null)
            {
                // Reset property
                var property = Settings.GetProperty(propertyName);
                if (property == null || property.CustomAttributes.Any(x => x.AttributeType == typeof(HiddenAttribute)))
                {
                    await ReplyAsync(embed: embedBuilder.WithError(NotFound).Build());
                    return;
                }

                var value = PropertyValueToString(Settings.ResetProperty(property));
                await ReplyAsync(embed: embedBuilder.WithSuccess($"{property.Name} has been reset to `{value}`.").Build());
            }
            else
            {
                // Reset everything
                Settings.Delete();
                await ReplyAsync(embed: embedBuilder.WithSuccess("Settings have been reset.").Build());
            }
        }

        private string PropertyValueToString(object obj)
        {
            return obj?.ToString() ?? "NULL";
        }
    }
}