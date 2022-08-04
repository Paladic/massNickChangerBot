using Discord;
using Discord.WebSocket;
using Serilog;
using NicknameChangerBot.Common;

namespace NicknameChangerBot.Services;

public class ClientHandler
{
    private readonly DiscordSocketClient _client;

    public ClientHandler(DiscordSocketClient client)
    {
        _client = client;
    }
    
#pragma warning disable CS1998
    public async Task InitializeAsync ( )
#pragma warning restore CS1998
#pragma warning restore 1998
    {
        _client.Ready += OnReady;
        _client.GuildMemberUpdated += GuildMemberUpdated;
    }
    
    private async Task GuildMemberUpdated(Cacheable<SocketGuildUser, ulong> arg1, SocketGuildUser arg2)
    {
        if (Extensions.ChangerActive && Extensions.NickNameToChange != "")
        {
            if (arg2.Nickname != Extensions.NickNameToChange)
            {
                await arg2.ModifyAsync(x => x.Nickname = Extensions.NickNameToChange);
                Log.Information("{User} попытался что-то сделать, меняем имя на {Nick}", arg2.Username,
                    Extensions.NickNameToChange);
            }
        }
        
    }


    private async Task OnReady()
    {
        await _client.SetGameAsync("Изменение никнеймов");
        await _client.SetStatusAsync(UserStatus.DoNotDisturb);
        Log.Information("{BotName}: Статус установлен!", _client.CurrentUser.Username);
    }
}