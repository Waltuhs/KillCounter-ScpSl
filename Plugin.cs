using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;

namespace KillCounter
{
    public class Plugin : Plugin<config>
    {
        public override string Author => "sexy waltuh";
        public override string Name => "kill count";
        public override string Prefix => "KillCounter";
        public override Version Version => new Version(1, 0, 0);
        static Dictionary<Player, int> killsss = new Dictionary<Player, int>();

        public override void OnEnabled()
        {
          
            base.OnEnabled();
            Exiled.Events.Handlers.Player.Died += OnPlayerDeath;
        }


        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Player.Died -= OnPlayerDeath;

        }
        static void OnPlayerDeath(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {

                if (!killsss.ContainsKey(ev.Attacker))
                {
                    killsss[ev.Attacker] = 0;
                    killsss[ev.Attacker]++;
                    ev.Attacker.ShowHint($"\n \n \n \n \n \n \n you have {killsss[ev.Attacker]} kill!", 3);
                }
                else
                {
                    killsss[ev.Attacker]++;
                    ev.Attacker.ShowHint($"\n \n \n \n \n \n \n you have {killsss[ev.Attacker]} kills!", 3);
                }
                
            }
            
        }
    }
}
