namespace Twitchy.Core;

public sealed class Message
{
    public string Id { get; }

    public uint UserId { get; }
    public string Username { get; }
    public string Channel { get; }
    public string Content { get; }

    public Message(string id, uint userId, string username, string channel, string content)
    {
        Id = id;
        UserId = userId;
        Username = username;
        Channel = channel;
        Content = content;
    }
}
