using iKudo.Domain.Model;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using iKudo.Common;

namespace iKudo.Domain.Logic
{
    public class JoinStateFactory
    {
        public JoinStateFactory()
        {

        }

        private static Dictionary<string, BaseJoinStatus> stateCache = GetJoinStates();

        public static BaseJoinStatus GetState(string name)
        {
            return stateCache[name];
        }

        public static BaseJoinStatus GetState(JoinStatus state)
        {
            return stateCache[state.GetDisplayName()];
        }

        private static Dictionary<string, BaseJoinStatus> GetJoinStates()
        {
            var derivedType = typeof(BaseJoinStatus);
            var assembly = Assembly.GetAssembly(typeof(BaseJoinStatus));
            return assembly.GetTypes()
                           .Where(x => x != derivedType && derivedType.IsAssignableFrom(x))
                           .Select(x => (BaseJoinStatus)Activator.CreateInstance(x))
                           .ToDictionary(x => x.Name);
        }
    }
}
