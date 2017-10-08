using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game.Roles
{
    public class Framer : Mafia
    {
        public Framer(PlayerGame game) : base(game)
        {
        }

        public override void CallAbility(string ability)
        {
            throw new NotImplementedException();
        }

        public override void DoAbility(Player player, string ability)
        {
            throw new NotImplementedException();
        }

        public override Ability[] GetAbilities()
        {
            return new Ability[] { new Ability(this, "frame").GoBefore(typeof(Sheriff)) };
        }

        public override string GetName()
        {
            return "Framer";
        }

        public override void OnTime(GamePhase phase)
        {
            throw new NotImplementedException();
        }
    }
}