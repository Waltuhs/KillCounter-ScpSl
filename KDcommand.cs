using CommandSystem;
using Exiled.API.Features;
using LiteDB;
using System;
using static KillCounter.Plugin;

namespace KillCounter
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class KDcommand : ICommand
    {
        public string Command { get; } = "KillDeathRatio";

        public string[] Aliases { get; } = new string[] { "KD" };

        public string Description { get; } = "Gets the player's Kill/Death ratio from the server-side database";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            Player player = Player.Get(sender);
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");
                var deathCollection = db.GetCollection<DeathCount>("death_counts");

                // get kill count spaghetti
                var killCount = killCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                int kills = killCount != null ? killCount.Kills : 0;

                // get deaths
                var deathCount = deathCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                int deaths = deathCount != null ? deathCount.Deaths : 0;

                // calc + print
                double ratio = deaths != 0 ? (double)kills / deaths : kills;
                response = $"\nPlayer: {player.Nickname}\nKills: {kills}\nDeaths: {deaths}\nKill/Death Ratio: {ratio:F2}";
            }
            return true;
        }
    }
}
