using System.Threading.Tasks;
using Twitchy.Core;
using Twitchy.Commands;

namespace Twitchy;

class Program
{
    static async Task Main(string[] args)
    {
        Configuration configuration = Configuration.LoadFromFile("config.cfg");
        Bot bot = new(configuration);

        CommandService commands = new(bot, configuration.Prefix);
        commands.AddCommand(new UrlCommand("dc", configuration.DiscordUrl));
        commands.AddCommand(new UrlCommand("yt", configuration.YouTubeUrl));
        commands.AddCommand(new UrlCommand("insta", configuration.InstagramUrl));
        commands.AddCommand(new UrlCommand("lig", configuration.LeagueOfGraphsUrl));
        commands.AddCommand(new EchoCommand());

        bot.MessageReceived += async (Message message) =>
        {
            await commands.TryExecuteAsync(message);
        };

        await bot.StartAsync();
    }
}
