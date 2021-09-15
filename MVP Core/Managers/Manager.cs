using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Managers
{
    public class Manager<T>
    {
        // Manager Boilerplate Code
        //private static readonly Lazy<Manager> lazy =
        //    new Lazy<Manager>(() => new Manager());
        //
        //public static Manager Instance { get { return lazy.Value; } }
        //
        //private Manager()
        //{
        //    Initialize();
        //}

        protected Dictionary<string, T> bank;

        public virtual void Initialize()
        {
            bank = new Dictionary<string, T>();
        }

        public void RegisterItem(string name, T item)
        {
            if (!bank.ContainsKey(name))
                bank.Add(name, item);
        }

        public T GetItem(string name)
        {
            return bank[name];
        }

        public List<T> GetAll()
        {
            return bank.Select(i => i.Value).ToList();
        }

        public void ClearItems()
        {
            bank = new Dictionary<string, T>();
        }
    }
}
