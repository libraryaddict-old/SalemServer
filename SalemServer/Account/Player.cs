using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SalemServer.Game;
using SalemServer.Game.Roles;
using SalemServer.account;

namespace SalemServer
{
    /// <summary>
    /// Data that only persists in the player game itself and is automatically removed when the game
    /// ends. Used to track ingame state.
    /// </summary>
    public class Player
    {
        private Account account;
        private Role role;
        private Boolean alive = true;
        private List<PlayerState> states = new List<PlayerState>();
        private String name;
        private int position;

        public Player(Account account)
        {
            this.account = account;
        }

        public void SetRoleAndPosition(Role role, int position)
        {
            this.role = role;
            this.position = position;
        }

        public int GetPosition()
        {
            return position;
        }

        public void SetName(String name)
        {
            this.name = name;
        }

        public Account GetAccount()
        {
            return account;
        }

        private void SendJson(JsonTypeSend type, String json)
        {
            account.SendJson(type, json);
        }

        public void SendTargets(RoleTargets action)
        {
            SendTargets(Newtonsoft.Json.JsonConvert.SerializeObject(action));
        }

        public void SendWill(String will)
        {
            SendJson(JsonTypeSend.WILL, will);
        }

        public void SendDeathnote(String deathnote)
        {
            SendJson(JsonTypeSend.DEATHNOTE, deathnote);
        }

        public void SendTargets(String serializedAction)
        {
            SendJson(JsonTypeSend.TARGETS, serializedAction);
        }

        public void SendMessage(String message)
        {
            SendJson(JsonTypeSend.MESSAGE, message);
        }

        public void Announce(String message)
        {
            SendJson(JsonTypeSend.ANNOUNCE, message);
        }

        /// <summary>
        /// If 'expires' is more than 0, then this is a timed phase and the player shall see a
        /// countdown in their screen
        ///
        /// Otherwise it hides the countdown.
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="expires"></param>
        public void SendPhase(GamePhase phase, int expires)
        {
            SendJson(JsonTypeSend.PHASE, Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>
               {
                   { "phase", phase.ToString() },
                   { "expires", "" + expires }
               }));
        }

        public Role GetRole()
        {
            return role;
        }

        public String GetName()
        {
            return name;
        }

        public Boolean IsAlive()
        {
            return alive;
        }

        public PlayerState GetState(StateType stateType)
        {
            return states.Find(state => state.GetState() == stateType);
        }

        public Boolean HasState(StateType state)
        {
            return states.Find(s => s != null && s.GetState().Equals(state)) != null;
        }

        public void SetState(StateType state)
        {
            SetState(state, null);
        }

        public void SetState(StateType state, Predicate<GamePhase> expires)
        {
            SetState(new PlayerState(state, expires));
        }

        public void SetState(PlayerState state)
        {
            if (HasState(state.GetState())) return;

            states.Add(state);
        }

        public void RemoveState(StateType state)
        {
            states.RemoveAll(s => s != null && s.GetState().Equals(state));
        }

        public void CheckExpires(GamePhase phase)
        {
            states.RemoveAll(s => s != null && s.HasExpired(phase));
        }
    }
}