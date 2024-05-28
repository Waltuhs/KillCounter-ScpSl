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
        public string Description { get; } = "Gets the player's Kill/Death ratio, Total kills, kills as scp and deaths from the server-side database";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.DoNotTrack)
            {
                using (var db = new LiteDatabase("kill_counter.db"))
                {
                    //spaghet
                    var killCollection = db.GetCollection<KillCount>("kill_counts");
                    var deathCollection = db.GetCollection<DeathCount>("death_counts");

                    // get kill count
                    var killCount = killCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                    int kills = killCount != null ? killCount.Kills : 0;

                    //get scp kills
                    var scpkillcount = killCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                    int scpkills = killCount != null ? killCount.ScpKills : 0;

                    //get Killed scps
                    var Killedscpscount = killCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                    int killedscps = killCount != null ? killCount.KilledScps : 0;

                    // get deaths
                    var deathCount = deathCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                    int deaths = deathCount != null ? deathCount.Deaths : 0;

                    // calc + print
                    double ratio = deaths != 0 ? (double)kills / deaths : kills;
                    response = Plugin.Instance.Translation.KdResponse
                        .Replace("%TotalKills%", kills.ToString())
                        .Replace("%scpkills%", scpkills.ToString())
                        .Replace("%killedscps%", killedscps.ToString())
                        .Replace("%deaths%", deaths.ToString())
                        .Replace("%player%", player.Nickname)
                        .Replace("%kd%", ratio.ToString("F2"))
                        .Replace("%n%", "\n"); 
                }
            }
            else
            {
                response = "You have DNT active.";
            }
            return true;
        }
    }
}
