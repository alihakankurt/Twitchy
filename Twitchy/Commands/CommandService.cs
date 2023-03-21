using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitchy.Core;

namespace Twitchy.Commands;

public sealed class CommandService
{
    private Bot _bot;
    private string _prefix;
    private List<Command> _commands;

    public Bot Bot => _bot;
    public string Prefix => _prefix;

    public CommandService(Bot bot, string prefix)
    {
        _bot = bot;
        _prefix = prefix;
        _commands = new List<Command>();
    }

    public bool AddCommand(Command command)
    {
        if (_commands.Any(c => c.Name == command.Name))
            return false;

        _commands.Add(command);
        return true;
    }

    public async Task TryExecuteAsync(Message message)
    {
        if (!message.Content.StartsWith(_prefix))
            return;

        string[] splitted = message.Content.Split(' ');
        string commandName = splitted[0][_prefix.Length..];

        Command? command = _commands.FirstOrDefault(c => c.Name == commandName);
        if (command is null)
            return;

        string[] args = splitted.Length > 1 ? splitted[1..] : Array.Empty<string>();
        await command.ExecuteAsync(new CommandContext(_bot, message, args));
    }
}
