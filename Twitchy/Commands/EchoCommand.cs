using System.Threading.Tasks;

namespace Twitchy.Commands;

public sealed class EchoCommand : Command
{
    public EchoCommand() : base("echo")
    {
    }

    protected override async Task ExecuteAsync()
    {
        await RespondAsync($"{string.Join(' ', Context.Args)}");
    }
}
