using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SalemServer.Game;

namespace SalemServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Network(new GameManager()).Start();
        }
    }
}