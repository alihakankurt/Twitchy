namespace Twitchy.Core;

public interface IConfiguration
{
    string Username { get; }
    string Password { get; }
    string Channel { get; }
}
