using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game.Roles
{
    public class Mayor : Town
    {
        public Mayor(PlayerGame playerGame) : base(playerGame)
        {
        }

        public override Ability[] GetAbilities()
        {
            return new Ability[0];
        }

        public override string GetName()
        {
            return "Mayor";
        }

        public override void OnTime(GamePhase phase)
        {
            // Mayors have no action
        }
    }
}