using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;

namespace KillCounter
{
    public class Plugin : Plugin<Config>
    {
        public static KillCounter Instance { get; private set; } = null;
        public override string Author => "sexy waltuh";
        public override string Name => "kill count";
        public override string Prefix => "KillCounter";
        public override Version Version => new Version(1, 1, 0);
        static Dictionary<Player, int> killsss = new Dictionary<Player, int>();

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;
            Exiled.Events.Handlers.Player.Died += OnPlayerDeath;
        }


        public override void OnDisabled()
        {
            base.OnDisabled();
            Instance = null;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDeath;
        }

        
        public void OnPlayerDeath(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                if (!killsss.ContainsKey(ev.Attacker))
                {
                    killsss[ev.Attacker] = 1;
                    string firstKillMessage = Config.firstkm.Replace("{kills}", "1");
                    ev.Attacker.ShowHint(firstKillMessage, Config.kmTime);
                }
                else
                {
                    killsss[ev.Attacker]++;
                    string PlayKills = killsss[ev.Attacker].ToString();
                    string killMessage = Config.km.Replace("{kills}", PlayKills);
                    ev.Attacker.ShowHint(killMessage, Config.kmTime);
                }
            }
        }
    }
}
