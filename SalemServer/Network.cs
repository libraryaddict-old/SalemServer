using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Fleck;
using SalemServer.Game;
using SalemServer.account;

namespace SalemServer
{
    public enum JsonTypeSend
    {
        /// <summary>
        /// Display a message in the players chatbox
        /// </summary>
        MESSAGE,

        /// <summary>
        /// Display a title/subtitle overlay on their screen with X expirary time
        /// </summary>
        ANNOUNCE,

        /// <summary>
        /// Display a will on their screen
        /// </summary>
        WILL,

        /// <summary>
        /// Display a deathnote on their screen
        /// </summary>
        DEATHNOTE,

        /// <summary>
        /// Sends a RoleTargets object which creates/hides buttons either as a jailor menu, mayor
        /// reveal or mafioso targeting at night menu
        /// </summary>
        TARGETS,

        /// <summary>
        /// Informs the client of the phase
        /// </summary>
        PHASE
    }

    public enum JsonTypeReceive
    {
        /// <summary>
        /// When they pick a name
        /// </summary>
        NAME,

        /// <summary>
        /// When they select an action using the voting menu
        /// </summary>
        ACTION,

        /// <summary>
        /// When they are talking
        /// </summary>
        MESSAGE,

        /// <summary>
        /// When they are filling in their last will
        /// </summary>
        WILL,

        /// <summary>
        /// When the killer is filling in his deathnote
        /// </summary>
        DEATHNOTE
    }

    public class Connection
    {
        private IWebSocketConnection connection;
        private Network network;
        private Account account;

        public Connection(Network network, IWebSocketConnection socket)
        {
            this.network = network;
            connection = socket;
        }

        public void SetAccount(Account account)
        {
            this.account = account;
        }

        public Account GetAccount()
        {
            return account;
        }

        public void Start()
        {
            connection.OnOpen = () =>
            {
                Console.WriteLine("Connection open.");
            };

            connection.OnClose = () =>
            {
                Console.WriteLine("Connection closed.");
                network.Remove(this);

                account.OnDisconnect();
            };

            connection.OnMessage = message =>
            {
                Console.WriteLine("Client Says: " + message);
                connection.Send(" client says: " + message);
                account.OnMessage(message);
            };
        }

        public void SendMessage(String message)
        {
            lock (this)
            {
                connection.Send(message);
            }
        }
    }

    public class Network
    {
        private Object readLock = new Object();
        private List<Connection> connections = new List<Connection>();
        private GameManager gameManager;

        public Network(GameManager manager)
        {
            this.gameManager = manager;
        }

        public Object GetLock()
        {
            return readLock;
        }

        public void Remove(Connection connection)
        {
            lock (readLock)
            {
                connections.Remove(connection);
            }
        }

        public void Start()
        {
            Fleck.WebSocketServer server = new Fleck.WebSocketServer("ws://0.0.0.0:62127");

            server.Start(socket =>
                    {
                        Connection connection = new Connection(this, socket);

                        lock (readLock)
                        {
                            connections.Add(connection);
                        }

                        connection.Start();

                        gameManager.OnJoin(connection);
                    }
                );

            // Sleep. Sleep the peace of the dead.
            System.Threading.Thread.Sleep(-1);

            //while (true) // We will never ever exit
            //    Console.ReadLine();
        }
    }
}