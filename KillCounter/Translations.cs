using Exiled.API.Interfaces;
using System.ComponentModel;

namespace KillCounter
{
    public class Translations : ITranslation
    {
        [Description("response to kd command %kills% = kills, %deaths% = deaths, %player% = player and %kd% = kd also use %n% for a newline")]
        public string KdResponse { get; set; } = "%n% Player: %player% %n% Kills: %kills% %n% Deaths: %deaths% %n% Kill/Death Ratio: %kd%";

        [Description("When spectating a DNT player, %n% for a newline")]
        public string DNTspechint { get; set; } = "%n% %n% %n% %n% %n% the player your spectating has DNT active.";
    }
}
