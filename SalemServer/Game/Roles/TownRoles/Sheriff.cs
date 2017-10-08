using System;
using System.Collections.Generic;

namespace SalemServer.Game.Roles
{
    public class Sheriff : Town
    {
        private Dictionary<Player, int> checks = new Dictionary<Player, int>();

        public Sheriff(PlayerGame game) : base(game)
        {
        }

        public override void CallAbility(PhaseActions actions, string ability)
        {
            foreach (Player player in checks.Keys)
            {
                int target = checks[player];
            }
        }

        public override void DoAction(Player player, RoleAction action)
        {
            if (action.GetTarget() < 0)
            {
                checks.Remove(player);
            }
            else
            {
                Player target = GetAllPlayers()[action.GetTarget()];

                if (!target.IsAlive()) return;

                checks.Add(player, action.GetTarget());
            }
        }

        public override Ability[] GetAbilities()
        {
            return new Ability[] { new Ability(this, "sheriff") };
        }

        public override string GetName()
        {
            return "Sheriff";
        }

        public override void OnTime(GamePhase phase)
        {
            if (phase != GamePhase.NIGHT_BEGIN)
            {
                checks.Clear();
                return;
            }

            // TODO Return sheriff results
        }
    }
}