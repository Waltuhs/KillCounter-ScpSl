using CommandSystem;
using Exiled.API.Features;
using LiteDB;
using System;
using System.Linq;
using static KillCounter.Plugin;

namespace KillCounter
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class LeaderboardCommand : ParentCommand
    {
        public LeaderboardCommand() => LoadGeneratedCommands();

        public override string Command { get; } = "leaderboard";
        public override string[] Aliases { get; } = new string[] { "lb" };
        public override string Description { get; } = "Shows the leaderboard for various metrics.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new LeaderboardKillsCommand());
            RegisterCommand(new LeaderboardDeathsCommand());
            RegisterCommand(new LeaderboardScpKillsCommand());
            RegisterCommand(new LeaderboardKillsAsScpCommand());
            RegisterCommand(new LeaderboardKDCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "\nPlease enter a valid subcommand:";

            foreach (ICommand command in AllCommands)
            {
                response += $"\n\n<color=blue><b>- {command.Command} {string.Join(", ", command.Aliases)}</b></color> \n<color=white>{command.Description}</color>";
            }
            return false;
        }
    }

    public class LeaderboardKillsCommand : ICommand
    {
        public string Command { get; } = Plugin.Instance.Translation.killssubcmdname;
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = Plugin.Instance.Translation.killssubcmddesc;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var killsLeaderboard = killCollection.FindAll()
                    .Where(kc => !kc.DNT)
                    .OrderByDescending(kc => kc.Kills)
                    .Take(10)
                    .ToList();

                string leaderboard = string.Join("\n", killsLeaderboard.Select((kc, i) =>
                    Plugin.Instance.Translation.LeaderboardKillsEntry
                        .Replace("%rank%", (i + 1).ToString())
                        .Replace("%player%", kc.PlayerName)
                        .Replace("%kills%", kc.Kills.ToString())));

                response = Plugin.Instance.Translation.LeaderboardKillsHeader.Replace("%n%", "\n") + leaderboard;
            }
            return true;
        }
    }

    public class LeaderboardDeathsCommand : ICommand
    {
        public string Command { get; } = Plugin.Instance.Translation.deathssubcmdname;
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = Plugin.Instance.Translation.deathssubcmddesc;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var deathCollection = db.GetCollection<DeathCount>("death_counts");

                var deathsLeaderboard = deathCollection.FindAll()
                    .Where(dc => !dc.DNT)
                    .OrderByDescending(dc => dc.Deaths)
                    .Take(10)
                    .ToList();

                response = Plugin.Instance.Translation.LeaderboardDeathsHeader.Replace("%n%", "\n") +
                           string.Join("\n", deathsLeaderboard.Select((dc, i) =>
                               Plugin.Instance.Translation.LeaderboardDeathsEntry
                                   .Replace("%rank%", (i + 1).ToString())
                                   .Replace("%player%", dc.PlayerName)
                                   .Replace("%deaths%", dc.Deaths.ToString())));
            }
            return true;
        }
    }

    public class LeaderboardScpKillsCommand : ICommand
    {
        public string Command { get; } = Plugin.Instance.Translation.ScpKillssubcmdname;
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = Plugin.Instance.Translation.ScpKillssubcmddesc;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var killsLeaderboard = killCollection.FindAll()
                    .Where(kc => !kc.DNT)
                    .OrderByDescending(kc => kc.KilledScps)
                    .Take(10)
                    .ToList();

                response = Plugin.Instance.Translation.LeaderboardScpKillsHeader.Replace("%n%", "\n") +
                           string.Join("\n", killsLeaderboard.Select((kc, i) =>
                               Plugin.Instance.Translation.LeaderboardScpKillsEntry
                                   .Replace("%rank%", (i + 1).ToString())
                                   .Replace("%player%", kc.PlayerName)
                                   .Replace("%scpkills%", kc.KilledScps.ToString())));
            }
            return true;
        }
    }

    public class LeaderboardKillsAsScpCommand : ICommand
    {
        public string Command { get; } = Plugin.Instance.Translation.KillsAsScpsubcmdname;
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = Plugin.Instance.Translation.KillsAsScpsubcmddesc;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var killsLeaderboard = killCollection.FindAll()
                    .Where(kc => !kc.DNT)
                    .OrderByDescending(kc => kc.ScpKills)
                    .Take(10)
                    .ToList();

                response = Plugin.Instance.Translation.LeaderboardKillsAsScpHeader.Replace("%n%", "\n") +
                           string.Join("\n", killsLeaderboard.Select((kc, i) =>
                               Plugin.Instance.Translation.LeaderboardKillsAsScpEntry
                                   .Replace("%rank%", (i + 1).ToString())
                                   .Replace("%player%", kc.PlayerName)
                                   .Replace("%killsasscp%", kc.ScpKills.ToString())));
            }
            return true;
        }
    }

    public class LeaderboardKDCommand : ICommand
    {
        public string Command { get; } = Plugin.Instance.Translation.KDsubcmdname;
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = Plugin.Instance.Translation.KDsubcmddesc;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var killsLeaderboard = killCollection.FindAll()
                    .Where(kc => !kc.DNT && !string.IsNullOrWhiteSpace(kc.PlayerName))
                    .OrderByDescending(kc => kc.KD)
                    .Take(10)
                    .ToList();

                response = Plugin.Instance.Translation.LeaderboardKDHeader.Replace("%n%", "\n") +
                           string.Join("\n", killsLeaderboard.Select((kc, i) =>
                               Plugin.Instance.Translation.LeaderboardKDEntry
                                   .Replace("%rank%", (i + 1).ToString())
                                   .Replace("%player%", kc.PlayerName)
                                   .Replace("%kd%", kc.KD.ToString())));
            }
            return true;
        }
    }
}

