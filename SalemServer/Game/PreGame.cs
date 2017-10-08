using System;
using System.Collections.Generic;

namespace SalemServer
{
    internal class PreGame : UserHandler
    {
        private Stack<Player> players = new Stack<Player>();

        private PreGame()
        {
        }

        public void OnJoin(Player user)
        {
            throw new NotImplementedException();
        }

        public void OnMessage(string messageType, string json)
        {
            throw new NotImplementedException();
        }

        public void OnQuit(Player user)
        {
            throw new NotImplementedException();
        }
    }
}