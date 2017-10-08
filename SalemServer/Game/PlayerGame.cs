using SalemServer.account;
using SalemServer.Game.Roles;
using System;
using System.Collections.Generic;

namespace SalemServer.Game
{
    public class PlayerGame
    {
        private List<Player> players = new List<Player>();
        private List<Tuple<Player, String>> messages = new List<Tuple<Player, string>>();
        private GamePhase phase = GamePhase.PREGAME;
        private int time;
        private int ticksTillSecond;
        private readonly int ticksInSecond;
        private Object ReadLock { get; } = new object();
        private List<Player> died;
        private Dictionary<Player, int> votes = new Dictionary<Player, int>();
        private Player judged;
        private Dictionary<Player, String> lastWills = new Dictionary<Player, string>();
        private Dictionary<Player, String> deathNotes = new Dictionary<Player, string>();

        public PlayerGame(int ticksInSecond)
        {
            this.ticksInSecond = ticksInSecond;
        }

        /// <summary>
        /// Only used prior to game starting, currently only returns as many roles as there are players.
        ///
        /// This is not balanced out currently as its intended for testing purposes
        /// </summary>
        /// <returns></returns>
        public List<Role> GetRoles()
        {
            List<Role> roles = new List<Role>();

            for (int i = 0; i < GetAllPlayers().Count; i++)
            {
                // Always keep one mafioso in the game
                if (i == 0)
                {
                    roles.Add(GetRole(roles, typeof(Mafioso)));
                }
                // Every 3rd player is a framer
                else if (i % 3 == 0)
                {
                    roles.Add(GetRole(roles, typeof(Framer)));
                }
                // Aside from the first role, every first and second player of every three is sheriff
                else
                {
                    roles.Add(GetRole(roles, typeof(Sheriff)));
                }
            }

            return roles;
        }

        private Role GetRole(List<Role> roles, Type roleType)
        {
            Role role = roles.Find(r => r.GetType() == roleType);

            if (role != null)
                return role;

            return (Role)Activator.CreateInstance(roleType, this);
        }

        public void OnSecond()
        {
            if (--time != 0) return;

            SwitchPhase(GetGamePhase());
        }

        public GamePhase GetGamePhase()
        {
            return phase;
        }

        public List<Player> GetLivingPlayers()
        {
            return GetAllPlayers().FindAll(player => player.IsAlive());
        }

        public List<Player> GetDeadPlayers()
        {
            return GetAllPlayers().FindAll(player => !player.IsAlive());
        }

        public List<Player> GetAllPlayers()
        {
            return players;
        }

        public void OnTick()
        {
            lock (ReadLock)
            {
                HandleMessages();

                if (--ticksTillSecond > 0) return;

                ticksTillSecond = ticksInSecond;
                OnSecond();
            }
        }

        public void SetGamePhase(GamePhase newPhase)
        {
            phase = newPhase;
            ticksTillSecond = ticksInSecond;
            time = GameTime.GetTime(GetGamePhase());

            foreach (Player player in GetAllPlayers())
            {
                player.SendMessage("Phase changed to " + newPhase.ToString());
            }

            votes.Clear();
        }

