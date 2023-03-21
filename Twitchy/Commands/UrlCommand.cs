using System.Threading.Tasks;

namespace Twitchy.Commands;

public sealed class UrlCommand : Command
{
    private readonly string _url;
    
    private string Url => _url;

    public UrlCommand(string name, string url) : base(name)
    {
        _url = url;
    }

    protected override async Task ExecuteAsync()
    {
        await RespondAsync(_url);
    }
}
