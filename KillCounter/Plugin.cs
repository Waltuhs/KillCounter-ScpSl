using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using MEC;
using PlayerRoles;
using LiteDB;
using Exiled.Events.EventArgs.Server;


namespace KillCounter
{
    public class Plugin : Plugin<Config, Translations>
    {
        public static Plugin Instance { get; private set; } = null;
        public override string Author => "sexy waltuh";
        public override string Name => "kill count";
        public override string Prefix => "KillCounter";
        public override Version Version => new Version(2, 1, 0);
        public static Dictionary<Player, int> killsss = new Dictionary<Player, int>();
        private Dictionary<Player, CoroutineHandle> spectatorCoroutines = new Dictionary<Player, CoroutineHandle>();
        private CoroutineHandle hintCoroutine;

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Player.Died += OnPlayerDeath;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer += OnChangingSpecedRole;
            Exiled.Events.Handlers.Server.RoundEnded += CleanUpDictionaries;
        }


        public override void OnDisabled()
        {
            base.OnDisabled();
            Instance = null;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDeath;
            Exiled.Events.Handlers.Player.ChangingSpectatedPlayer -= OnChangingSpecedRole;
            Exiled.Events.Handlers.Server.RoundEnded -= CleanUpDictionaries;
            Timing.KillCoroutines(hintCoroutine);
        }


        public void OnPlayerDeath(DiedEventArgs ev)
        {
            if (ev.Attacker != null && ev.Player != null)
            {
                if (Config.CountKillsAndDeathsBeforeRoundStarts == false && !Round.IsStarted)
                {
                    return;
                }
                if (Config.CountKillsAndDeathsAfterRoundEnds == false && Round.IsEnded)
                {
                    return;
                }
                if (!Config.CountKillsAndDeathsIfFriendlyFire == false && ev.Attacker.Role.Team == ev.Player.Role.Team)
                {
                    return;
                }
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
                            UpdateKillCount(ev.Attacker, ev.TargetOldRole);
                            UpdateKD(ev.Player);
                            UpdateKD(ev.Attacker);
                        }
                        else
                        {
                            killsss[ev.Attacker]++;
                            string killMessage = Config.km.Replace("{kills}", killsss[ev.Attacker].ToString());
                            ev.Attacker.ShowHint(killMessage, Config.kmTime);
                            UpdateDeathCount(ev.Player);
                            UpdateKillCount(ev.Attacker, ev.TargetOldRole);
                            UpdateKD(ev.Player);
                            UpdateKD(ev.Attacker);
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
                            UpdateKillCount(ev.Attacker, ev.TargetOldRole);
                            UpdateKD(ev.Attacker);
                        }
                        else
                        {
                            killsss[ev.Attacker]++;
                            string killMessage = Config.km.Replace("{kills}", killsss[ev.Attacker].ToString());
                            ev.Attacker.ShowHint(killMessage, Config.kmTime);
                            UpdateKillCount(ev.Attacker, ev.TargetOldRole);
                            UpdateKD(ev.Attacker);
                        }
                    }
                }
                else
                {
                    UpdateDeathCount(ev.Player);
                    UpdateKD(ev.Player);
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

        private void UpdateKillCount(Player player, RoleTypeId DeadPlayerOldRole)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var collection = db.GetCollection<KillCount>("kill_counts");
                var killCount = collection.FindOne(Query.EQ("PlayerId", player.UserId));

                if (killCount != null)
                {
                    killCount.Kills++;
                    killCount.PlayerName = player.Nickname; 
                    if (player.Role.Team == Team.SCPs)
                    {
                        killCount.ScpKills++;
                    }
                    if (!DeadPlayerOldRole.IsHuman())
                    {
                        killCount.KilledScps++;
                    }
                    if (player.DoNotTrack && !killCount.DNT)
                    {
                        killCount.DNT = true;
                    }
                    collection.Update(killCount);
                }
                else
                {
                    killCount = new KillCount
                    {
                        PlayerId = player.UserId,
                        PlayerName = player.Nickname,  
                        Kills = 1,
                        ScpKills = player.Role.Team == Team.SCPs ? 1 : 0,
                        KilledScps = !DeadPlayerOldRole.IsHuman() ? 1 : 0,
                        DNT = player.DoNotTrack
                    };
                    collection.Insert(killCount);
                }
            }
        }

        private void UpdateDeathCount(Player player)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var collection = db.GetCollection<DeathCount>("death_counts");
                var deathCount = collection.FindOne(Query.EQ("PlayerId", player.UserId));

                if (deathCount != null)
                {
                    deathCount.Deaths++;
                    deathCount.PlayerName = player.Nickname;  
                    if (player.DoNotTrack) deathCount.DNT = true;
                    collection.Update(deathCount);
                }
                else
                {
                    deathCount = new DeathCount
                    {
                        PlayerId = player.UserId,
                        PlayerName = player.Nickname,  
                        Deaths = 1,
                        DNT = player.DoNotTrack
                    };
                    collection.Insert(deathCount);
                }
            }
        }

        private void UpdateKD(Player player)
        {
            using (var db = new LiteDatabase("kill_counter.db"))
            {
                var killCollection = db.GetCollection<KillCount>("kill_counts");
                var deathCollection = db.GetCollection<DeathCount>("death_counts");

                var killCount = killCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                int kills = killCount != null ? killCount.Kills : 0;

                var deathCount = deathCollection.FindOne(Query.EQ("PlayerId", player.UserId));
                int deaths = deathCount != null ? deathCount.Deaths : 0;

                double ratio = deaths != 0 ? (double)kills / deaths : kills;
                if (killCount != null)
                {
                    killCount.KD = ratio;
                    killCount.PlayerName = player.Nickname;
                    killCollection.Update(killCount);
                }
                else
                {
                    killCount = new KillCount
                    {
                        PlayerId = player.UserId,
                        KD = ratio,
                        PlayerName = player.Nickname,
                        DNT = player.DoNotTrack
                    };
                    killCollection.Insert(killCount);
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
            public string PlayerName { get; set; }
            public int Kills { get; set; }
            public int ScpKills { get; set; }
            public int KilledScps { get; set; }
            public bool DNT {  get; set; }
        }

        public class DeathCount
        {
            public int Id { get; set; }
            public string PlayerId { get; set; }
            public string PlayerName { get; set; }
            public int Deaths { get; set; }
            public bool DNT { get; set; }
        }
    }
}
