using System;

namespace MVP_Core.Managers
{
    public class ScriptManager : Manager<Tuple<Type, Action<int>>>
    {
        private static readonly Lazy<ScriptManager> lazy =
            new Lazy<ScriptManager>(() => new ScriptManager());

        public static ScriptManager Instance { get { return lazy.Value; } }

        private ScriptManager()
        {
            Initialize();
        }
    }
}
