using Twitchy.Core;

namespace Twitchy.Commands;

public sealed class CommandContext
{
    public Bot Bot { get; }
    public string Channel { get; }
    public string Username { get; }
    public string Content { get; }
    public Message Message { get; }
    public string[] Args { get; }

    public CommandContext(Bot bot, Message message, string[] args)
    {
        Bot = bot;
        Channel = message.Channel;
        Username = message.Username;
        Content = message.Content;
        Message = message;
        Args = args;
    }
}
