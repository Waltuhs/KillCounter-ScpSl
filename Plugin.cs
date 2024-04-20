using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using MEC;
using PlayerRoles;


namespace KillCounter
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; } = null;
        public override string Author => "sexy waltuh";
        public override string Name => "kill count";
        public override string Prefix => "KillCounter";
        public override Version Version => new Version(1, 2, 0);
        public static Dictionary<Player, int> killsss = new Dictionary<Player, int>();
        private CoroutineHandle hintCoroutine;

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;
            Log.Warn("This version of KillCounter is UNTESTED please open any issues in the issues tab on github.");
            Exiled.Events.Handlers.Player.Died += OnPlayerDeath;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += OnChangingSpecedRole;
        }


        public override void OnDisabled()
        {
            base.OnDisabled();
            Instance = null;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDeath;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= OnChangingSpecedRole;
            Timing.KillCoroutines(hintCoroutine);
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
        public void OnChangingSpecedRole(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.Player != null && ev.NewTarget != null)
            {
                if (!Plugin.killsss.ContainsKey(ev.NewTarget))
                {
                    Plugin.killsss[ev.NewTarget] = 0;
                    string newSpectatedPlayerName = ev.NewTarget.Nickname;
                    string playerName = ev.Player.Nickname;
                    Timing.RunCoroutine(SpectatorHintCoroutine(playerName, newSpectatedPlayerName));
                }
                else
                {
                    string newSpectatedPlayerName = ev.NewTarget.Nickname;
                    string playerName = ev.Player.Nickname;
                    Timing.RunCoroutine(SpectatorHintCoroutine(playerName, newSpectatedPlayerName));
                    { }
                }

            }
        }
        private IEnumerator<float> SpectatorHintCoroutine(string playerName, string newSpectatedPlayerName)
        {
            Player player = Player.Get(playerName);
            Player newSpectatedPlayer = Player.Get(newSpectatedPlayerName);

            while (true)
            {
                if (player.Role != RoleTypeId.Spectator)
                {
                    yield break; 
                }
                if (player == null || newSpectatedPlayer == null)
                {
                    Log.Error($"Player '{playerName}' or new spectated player '{newSpectatedPlayerName}' detected null");
                    yield break;
                }

                if (killsss.ContainsKey(newSpectatedPlayer))
                {
                    string kills = killsss[newSpectatedPlayer].ToString();
                    string SpectatorMessage = Config.HintMessageSpec.Replace("{kills}", kills).Replace("{Spectated}", newSpectatedPlayer.Nickname);
                    player.ShowHint(SpectatorMessage, 10);
                }
                yield return Timing.WaitForSeconds(1.2f);
            }
        }
    }
}
