using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Twitchy.Core;

public sealed class Bot
{
    public const string Ip = "irc.chat.twitch.tv";
    public const int Port = 6667;

    private readonly string _username;
    private readonly string _password;
    private readonly string _channel;
    private readonly IConfiguration _configuration;
    private readonly TcpClient _tcpClient;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private bool _connected;

    public string Username => _username;
    public string Password => _password;

    public delegate void MessageReceivedDelegate(Message message);
    public MessageReceivedDelegate? MessageReceived;

    public Bot(IConfiguration configuration)
    {
        _username = configuration.Username;
        _password = configuration.Password;
        _channel = configuration.Channel;
        _configuration = configuration;
        _tcpClient = new TcpClient();
    }

    public async Task StartAsync()
    {
        await _tcpClient.ConnectAsync(Ip, Port);
        if (!_tcpClient.Connected)
        {
            Console.WriteLine("Couldn't connect to Twitch IRC");
            return;
        }

        Stream stream = _tcpClient.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream)
        {
            AutoFlush = true,
            NewLine = Environment.NewLine,
        };

        await _writer.WriteLineAsync($"PASS {_password}");
        await _writer.WriteLineAsync($"NICK {_username.ToLower()}");
        await _writer.WriteLineAsync("CAP REQ :twitch.tv/commands twitch.tv/tags");
        _connected = true;

        await JoinAsync(_channel);
        while (_connected)
        {
            string? line = await _reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line))
                continue;
            
            string[] splitted = line.Split(' ', StringSplitOptions.TrimEntries);
            if (splitted[0] == "PING")
            {
                await _writer.WriteLineAsync($"PONG {splitted[1]}");
                Log("Ping", string.Empty);
            }
            else if (splitted.Length >= 3 && splitted[1] == "JOIN")
            {
                int exclamPosition = splitted[0].IndexOf('!');
                string username = splitted[0][1..exclamPosition];
                Log("Join", $"Username = {username}, Channel = {splitted[2][1..]}");
            }
            else if (splitted.Length >= 3 && splitted[1] == "PART")
            {
                int exclamPosition = splitted[0].IndexOf('!');
                string username = splitted[0][1..exclamPosition];
                Log("Leave", $"Username = {username}, Channel = {splitted[2][1..]}");
            }
            else if (splitted.Length >= 5 && splitted[2] == "PRIVMSG")
            {
                int startPosition = splitted[0].IndexOf(";display-name=") + 14;
                string raw = splitted[0][startPosition..];
                int semicolonPosition = raw.IndexOf(';');
                string username = raw[..semicolonPosition];

                startPosition = splitted[0].IndexOf(";id=") + 4;
                raw = splitted[0][startPosition..];
                semicolonPosition = raw.IndexOf(';');
                string messageId = raw[..semicolonPosition];

                startPosition = splitted[0].IndexOf(";user-id=") + 9;
                raw = splitted[0][startPosition..];
                semicolonPosition = raw.IndexOf(';');
                raw = raw[..semicolonPosition];
                uint userId = uint.Parse(raw);

                string channel = splitted[3][1..];

                string content = splitted[4][1..];
                if (splitted.Length > 5)
                    content = $"{content} {string.Join(' ', splitted[5..])}";

                Log("Received", $"Username = {username}, Channel = {channel}, Content = {content}");
                MessageReceived?.Invoke(new Message(messageId, userId, username, channel, content));
            }
            else
            {
                Console.WriteLine($"RAW {line}");
            }
        }
    }

    public Task StopAsync()
    {
        _connected = false;
        return Task.CompletedTask;
    }

    public async Task JoinAsync(string channel)
    {
        if (!_connected)
            return;

        await _writer!.WriteLineAsync($"JOIN #{channel.ToLower()}");
    }

    public async Task SendAsync(string channel, string text)
    {
        if (!_connected)
            return;

        if (string.IsNullOrEmpty(text))
        {
            Log("Fail", "Couldn't send message 'cause text was empty");
            return;
        }

        await _writer!.WriteLineAsync($"PRIVMSG #{channel.ToLower()} :{text}");
        Log("Sent", $"Channel = {channel}, Content = {text}");
    }

    public async Task RespondAsync(Message message, string text)
    {
        if (!_connected)
            return;

        if (string.IsNullOrEmpty(text))
        {
            Log("Fail", "Couldn't respond to message 'cause text was empty");
            return;
        }

        await _writer!.WriteLineAsync($"@reply-parent-msg-id={message.Id} PRIVMSG #{message.Channel} :{text}");
        Log("Respond", $"Channel = {message.Channel}, MessageId = {message.Id}, User = {message.Username}, Content = {text}");
    }

    private static void Log(string name, string text)
    {
        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {name}: {text}");
    }
}
