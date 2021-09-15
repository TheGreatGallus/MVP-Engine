using MVP_Core.Entities;
using System;

namespace MVP_Core.Managers
{
    public class CritterManager : Manager<Critter>
    {
        private static readonly Lazy<CritterManager> lazy =
            new Lazy<CritterManager>(() => new CritterManager());

        public static CritterManager Instance { get { return lazy.Value; } }

        private CritterManager()
        {
            Initialize();
        }
    }
}
