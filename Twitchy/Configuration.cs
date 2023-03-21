using System;
using System.IO;
using System.Linq;
using Twitchy.Core;

namespace Twitchy;

public sealed class Configuration : IConfiguration
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Channel { get; set; }
    public string Prefix { get; set; }
    public string DiscordUrl { get; set; }
    public string YouTubeUrl { get; set; }
    public string InstagramUrl { get; set; }
    public string LeagueOfGraphsUrl { get; set; }

    #pragma warning disable CS8618
    private Configuration()
    {
    }
    #pragma warning restore CS8618

    public static Configuration LoadFromFile(string filename)
    {
        Configuration configuration = new();
        string[] lines = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, filename))
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string line in lines)
        {
            string[] splitted = line.Split('=', count: 2);
            if (splitted.Length < 2)
            {
                Console.WriteLine($"Invalid configuration option: {line}");
                Environment.Exit(1);
            }

            switch (splitted[0])
            {
                case "Username":
                    configuration.Username = splitted[1];
                    break;

                case "Password":
                    configuration.Password = splitted[1];
                    break;

                case "Channel":
                    configuration.Channel = splitted[1];
                    break;

                case "Prefix":
                    configuration.Prefix = splitted[1];
                    break;

                case "DiscordUrl":
                    configuration.DiscordUrl = splitted[1];
                    break;

                case "YouTubeUrl":
                    configuration.YouTubeUrl = splitted[1];
                    break;

                case "InstagramUrl":
                    configuration.InstagramUrl = splitted[1];
                    break;

                case "LeagueOfGraphsUrl":
                    configuration.LeagueOfGraphsUrl = splitted[1];
                    break;

                default:
                    break;
            }
        }

        return configuration;
    }
}
