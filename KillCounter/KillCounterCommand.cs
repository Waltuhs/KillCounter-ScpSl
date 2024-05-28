using CommandSystem;
using Exiled.API.Features;
using LiteDB;
using System;
using static KillCounter.Plugin;

namespace KillCounter
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class KillCounterCommand : ParentCommand
    {
        public KillCounterCommand() => LoadGeneratedCommands();

        public override string Command { get; } = "killcounter";
        public override string[] Aliases { get; } = new string[] { "kc" };
        public override string Description { get; } = "Edits kill counter statistics.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new SetKillsCommand());
            RegisterCommand(new SetDeathsCommand());
            RegisterCommand(new SetScpKillsCommand());
            RegisterCommand(new SetKilledScpsCommand());
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

    public class SetKillsCommand : ICommand
    {
        public string Command { get; } = "setkills";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Sets the total kills for a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: setkills <steamid> <value>";
                return false;
            }

            string steamId = arguments.At(0);
            if (!int.TryParse(arguments.At(1), out int kills))
            {
                response = "Invalid value for kills.";
                return false;
            }

            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var playerData = killCollection.FindOne(Query.EQ("PlayerId", steamId));
                if (playerData == null)
                {
                    response = "Player not found.";
                    return false;
                }

                playerData.Kills = kills;
                killCollection.Update(playerData);
                response = $"Set kills for player {playerData.PlayerName} to {kills}.";
            }
            return true;
        }
    }

    public class SetDeathsCommand : ICommand
    {
        public string Command { get; } = "setdeaths";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Sets the total deaths for a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: setdeaths <steamid> <value>";
                return false;
            }

            string steamId = arguments.At(0);
            if (!int.TryParse(arguments.At(1), out int deaths))
            {
                response = "Invalid value for deaths.";
                return false;
            }

            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var deathCollection = db.GetCollection<DeathCount>("death_counts");

                var playerData = deathCollection.FindOne(Query.EQ("PlayerId", steamId));
                if (playerData == null)
                {
                    response = "Player not found.";
                    return false;
                }

                playerData.Deaths = deaths;
                deathCollection.Update(playerData);
                response = $"Set deaths for player {playerData.PlayerName} to {deaths}.";
            }
            return true;
        }
    }

    public class SetScpKillsCommand : ICommand
    {
        public string Command { get; } = "setkillsasscp";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Sets the total kills as scp for a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: setkillsasscp <steamid> <value>";
                return false;
            }

            string steamId = arguments.At(0);
            if (!int.TryParse(arguments.At(1), out int scpKills))
            {
                response = "Invalid value for kills as scp.";
                return false;
            }

            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var playerData = killCollection.FindOne(Query.EQ("PlayerId", steamId));
                if (playerData == null)
                {
                    response = "Player not found.";
                    return false;
                }

                playerData.ScpKills = scpKills;
                killCollection.Update(playerData);
                response = $"Set kills as scp for player {playerData.PlayerName} to {scpKills}.";
            }
            return true;
        }
    }

    public class SetKilledScpsCommand : ICommand
    {
        public string Command { get; } = "setkilledscps";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Sets the total number of SCPs killed by a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: setkilledscps <steamid> <value>";
                return false;
            }

            string steamId = arguments.At(0);
            if (!int.TryParse(arguments.At(1), out int killedScps))
            {
                response = "Invalid value for killed SCPs.";
                return false;
            }

            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");

                var playerData = killCollection.FindOne(Query.EQ("PlayerId", steamId));
                if (playerData == null)
                {
                    response = "Player not found.";
                    return false;
                }

                playerData.KilledScps = killedScps;
                killCollection.Update(playerData);
                response = $"Set the number of SCPs killed by player {playerData.PlayerName} to {killedScps}.";
            }
            return true;
        }
    }
}