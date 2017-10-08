using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game
{
    /// <summary>
    /// Represents a chat channel that lasts as long as needed
    /// </summary>
    public class ChatChannel
    {
        /// <summary>
        /// This exists only when its a message from a player
        /// </summary>
        private Player chatter;

        /// <summary>
        /// This exists only when its a whisper to a target
        /// </summary>
        private Player target;

        /// <summary>
        /// The message being delivered
        /// </summary>
        private String message;

        /// <summary>
        /// A predicate that's used to check if the player can view this message
        /// </summary>
        private Predicate<Player> canView;

        public ChatChannel(Player chatter, String message, Predicate<Player> canView)
        {
            this.chatter = chatter; this.message = message; this.canView = canView;
        }

        public Boolean CanView(Player player)
        {
            return canView.Invoke(player);
        }
    }
}