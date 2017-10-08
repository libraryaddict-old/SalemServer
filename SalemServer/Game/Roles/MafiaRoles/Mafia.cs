using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game.Roles
{
    public abstract class Mafia : Role
    {
        public Mafia(PlayerGame playerGame) : base(playerGame)
        {
        }
    }
}