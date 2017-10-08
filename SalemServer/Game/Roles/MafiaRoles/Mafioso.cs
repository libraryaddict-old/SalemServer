using System;

namespace SalemServer.Game.Roles
{
    public class Mafioso : Mafia
    {
        public Mafioso(PlayerGame game) : base(game)
        {
        }

        public override string GetName()
        {
            return "Mafioso";
        }

        public override void OnTime(GamePhase phase)
        {
            throw new NotImplementedException();
        }
    }
}