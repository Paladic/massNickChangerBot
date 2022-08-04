using Discord;

namespace NicknameChangerBot.Common
{
    public static class Extensions
    {
        public static bool ChangerActive = false;
        public static string NickNameToChange = "";
        public static bool StopChanger = false;
        
        public static Task<Embed> CreateErrorAsync(this IMessageChannel channel,
            string description)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 100, 100))
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author
                        .WithIconUrl(
                            "https://media.discordapp.net/attachments/890682513503162420/980144116832825435/1-62835-128.png")
                        .WithName("Произошла ошибка:");
                })
                .Build();
            
            return Task.FromResult(embed);
        }
        
        public static Task<EmbedAuthorBuilder> CreateAuthorEmbed(IUser user)
        {
            

            var authorBuilder = new EmbedAuthorBuilder()
                .WithName(user.Username + "#" + user.Discriminator)
                .WithIconUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

            return Task.FromResult(authorBuilder);
            // await Extensions.createAuthorEmbed(Context.User)
        }
        
    }
}