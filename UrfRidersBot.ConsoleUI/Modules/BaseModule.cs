using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using UrfRidersBot.Library;

namespace UrfRidersBot.ConsoleUI.Modules
{
    public class BaseModule : ModuleBase<SocketCommandContext>
    {
        public IEmbedService Embed { get; set; } = null!;

        // protected IUserMessage? ActiveMessage;
        //
        // protected async Task<IUserMessage> ReplyEmbed(Embed embed)
        // {
        //     if (ActiveMessage == null)
        //     {
        //         ActiveMessage = await ReplyAsync(embed: embed);
        //     }
        //     else
        //     {
        //         await ActiveMessage.ModifyAsync(x => x.Embed = embed);
        //     }
        //
        //     return ActiveMessage;
        // }
        //
        // protected async Task<IUserMessage> ReplyEmbed(string? description, string? title = null)
        // {
        //     return await ReplyEmbed(Embed.Basic(description, title).Build());
        // }
        //
        // protected async Task<IUserMessage> ReplySuccess(string? description, string title = "Success")
        // {
        //     return await ReplyEmbed(Embed.Success(description, title).Build());
        // }
        //
        // protected async Task<IUserMessage> ReplyError(string? description, string title = "Error")
        // {
        //     return await ReplyEmbed(Embed.Error(description, title).Build());
        // }
    }
}