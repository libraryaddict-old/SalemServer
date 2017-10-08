using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalemServer.Game.Roles
{
    /// <summary>
    /// Sent from client to server, tells the server if they used a role action. Such as jailing,
    /// sheriff checking, mafioso attacking. Who they selected as a target. Also used for role
    /// abilities, voting someone up and voting guilty/innocent.
    ///
    /// -1 generally represents cancel ability while the other numbers are role specific
    /// </summary>
    public class RoleAction
    {
        private String ability;
        private int target;

        public RoleAction(String ability, int target)
        {
            this.ability = ability;
            this.target = target;
        }

        public String GetAbility()
        {
            return ability;
        }

        public int GetTarget()
        {
            return target;
        }
    }

    /// <summary>
    /// What players can you select? If this is sent with only 'ability' then the player loses this
    /// on his client
    /// </summary>
    public class RoleTargets
    {
        /// <summary>
        /// If this is not null, then the player will see an icon that they can use. Such as mayor reveals
        ///
        /// This is an url to the icon to use
        /// </summary>
        private String openIcon;

        /// <summary>
        /// What ability is this registered under? Eg: mafioso
        /// </summary>
        private String ability;

        /// <summary>
        /// If this is null, then 'openIcon' must not be null (eg, mayor reveals).
        ///
        /// If 'openIcon' is not null and this is not null, then it opens a jailor menu.
        ///
        /// If 'openIcon' is null and this is not null. Then this is a voting menu for night
        /// activities or voting
        /// </summary>
        private List<int>[] targets;

        public RoleTargets(String ability)
        {
            this.ability = ability;
        }

        public RoleTargets(String ability, List<int> targetList) : this(ability)
        {
            targets = new List<int>[] { targetList };
        }

        public RoleTargets(String ability, List<Player> targetList) : this(ability, GetList(targetList))
        {
        }

        public RoleTargets(String ability, List<int>[] targetLists) : this(ability)
        {
            targets = targetLists;
        }

        public RoleTargets(String ability, List<Player>[] targetLists) : this(ability)
        {
            targets = new List<int>[targetLists.Length];

            for (int i = 0; i < targetLists.Length; i++)
            {
                targets[i] = GetList(targetLists[i]);
            }
        }

        public RoleTargets(String ability, String openIcon, List<int> targetList) : this(ability, openIcon)
        {
            targets = new List<int>[] { targetList };
        }

        public RoleTargets(String ability, String openIcon, List<Player> targetList) : this(ability, openIcon, GetList(targetList))
        {
        }

        public RoleTargets(String ability, String openIcon) : this(ability)
        {
            this.openIcon = openIcon;
        }

        private static List<int> GetList(List<Player> targetList)
        {
            List<int> list = new List<int>();
            targetList.ForEach(p => list.Add(p.GetPosition()));

            return list;
        }

        public String Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}