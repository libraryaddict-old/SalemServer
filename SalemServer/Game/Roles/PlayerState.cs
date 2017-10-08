using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game.Roles
{
    public class StateType
    {
        private String name;
        private List<String> registeredStates = new List<String>();

        public StateType(String name)
        {
            this.name = name;

            if (registeredStates.Contains(GetName()))
            {
                throw new InvalidOperationException("The StateType '" + GetName() + "' has already been registered!");
            }

            registeredStates.Add(GetName());
        }

        public String GetName()
        {
            return name;
        }

        /// <summary>
        /// Used to store the death messages
        /// </summary>
        public static StateType DEATH = new StateType("death");

        /// <summary>
        /// The first deathnote to assign this wins. Used to display a deathnote after the player
        /// dies. Only should be set if the killer was successful
        /// </summary>
        public static StateType DEATHNOTE = new StateType("deathnote");

        /// <summary>
        /// Checks if someone is framed
        /// </summary>
        public static StateType FRAMED = new StateType("framed");

        /// <summary>
        /// Checks if someone was jailed
        /// </summary>
        public static StateType JAILED = new StateType("jailed");
    }

    public class PlayerState
    {
        private StateType state;
        private Predicate<GamePhase> expired;
        private Object extraInfo;

        public PlayerState(StateType state)
        {
            this.state = state;
        }

        public PlayerState(StateType state, Predicate<GamePhase> expires) : this(state)
        {
            this.expired = expires;
        }

        public PlayerState(StateType state, Predicate<GamePhase> expires, PlayerState extraInfo) : this(state, expires)
        {
            this.extraInfo = extraInfo;
        }

        public StateType GetState()
        {
            return state;
        }

        public Object GetExtraInfo()
        {
            return extraInfo;
        }

        public void SetExtraInfo(Object info)
        {
            this.extraInfo = info;
        }

        public Boolean HasExpired(GamePhase phase)
        {
            return expired != null && expired.Invoke(phase);
        }
    }
}