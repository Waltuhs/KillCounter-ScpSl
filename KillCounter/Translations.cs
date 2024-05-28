using Exiled.API.Interfaces;
using System.ComponentModel;

namespace KillCounter
{
    public class Translations : ITranslation
    {
        [Description("response to kd command %TotalKills% = kills, %scpkills% = kills as scp, %killedscps% = scps killed, %deaths% = deaths, %player% = player and %kd% = kd also use %n% for a newline")]
        public string KdResponse { get; set; } = "%n% Player: %player% %n% Kills: %TotalKills% %n% kills as scp: %scpkills% %n% killed scps: %killedscps% %n% Deaths: %deaths% %n% Kill/Death Ratio: %kd%";

        [Description("When spectating a DNT player, %n% for a newline")]
        public string DNTspechint { get; set; } = "%n% %n% %n% %n% %n% the player your spectating has DNT active.";

        [Description("the name of the leaderboard parent command")]
        public string leaderboardcommandname { get; set; } = "leaderboard";

        [Description("the shorter prefix of the leaderboard parent command")]
        public string leaderboardcommandalias { get; set; } = "lb";

        [Description("the description of the leaderboard parent command")]
        public string leaderboardcommanddesc { get; set; } = "Shows the leaderboard for various metrics.";

        [Description("the name of the subcommand for kills in the leaderboard command")]
        public string killssubcmdname { get; set; } = "kills";

        [Description("the description of the subcommand for kills in the leaderboard command")]
        public string killssubcmddesc { get; set; } = "Shows the top players by total kills.";

        [Description("the header of the subcommand for kills in the leaderboard command")]
        public string LeaderboardKillsHeader { get; set; } = "Top 10 Kills:%n%";

        [Description("the line for each player on the leaderboard of the subcommand for kills in the leaderboard command")]
        public string LeaderboardKillsEntry { get; set; } = "%rank%. %player% has %kills% Kills";

        [Description("the name of the subcommand for deaths in the leaderboard command")]
        public string deathssubcmdname { get; set; } = "deaths";

        [Description("the description of the subcommand for deaths in the leaderboard command")]
        public string deathssubcmddesc { get; set; } = "Shows the top players by total deaths.";

        [Description("the header of the subcommand for deaths in the leaderboard command")]
        public string LeaderboardDeathsHeader { get; set; } = "Top 10 Deaths:%n%";

        [Description("the line for each player on the leaderboard of the subcommand for deaths in the leaderboard command")]
        public string LeaderboardDeathsEntry { get; set; } = "%rank%. %player% has %deaths% Deaths";

        [Description("the name of the subcommand for scpkills in the leaderboard command")]
        public string ScpKillssubcmdname { get; set; } = "scpkills";

        [Description("the description of the subcommand for scpkills in the leaderboard command")]
        public string ScpKillssubcmddesc { get; set; } = "Shows the top players by total scp kills.";

        [Description("the header of the subcommand for scpkills in the leaderboard command")]
        public string LeaderboardScpKillsHeader { get; set; } = "Top 10 SCP Kills:%n%";

        [Description("the line for each player on the leaderboard of the subcommand for scpkills in the leaderboard command")]
        public string LeaderboardScpKillsEntry { get; set; } = "%rank%. %player% has %scpkills% SCP Kills";

        [Description("the name of the subcommand for killsAsScp in the leaderboard command")]
        public string KillsAsScpsubcmdname { get; set; } = "killsAsScp";

        [Description("the description of the subcommand for killsAsScp in the leaderboard command")]
        public string KillsAsScpsubcmddesc { get; set; } = "Shows the top players by total kills as scp.";

        [Description("the header of the subcommand for killsAsScp in the leaderboard command")]
        public string LeaderboardKillsAsScpHeader { get; set; } = "Top 10 Kills as SCP:%n%";

        [Description("the line for each player on the leaderboard of the subcommand for killsAsScp in the leaderboard command")]
        public string LeaderboardKillsAsScpEntry { get; set; } = "%rank%. %player% has %killsasscp% Kills as SCP";
    }
}

