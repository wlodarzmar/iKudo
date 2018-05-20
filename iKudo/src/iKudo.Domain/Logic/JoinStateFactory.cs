using iKudo.Common;
using iKudo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace iKudo.Domain.Logic
{
    public static class JoinStateFactory
    {
        private static Dictionary<string, JoinState> stateCache = GetJoinStates();

        public static JoinState GetState(string name)
        {
            return stateCache[name];
        }

        public static JoinState GetState(JoinStatus state)
        {
            return stateCache[state.GetDisplayName()];
        }

        private static Dictionary<string, JoinState> GetJoinStates()
        {
            var derivedType = typeof(JoinState);
            var assembly = Assembly.GetAssembly(typeof(JoinState));
            return assembly.GetTypes()
                           .Where(x => x != derivedType && derivedType.IsAssignableFrom(x))
                           .Select(x => (JoinState)Activator.CreateInstance(x))
                           .ToDictionary(x => x.Name);
        }
    }
}
