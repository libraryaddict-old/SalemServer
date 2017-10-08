using System;
using System.Collections.Generic;

namespace SalemServer.Game.Roles
{
    internal class RoleAbilityException : Exception
    {
        public RoleAbilityException(String message)
          : base(message)
        {
        }
    }

    public class Ability
    {
        private String name;
        private LinkedList<String> goBefore = new LinkedList<String>();
        private LinkedList<String> goAfter = new LinkedList<String>();
        private Role role;

        public Ability(Role role, String name)
        {
            if (!name.Equals(name.ToLower()))
                throw new RoleAbilityException("The ability " + name + " needs to be lowercase");

            if (!name.Equals(name.Trim()))
                throw new RoleAbilityException("The ability " + name + " cannot have trailing spaces");

            this.name = name;
            this.role = role;
        }

        /// <summary>
        /// Called after the logic is done
        /// </summary>
        public void Clear()
        {
            goBefore.Clear();
            goAfter.Clear();
        }

        public Role GetRole()
        {
            return role;
        }

        public String GetName()
        {
            return name;
        }

        /**
         * The only time the methods involving the ability name should be called, is because a role has multiple abilities and those abilities cannot be performed at the same time.
         *
         *
         */

        public Ability GoAfter(String ability)
        {
            goAfter.AddLast(ability);
            return this;
        }

        public Ability GoBefore(String ability)
        {
            goBefore.AddLast(ability);
            return this;
        }

        public Ability GoAfter(Type role)
        {
            goAfter.AddLast("role:" + role.Name);
            return this;
        }

        public Ability GoBefore(Type role)
        {
            goBefore.AddLast("role:" + role.Name);
            return this;
        }

        public Boolean IsGoBefore(String ability)
        {
            return goBefore.Contains(ability);
        }

        public Boolean IsGoAfter(String ability)
        {
            return goAfter.Contains(ability);
        }

        public Boolean IsGoBefore(Type role)
        {
            return goBefore.Contains("role:" + role.Name);
        }

        public Boolean IsGoAfter(Type role)
        {
            return goAfter.Contains("role:" + role.Name);
        }
    }

    public class RolesHandler
    {
        private List<Ability> abilities = new List<Ability>();

        public RolesHandler(Role[] roles)
        {
            foreach (Role role in roles)
            {
                abilities.AddRange(role.GetAbilities());
            }

            abilities.Sort(new Comparison<Ability>(Compare));

            abilities.ForEach((ab) => ab.Clear());
        }

        public List<Ability> GetAbilitiesOrder()
        {
            return abilities;
        }

        private int Compare(Ability ab1, Ability ab2)
        {
            if (ab1.IsGoAfter(ab2.GetName()) && ab2.IsGoAfter(ab1.GetName()))
                throw new RoleAbilityException("The abilities " + ab1.GetName() + " and " + ab2.GetName() + " cannot both go after each other");

            if (ab1.IsGoAfter(ab2.GetRole().GetType()) && ab2.IsGoAfter(ab1.GetRole().GetType()))
                throw new RoleAbilityException("The roles " + ab1.GetRole().GetName() + " and " + ab2.GetRole().GetName() + " cannot both go after each other");

            if (ab1.IsGoBefore(ab2.GetName()) && ab2.IsGoBefore(ab1.GetName()))
                throw new RoleAbilityException("The abilities " + ab1.GetName() + " and " + ab2.GetName() + " cannot both go before each other");

            if (ab1.IsGoBefore(ab2.GetRole().GetType()) && ab2.IsGoBefore(ab1.GetRole().GetType()))
                throw new RoleAbilityException("The roles " + ab1.GetRole().GetName() + " and " + ab2.GetRole().GetName() + " cannot both go before each other");

            if (ab1.IsGoAfter(ab2.GetName()) || ab2.IsGoBefore(ab1.GetName()))
                return -1;

            if (ab1.IsGoAfter(ab2.GetRole().GetType()) || ab2.IsGoBefore(ab1.GetRole().GetType()))
                return -1;

            if (ab1.IsGoBefore(ab2.GetName()) || ab2.IsGoAfter(ab1.GetName()))
                return 1;

            if (ab1.IsGoBefore(ab2.GetRole().GetType()) || ab2.IsGoAfter(ab1.GetRole().GetType()))
                return 1;

            return 0;
        }
    }
}