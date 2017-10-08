using SalemServer.account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SalemServer.Game
{
    public class GameManager
    {
        private PlayerGame game;
        private System.Threading.Thread thread;
        private Timer myTimer;
        private int tickAmount = 20;

        public GameManager()
        {
            tickAmount = Math.Min(1000, tickAmount);

            // Hack fix for anyone that does custom tick amounts which cannot be divided properly
            while (1000 % tickAmount != 0) tickAmount++;

            myTimer = new Timer(1000 / tickAmount)
            {
                AutoReset = true
            };

            myTimer.Elapsed += new ElapsedEventHandler(OnTick);
            myTimer.Enabled = true;

            lock (myTimer)
            {
                game = new PlayerGame(tickAmount);
            }
        }

        public void OnJoin(Connection connection)
        {
            Account account = new Account(this, connection);

            lock (myTimer)
            {
                game.OnJoin(account);
            }
        }

        public void OnQuit(Connection connection)
        {
            if (connection.GetAccount() == null) return;

            lock (myTimer)
            {
                game.OnQuit(connection.GetAccount());
            }
        }

        private void OnTick(object source, ElapsedEventArgs e)
        {
            lock (myTimer)
            {
                // We let only one instance run at the time
                if (thread != null) return;

                thread = new System.Threading.Thread(OnTick);
                thread.Start();
            }
        }

        private void OnTick()
        {
            // Finish up
            lock (myTimer)
            {
                game.OnTick();
                thread = null;
            }
        }
    }
}