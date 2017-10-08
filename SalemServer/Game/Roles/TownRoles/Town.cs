using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game.Roles
{
    public abstract class Town : Role
    {
        public Town(PlayerGame playerGame) : base(playerGame)
        {
        }
    }
}