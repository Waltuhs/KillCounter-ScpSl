using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using MEC;
using PlayerRoles;
using LiteDB;


namespace KillCounter
{
    public class Plugin : Plugin<Config, Translations>
    {
        public static Plugin Instance { get; private set; } = null;
        public override string Author => "sexy waltuh";
        public override string Name => "kill count";
        public override string Prefix => "KillCounter";
        public override Version Version => new Version(1, 2, 6);
        public static Dictionary<Player, int> killsss = new Dictionary<Player, int>();
        private Dictionary<Player, CoroutineHandle> spectatorCoroutines = new Dictionary<Player, CoroutineHandle>();
        private CoroutineHandle hintCoroutine;

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;
            Log.Warn("This version of KillCounter is a newer less tested release please report any and all bugs to walter.jr. on discord or the issues tab on github.");
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
            if (ev.Attacker != null && ev.Player != null)
            {
                if (!ev.Attacker.DoNotTrack)
                {
                    if (!ev.Player.DoNotTrack)
                    {
                        if (!killsss.ContainsKey(ev.Attacker))
                        {
                            killsss[ev.Attacker] = 0;
                            killsss[ev.Attacker]++;
                            string firstkillMessage = Config.firstkm.Replace("{kills}", killsss[ev.Attacker].ToString());
                            ev.Attacker.ShowHint(firstkillMessage, Config.kmTime);
                            UpdateDeathCount(ev.Player);
                            UpdateKillCount(ev.Attacker, ev.Player);
                        }
                        else
                        {
                            killsss[ev.Attacker]++;
                            string killMessage = Config.km.Replace("{kills}", killsss[ev.Attacker].ToString());
                            ev.Attacker.ShowHint(killMessage, Config.kmTime);
                            UpdateDeathCount(ev.Player);
                            UpdateKillCount(ev.Attacker, ev.Player);
                        }
                    }
                    else
                    {
                        if (!killsss.ContainsKey(ev.Attacker))
                        {
                            killsss[ev.Attacker] = 0;
                            killsss[ev.Attacker]++;
                            string firstkillMessage = Config.firstkm.Replace("{kills}", killsss[ev.Attacker].ToString());
                            ev.Attacker.ShowHint(firstkillMessage, Config.kmTime);
                            UpdateKillCount(ev.Attacker, ev.Player);
                        }
                        else
                        {
                            killsss[ev.Attacker]++;
                            string killMessage = Config.km.Replace("{kills}", killsss[ev.Attacker].ToString());
                            ev.Attacker.ShowHint(killMessage, Config.kmTime);
                            UpdateKillCount(ev.Attacker, ev.Player);
                        }
                    }
                }
                else
                {
                    UpdateDeathCount(ev.Player);
                }
            }
        }

        private void CleanUpDictionaries(RoundEndedEventArgs ev)
        {
            killsss.Clear();
            foreach (var coroutineHandle in spectatorCoroutines.Values)
            {
                Timing.KillCoroutines(coroutineHandle);
            }
            spectatorCoroutines.Clear();
        }

        public void OnChangingSpecedRole(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.Player != null && ev.NewTarget != null && Config.SpecHintIsEnabled == true)
            {
                if (spectatorCoroutines.ContainsKey(ev.Player) && Timing.IsRunning(spectatorCoroutines[ev.Player]))
                {
                    Timing.KillCoroutines(spectatorCoroutines[ev.Player]);
                    spectatorCoroutines.Remove(ev.Player);
                }

                if (!ev.NewTarget.DoNotTrack)
                {
                    if (!Plugin.killsss.ContainsKey(ev.NewTarget))
                    {
                        Plugin.killsss[ev.NewTarget] = 0;
                        string newSpectatedPlayerName = ev.NewTarget.Nickname;
                        string newSpecedUserId = ev.NewTarget.UserId;
                        string playerName = ev.Player.Nickname;
                        spectatorCoroutines[ev.Player] = Timing.RunCoroutine(SpectatorHintCoroutine(playerName, newSpectatedPlayerName, newSpecedUserId));
                    }
                    else
                    {
                        string newSpecedUserId = ev.NewTarget.UserId;
                        string newSpectatedPlayerName = ev.NewTarget.Nickname;
                        string playerName = ev.Player.Nickname;
                        spectatorCoroutines[ev.Player] = Timing.RunCoroutine(SpectatorHintCoroutine(playerName, newSpectatedPlayerName, newSpecedUserId));
                    }
                }
                else
                {
                    string playerName = ev.Player.Nickname;
                    spectatorCoroutines[ev.Player] = Timing.RunCoroutine(DNTSpectatorHintCoroutine(playerName));
                }
            }
        }


        private IEnumerator<float> SpectatorHintCoroutine(string playerName, string newSpectatedPlayerName, string newSpecedUserId)
        {
            Player player = Player.Get(playerName);
            Player newSpectatedPlayer = Player.Get(newSpectatedPlayerName);

            while (true)
            {
                if (player.Role != RoleTypeId.Spectator)
                {
                    yield break;
                }
                if (player == null)
                {
                    yield break;
                }

                if (newSpecedUserId == null)
                {
                    yield break;
                }

                if (killsss.ContainsKey(newSpectatedPlayer))
                {
                    string kills = killsss[newSpectatedPlayer].ToString();
                    string SpectatorMessage = Config.HintMessageSpec.Replace("{kills}", kills).Replace("{Spectated}", newSpectatedPlayer.Nickname);
                    player.ShowHint(SpectatorMessage, 1.5f);
                }
                yield return Timing.WaitForSeconds(1.2f);
                Timing.KillCoroutines(hintCoroutine);
            }
        }

        private void UpdateKillCount(Player player, Player DeadPlayer)
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
                        if(player.Role.Team == Team.SCPs)
                        {
                            killCount.ScpKills++;
                            collection.Update(killCount);
                        }
                        if(DeadPlayer.Role.Team == Team.SCPs)
                        {
                            killCount.KilledScps++;
                            collection.Update(killCount);
                        }
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
                    yield break;
                }
                player.ShowHint(Translation.DNTspechint.Replace("%n%", "\n"), 1.5f);
                yield return Timing.WaitForSeconds(1.2f);
            }
        }


        public class KillCount
        {
            public int Id { get; set; }
            public string PlayerId { get; set; } 
            public int Kills { get; set; }
            public int ScpKills { get; set; }
            public int KilledScps { get; set; }
        }

        public class DeathCount
        {
            public int Id { get; set; }
            public string PlayerId { get; set; } 
            public int Deaths { get; set; }
        }
    }
}
