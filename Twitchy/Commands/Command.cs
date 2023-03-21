using System.Threading.Tasks;

namespace Twitchy.Commands;

public abstract class Command
{
    private string _name;
    private CommandContext _context;

    public string Name => _name;
    protected CommandContext Context => _context;

    #pragma warning disable CS8618
    public Command(string name)
    {
        _name = name;
    }
    #pragma warning restore CS8618

    public Task ExecuteAsync(CommandContext context)
    {
        _context = context;
        return ExecuteAsync();
    }

    protected abstract Task ExecuteAsync();

    protected Task SendAsync(string text)
    {
        return _context.Bot.SendAsync(_context.Channel, text);
    }

    protected Task RespondAsync(string text)
    {
        return _context.Bot.RespondAsync(_context.Message, text);
    }
}
