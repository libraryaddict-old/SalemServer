using SalemServer.Game.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game
{
    /// <summary>
    /// Keeps track of the night's actions
    /// </summary>
    public class PhaseActions
    {
        /// <summary>
        /// We keep track of the players incase of transports
        /// </summary>
        private List<Player> players = new List<Player>();

        private Dictionary<int, List<Player>> visitors = new Dictionary<int, List<Player>>();

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Player GetPlayer(int target)
        {
            return players[target];
        }

        public void Transport(int target1, int target2)
        {
            Player player = players[target1];
            players[target1] = players[target2];
            players[target2] = players[target1];
        }

        public Boolean IsJailed(int target)
        {
            return GetPlayer(target).HasState(StateType.JAILED);
        }

        public Boolean IsMafia(int target)
        {
            return GetRole(target) is Mafia || GetPlayer(target).HasState(StateType.FRAMED);
        }

        public Role GetRole(int target)
        {
            return GetPlayer(target).GetRole();
        }
    }
}