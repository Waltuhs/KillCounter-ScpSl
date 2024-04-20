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

        [Description("first kill message. {kills} = amount of kills (more for grammar e.g you have 1 kills. instead you have 1 kill)")]
        public string firstkm { get; set; } = "You have {kills} kill!";

        [Description("Kill message hint time (int)")]
        public int kmTime { get; set; } = 3;
    }
}
