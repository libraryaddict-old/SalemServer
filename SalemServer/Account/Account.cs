using SalemServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.account
{
    public interface AccountHandler
    {
        void OnQuit(Account account);

        void OnJoin(Account account);

        void OnMessage(String messageType, String json);
    }

    /// <summary>
    /// Class that handles a player's information, this should be data that persists between games
    /// </summary>
    public class Account
    {
        private Connection connection;
        private AccountHandler handler;
        private PlayerGame game;
        private GameManager manager;

        public Account(GameManager gameManager, Connection connection)
        {
            this.manager = gameManager;
            connection.SetAccount(this);
        }

        public void SendJson(JsonTypeSend type, String json)
        {
            connection.SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>
               {
                   { "type", "" + (int) type },
                   { "value", json }
               }));
        }

        public void OnDisconnect()
        {
            throw new NotImplementedException();
        }

        public void OnMessage(string message)
        {
            game.ReceiveMessage(this, message);
        }

        public void SetHandler(AccountHandler handler)
        {
            this.handler = handler;
        }
    }
}