using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace KillCounter
{
    public sealed class Config : IConfig
    {
        [Description("is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("are debug messages enabled?")]
        public bool Debug { get; set; } = false;

        [Description("kill message customization. {kills} = amount of kills")]
        public string km { get; set; } = "you have {kills} kills!";

        [Description("first kill message customization. {kills} = amount of kills (more for grammar e.g you have 1 kills. instead you have 1 kill)")]
        public string firstkm { get; set; } = "You have {kills} kill!";

        [Description("Kill message hint time (int)")]
        public int kmTime { get; set; } = 3;

        [Description("Spectator kill count hint message")]
        public string HintMessageSpec { get; set; } = "You are now spectating {Spectated} <color=#00FFFF>they have {kills} kills!</color>";

        [Description("is the spectator kill count hint enabled?")]
        public bool SpecHintIsEnabled { get; set; } = true;

        [Description("Are kills & deaths counted before round started?")]
        public bool CountKillsAndDeathsBeforeRoundStarts { get; set; } = true;

        [Description("Are kills & deaths counted on round end")]
        public bool CountKillsAndDeathsAfterRoundEnds { get; set; } = true;

        [Description("are friendly fire kills counted?")]
        public bool CountKillsAndDeathsIfFriendlyFire { get; set; } = true;
    }
}