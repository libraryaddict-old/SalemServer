using System;
using System.Collections.Generic;

namespace SalemServer.Game.Roles
{
    public abstract class Role
    {
        private PlayerGame game;

        public Role(PlayerGame playerGame)
        {
            this.game = playerGame;
        }

        public PlayerGame GetGame()
        {
            return game;
        }

        /// <summary>
        /// Everytime the game phase changes, this will be fired
        /// </summary>
        /// <param name="phase"></param>
        public abstract void OnTime(GamePhase phase);

        /// <summary>
        /// Get name of the role
        /// </summary>
        /// <returns></returns>
        public abstract String GetName();

        public List<Player> GetAllPlayers()
        {
            return GetGame().GetAllPlayers();
        }

        public List<Player> GetLivingPlayers()
        {
            return GetGame().GetLivingPlayers();
        }

        public List<Player> GetDeadPlayers()
        {
            return GetGame().GetDeadPlayers();
        }

        public Boolean HasDeathNote()
        {
            return false;
        }

        public Boolean IsUnique()
        {
            return false;
        }

        /**
        * Returns the abilities owned by this role, each ability name should be unique.
*/

        public abstract Ability[] GetAbilities();

        /// <summary>
        /// Called every GamePhase and is called in order from the abilities with highest priority to
        /// the ones with lowest priority.
        ///
        /// As such its down to the role itself to handle the logic
        /// </summary>
        /// <param name="ability"></param>
        public abstract void CallAbility(PhaseActions actions, String ability);

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="ability"></param>
        /// <param name="action"></param>
        public abstract void DoAction(Player player, RoleAction action);
    }
}