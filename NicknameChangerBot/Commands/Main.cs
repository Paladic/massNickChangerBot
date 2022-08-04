using Discord;
using Discord.Interactions;
using NicknameChangerBot.Common;
using Serilog;

namespace NicknameChangerBot.Commands;

public class Main : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("никнейм-изменить-всем", "меняет всем пользователям никнейм на указанный", runMode: RunMode.Async)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [EnabledInDm(false)]
    public async Task ChangeNickAsync([Summary("никнейм", "на что будем менять")]string nick)
    {
        await DeferAsync();
        if (nick.Length > 32)
        {
            await Context.Interaction.ModifyOriginalResponseAsync(x =>
                x.Content = "Длина никнейма слишком большая. Не более 32 символов.");
        }
        
        var users = Context.Guild.Users.ToList();
        Extensions.NickNameToChange = nick;

        await Context.Interaction.ModifyOriginalResponseAsync(x => x.Content = $"Начинаем! У нас на подходе: {users.Count} чл.");
        for (var index = 0; index < users.Count; index++)
        {
            var user = users[index];
            if (user.Nickname != nick)
            {
                if (Extensions.StopChanger)
                {
                    Log.Information("==============Изменение никнеймов остановлено=========");
                    Extensions.StopChanger = false;
                    return;
                }
                try
                {
                    await user.ModifyAsync(x => x.Nickname = nick);
                    Log.Information("{UserUsername} стал {Nick} [{Index}/{UsersCount}]", user.Username, nick, index, users.Count);
                }
                catch
                {
                    Log.Information(">>> {UserUsername} НЕ стал {Nick} [{Index}/{UsersCount}]", user.Username, nick, index, users.Count);
                }
            }
        }
        Log.Information("Мы изменили никнейм {UsersCount} чл", users.Count);
    }

    [SlashCommand("никнейм-менять-всегда", "Менять никнейм пользователя при попытке изменить его",
        runMode: RunMode.Async)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [EnabledInDm(false)]
    public async Task ChangeEvertime([Summary("действие", "да - меняем, нет - не меняем"), 
                                      Choice("да", "true"), 
                                      Choice("нет", "false")]string changer)
    {
        Extensions.ChangerActive = Convert.ToBoolean(changer);
        if (Convert.ToBoolean(changer))
        {
            await Context.Interaction.RespondAsync(
                "готово, теперь всем пользователям при попытки обойти систему будет возвращаться ник из массовой выдачи");
        }
        else
        {
            await Context.Interaction.RespondAsync(
                "готово, теперь всем пользователям при попытки обойти систему **НЕ будет** возвращаться ник из массовой выдачи");
        }
    }

    [SlashCommand("остановить-изменение", "Останавливает изменение никнеймов")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [EnabledInDm(false)]
    public async Task StopChange()
    {
        Extensions.StopChanger = true;
        await Context.Interaction.RespondAsync("Готово выдача остановлена");
    }
    
}