using Exiled.API.Interfaces;
using System.ComponentModel;

namespace KillCounter
{
    public sealed class config : IConfig
    {
        [Description("is Kill Counter enabled?")]
        public bool IsEnabled { get; set; } = true;
        [Description("Debug messages?")]
        public bool Debug { get; set; } = false;

    }
}
