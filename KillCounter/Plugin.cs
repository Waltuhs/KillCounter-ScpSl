using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using MEC;
using PlayerRoles;
using LiteDB;


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
            Log.Warn("This version of KillCounter is a newer less tested release please report any and all buys to walter.jr. on discord or the issues tab on github.");
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

            if(!ev.Attacker.DoNotTrack)
            {

                if (ev.Attacker != null && ev.Player != null)
                {
                    if (!killsss.ContainsKey(ev.Attacker))
                    {
                        killsss[ev.Attacker] = 0;
                    }
                    killsss[ev.Attacker]++;
                    UpdateKillCount(ev.Attacker);
                    string killMessage = Config.km.Replace("{kills}", killsss[ev.Attacker].ToString());
                    ev.Attacker.ShowHint(killMessage, Config.kmTime);
                }
            }

            UpdateDeathCount(ev.Player);
        }

        public void OnChangingSpecedRole(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.Player != null && ev.NewTarget != null)
            {

                if (!ev.NewTarget.DoNotTrack)
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
                else
                {
                 string playerName = ev.Player.Nickname;
                 Timing.RunCoroutine(DNTSpectatorHintCoroutine(playerName));
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
                    player.ShowHint(SpectatorMessage, 1.5f);
                }
                yield return Timing.WaitForSeconds(1.2f);
            }
        }

        private void UpdateKillCount(Player player)
        {
            if (!player.DoNotTrack)
            {
                using (var db = new LiteDatabase("kill_counter.db"))
                {
                    var collection = db.GetCollection<KillCount>("kill_counts");
                    var killCount = collection.FindOne(Query.EQ("PlayerId", player.UserId));

                    if (killCount != null)
                    {
                        killCount.Kills++;
                        collection.Update(killCount);
                    }
                    else
                    {
                        killCount = new KillCount { PlayerId = player.UserId, Kills = 1 };
                        collection.Insert(killCount);
                    }
                }
            }
        }

        private void UpdateDeathCount(Player player)
        {
            if(!player.DoNotTrack)
            {
                using (var db = new LiteDatabase("kill_counter.db"))
                {
                    var collection = db.GetCollection<DeathCount>("death_counts");
                    var deathCount = collection.FindOne(Query.EQ("PlayerId", player.UserId));

                    if (deathCount != null)
                    {
                        deathCount.Deaths++;
                        collection.Update(deathCount);
                    }
                    else
                    {
                        deathCount = new DeathCount { PlayerId = player.UserId, Deaths = 1 };
                        collection.Insert(deathCount);
                    }
                }
            }
        }

        private IEnumerator<float> DNTSpectatorHintCoroutine(string playerName)
        {
            Player player = Player.Get(playerName);

            while (true)
            {
                if (player.Role != RoleTypeId.Spectator)
                {
                    yield break;
                }
                if (player == null)
                {
                    Log.Error($"Player '{playerName}' detected null");
                    yield break;
                }
                player.ShowHint("\n \n \n \n \n \n the player your spectating has DNT active.", 1.5f);
                yield return Timing.WaitForSeconds(1.2f);
            }
        }


        public class KillCount
        {
            public int Id { get; set; }
            public string PlayerId { get; set; } 
            public int Kills { get; set; }
        }

        public class DeathCount
        {
            public int Id { get; set; }
            public string PlayerId { get; set; } 
            public int Deaths { get; set; }
        }

    }
}