        private Boolean NameConflicts(String name)
        {
            return GetAllPlayers().Exists(player => player.GetName() != null && player.GetName().Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private static Random rng = new Random();

        private void Shuffle<T>(List<T> list)
        {
            lock (rng)
            {
                int n = list.Count;

                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        }

        public void OnPhaseEnd(GamePhase gamePhase)
        {
            if (gamePhase == GamePhase.PICK_NAME)
            {
                foreach (Player player in GetAllPlayers())
                {
                    if (player.GetName() != null)
                        continue;

                    for (int i = 0; i < 40; i++)
                    {
                        String name = Messages.PickRandomName();

                        if (NameConflicts(name))
                            continue;

                        player.SetName(name);
                        break;
                    }

                    if (player.GetName() == null)
                    {
                        player.SetName("Player " + new Random().Next(1, 3000));
                    }
                }
            }
            else if (gamePhase == GamePhase.DAY_BEGIN)
            {
                died = GetDeadPlayers().FindAll(player => player.HasState(StateType.DEATH));
            }
            else if (gamePhase == GamePhase.VOTING)
            {
                String targets = new RoleTargets("vote").Serialize();

                GetLivingPlayers().ForEach(p => p.SendTargets(targets));
            }
        }

        public void OnPhaseStart(GamePhase gamePhase)
        {
            if (gamePhase == GamePhase.ASSIGNING_ROLE)
            {
                List<Role> roles = GetRoles();
                Shuffle(roles);
                Shuffle(GetAllPlayers());

                for (int i = 0; i < roles.Count; i++)
                {
                    Player player = GetAllPlayers()[i];
                    player.SetRoleAndPosition(roles[i], i);

                    player.SendMessage(String.Format(Messages.INFO_RECEIVED_ROLE, roles[i].GetName()));
                }
            }
            else if (gamePhase == GamePhase.DEATH_NAME)
            {
                BroadcastMessage(String.Format(Messages.ANNOUNCE_DEATH_NAME, died[0].GetName()));

                List<String> diedFrom = (List<String>)died[0].GetState(StateType.DEATH).GetExtraInfo();

                String death = "";

                for (int i = 0; i < diedFrom.Count; i++)
                {
                    death += (i > 0 ? ", " : "") + String.Format(diedFrom[i], i == 0 ? "He" : "he", i == 0 ? "" : "also ");
                }

                BroadcastMessage(death);
            }
            else if (gamePhase == GamePhase.DEATH_WILL_EXISTS)
            {
                if (lastWills.ContainsKey(died[0]))
                {
                    Announce(Messages.ANNOUNCE_DEATH_WILL_FOUND);
                }
                else
                {
                    Announce(Messages.ANNOUNCE_DEATH_WILL_NOT_FOUND);
                }
            }
            else if (gamePhase == GamePhase.DEATH_WILL_DISPLAY)
            {
                String lastWill = lastWills[died[0]];

                GetAllPlayers().ForEach(p => p.SendWill(lastWill));
            }
            else if (gamePhase == GamePhase.DEATH_DEATHNOTE_FOUND)
            {
                Announce(Messages.ANNOUNCE_DEATH_DEATHNOTE);
            }
            else if (gamePhase == GamePhase.DEATH_ROLE) // This was their role
            {
                Announce(String.Format(Messages.ANNOUNCE_DEATH_ROLE, died[0].GetRole().GetName()));

                died[0].RemoveState(StateType.DEATH);
                died.RemoveAt(0);
            }
            else if (gamePhase == GamePhase.VOTING)
            {
                String targets = new RoleTargets("vote", GetLivingPlayers()).Serialize();

                GetLivingPlayers().ForEach(p => p.SendTargets(targets));
            }
            else if (gamePhase == GamePhase.VOTING_DEATH_WILL_DISPLAY)
            {
                String lastWill = lastWills[judged];

                GetAllPlayers().ForEach(p => p.SendWill(lastWill));
            }
            else if (gamePhase == GamePhase.VOTING_DEATH_ROLE)
            {
                Announce(String.Format(Messages.ANNOUNCE_DEATH_ROLE, judged.GetRole().GetName()));
            }
        }

        /// <summary>
        /// Used to figure out what phase starts next
        /// </summary>
        /// <param name="currentPhase"></param>
        public void SwitchPhase(GamePhase currentPhase)
        {
            if (currentPhase == GamePhase.DAY_BEGIN) // The day has just begun, if there's deaths start showing them. Else go to discussion
            {
                if (died.Count > 0)
                {
                    SetGamePhase(GamePhase.DEATH_NAME);
                }
                else
                {
                    SetGamePhase(GamePhase.DISCUSSION);
                }
            }
            else if (currentPhase == GamePhase.DEATH_WILL_EXISTS)
            {
                if (lastWills.ContainsKey(judged))
                {
                    SetGamePhase(GamePhase.DEATH_WILL_DISPLAY);
                }
                else if (died[0].HasState(StateType.DEATHNOTE))
                {
                    SetGamePhase(GamePhase.DEATH_DEATHNOTE_FOUND);
                }
                else
                {
                    SetGamePhase(GamePhase.DEATH_ROLE);
                }
            }
            else if (currentPhase == GamePhase.DEATH_ROLE) // We've just finished displaying a death if there's more deaths. Do them. Else discussion
            {
                if (died.Count > 0)
                {
                    SetGamePhase(GamePhase.DEATH_NAME);
                }
                else
                {
                    SetGamePhase(GamePhase.DISCUSSION);
                }
            }
            else if (currentPhase == GamePhase.VOTING_DECISION)
            {
                int guilty = 0;
                int inno = 0;

                foreach (Player player in GetLivingPlayers())
                {
                    if (!votes.ContainsKey(player))
                    {
                        BroadcastMessage(String.Format(Messages.VOTING_VOTE_RESULTS_ABSTAINED, player.GetName()));
                    }
                    else if (votes[player] == 0)
                    {
                        guilty += player.GetRole() is Mayor ? 3 : 1;
                        BroadcastMessage(String.Format(Messages.VOTING_VOTE_RESULTS_GUILTY, player.GetName()));
                    }
                    else if (votes[player] == 1)
                    {
                        inno += player.GetRole() is Mayor ? 3 : 1;
                        BroadcastMessage(String.Format(Messages.VOTING_VOTE_RESULTS_INNOCENT, player.GetName()));
                    }
                }

                if (guilty > inno)
                {
                    SetGamePhase(GamePhase.VOTING_GUILTY);
                }
                else
                {
                    SetGamePhase(GamePhase.VOTING_INNO);
                }
            }
            else if (currentPhase == GamePhase.VOTING_DEATH_WILL_EXISTS)
            {
                if (lastWills.ContainsKey(judged))
                {
                    SetGamePhase(GamePhase.VOTING_DEATH_WILL_DISPLAY);
                }
                else
                {
                    SetGamePhase(GamePhase.VOTING_DEATH_ROLE);
                }
            }
            else
            {
                SetGamePhase(GameTime.GetNextPhase(currentPhase));
            }
        }

        public void Announce(String message)
        {
            GetAllPlayers().ForEach(p => p.Announce(message));
        }

        public void OnJoin(Account account)
        {
            lock (ReadLock)
            {
                Player player = new Player(account);

                players.Add(player);

                BroadcastMessage(String.Format(Messages.GAME_JOIN, "Player"));
            }
        }

        public void BroadcastMessage(String message)
        {
            GetAllPlayers().ForEach(player => player.SendMessage(message));
        }

        private void HandleChat(Player player, String message)
        {
            // TODO Validate who should see it, mafia chat, jailor chat, dead chat

            message = String.Format(player.IsAlive() ? Messages.CHAT_LIVING : Messages.CHAT_DEAD, System.Web.HttpUtility.HtmlEncode(message));

            foreach (Player viewer in GetAllPlayers())
            {
                viewer.SendMessage(message);
            }
        }

        public void ReceiveMessage(Account account, String message)
        {
            lock (ReadLock)
            {
                Player player = GetAllPlayers().Find(p => p.GetAccount() == account);

                if (player == null || player.GetName() == null) return;

                messages.Add(new Tuple<Player, string>(player, message));
            }
        }

        private void HandleMessages()
        {
            foreach (Tuple<Player, String> tuple in messages)
            {
                try
                {
                    OnMessage(tuple.Item1, tuple.Item2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            messages.Clear();
        }

        public void OnMessage(Player player, String message)
        {
            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(message);
            JsonTypeReceive messageType = (JsonTypeReceive)Enum.Parse(typeof(JsonTypeReceive), obj.GetValue("type").ToString());
            Newtonsoft.Json.Linq.JToken token = obj.GetValue("value");

            // If the player is talking
            if (messageType == JsonTypeReceive.MESSAGE)
            {
                HandleChat(player, token.ToString());
            }
            else if (messageType == JsonTypeReceive.WILL && player.IsAlive() && player.GetRole() != null)
            {
                String will = token.ToString().Trim();

                if (will.Length > 10000)
                    will = will.Substring(0, 10000);

                if (will.Length <= 0)
                {
                    lastWills.Remove(player);
                }
                else
                {
                    lastWills.Add(player, System.Web.HttpUtility.HtmlEncode(will));
                }
            }
            else if (messageType == JsonTypeReceive.DEATHNOTE && player.IsAlive() && player.GetRole() != null && player.GetRole().HasDeathNote())
            {
                String deathnote = token.ToString().Trim();

                if (deathnote.Length > 10000)
                    deathnote = deathnote.Substring(0, 10000);

                if (deathnote.Length <= 0)
                {
                    deathNotes.Remove(player);
                }
                else
                {
                    deathNotes.Add(player, System.Web.HttpUtility.HtmlEncode(deathnote));
                }
            }
            else if (messageType == JsonTypeReceive.NAME && GetGamePhase() == GamePhase.PICK_NAME)
            {
                String name = token.ToString().Trim();

                if (!new System.Text.RegularExpressions.Regex("(?!.*[ ]{2})[a-zA-Z ]{2,16}").IsMatch(name))
                {
                    player.SendMessage(Messages.INFO_INVALID_NAME);
                    return;
                }

                if (NameConflicts(name))
                {
                    player.SendMessage(Messages.INFO_NAME_EXISTS);
                    return;
                }

                if (player.GetName() == null)
                {
                    BroadcastMessage(String.Format(Messages.GAME_PICKED_NAME, name));
                }
                else
                {
                    BroadcastMessage(String.Format(Messages.GAME_PICKED_RENAME, player.GetName(), name));
                }

                player.SetName(name);
            }
            else if (messageType == JsonTypeReceive.ACTION)
            {
                RoleAction action = token.ToObject<RoleAction>();

                if (action.GetAbility().Equals("votes"))
                {
                    if (GetGamePhase() == GamePhase.VOTING)
                    {
                        if (action.GetTarget() < 0 || (votes.ContainsKey(player) && votes[player] == action.GetTarget()))
                        {
                            if (votes.ContainsKey(player))
                            {
                                int target = votes[player];
                                votes.Remove(player);

                                BroadcastMessage(String.Format(Messages.VOTING_UNVOTED_PLAYER, player.GetName(), GetAllPlayers()[target]));
                            }
                        }
                        else if (action.GetTarget() < GetAllPlayers().Count)
                        {
                            Player target = GetAllPlayers()[action.GetTarget()];

                            if (!target.IsAlive()) return;

                            BroadcastMessage(String.Format(votes.ContainsKey(player) ? Messages.VOTING_VOTED_ANOTHER_PLAYER : Messages.VOTING_VOTED_PLAYER, player.GetName(), target.GetName()));

                            votes.Add(player, action.GetTarget());
                        }

                        Dictionary<Player, int> voteCount = new Dictionary<Player, int>();
                        int votesNeeded = (GetLivingPlayers().Count / 2) + 1;

                        foreach (Player voter in votes.Keys)
                        {
                            Player p = GetAllPlayers()[votes[voter]];

                            if (!p.IsAlive())
                            {
                                // TODO Reset votes for this player

                                continue;
                            }

                            int newCount = (voteCount.ContainsKey(p) ? voteCount[p] : 0) + (voter.GetRole() is Mayor ? 3 : 1);

                            voteCount.Add(p, newCount);

                            if (newCount < votesNeeded)
                            {
                                continue;
                            }

                            String targets = new RoleTargets("vote").Serialize();

                            GetLivingPlayers().ForEach(pl => pl.SendTargets(targets));

                            SetGamePhase(GamePhase.VOTING_UP);
                            judged = p;
                        }
                    }
                    else if (GetGamePhase() == GamePhase.VOTING_DECISION)
                    {
                        if (action.GetTarget() < 0 || (votes.ContainsKey(player) && votes[player] == action.GetTarget()))
                        {
                            if (votes.ContainsKey(player))
                            {
                                int target = votes[player];
                                votes.Remove(player);

                                BroadcastMessage(String.Format(Messages.VOTING_JUDGEMENT_UNCAST, player.GetName(), GetAllPlayers()[target]));
                            }
                        }
                        else if (action.GetTarget() < GetAllPlayers().Count)
                        {
                            Player target = GetAllPlayers()[action.GetTarget()];

                            if (!target.IsAlive()) return;

                            BroadcastMessage(String.Format(votes.ContainsKey(player) ? Messages.VOTING_JUDGEMENT_SWITCH : Messages.VOTING_JUDGEMENT_CAST, player.GetName(), target.GetName()));

                            votes.Add(player, action.GetTarget());
                        }
                    }
                }
                else
                {
                    player.GetRole().DoAction(player, action);
                }
            }
        }

        public void OnQuit(Account account)
        {
            lock (ReadLock)
            {
                Player player = GetAllPlayers().Find(p => p.GetAccount() == account);

                if (player == null) return;

                if (player.GetName() == null)
                {
                    GetAllPlayers().Remove(player);
                    BroadcastMessage(String.Format(Messages.GAME_QUIT, "A player"));
                    return;
                }

                BroadcastMessage(String.Format(Messages.GAME_QUIT, player.GetName()));

                PlayerState state = player.GetState(StateType.DEATH);

                if (state == null)
                {
                    state = new PlayerState(StateType.DEATH);
                    state.SetExtraInfo(new List<String>());
                    player.SetState(state);
                }

                ((List<String>)state.GetExtraInfo()).Add(Messages.DEATH_SUICIDE);
            }
        }
    }
}