using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Entities
{
    public class Critter
    {
        //public static Dictionary<string, Tuple<Type, Action<int>>> Critters = new Dictionary<string, Tuple<Type, Action<int>>>();
        //public static Dictionary<string, Type> types = new Dictionary<string, Type>();
        public Type entityType;
        public Action<int> updateAction;
        public Action<int> collisionAction;
        public Vector2 dimensions;
        public string name;

        public Critter()
        {

        }

        public Critter(Type entityType, Action<int> updateAction, Vector2 dimensions, Action<int> collisionAction)
        {
            this.entityType = entityType;
            this.updateAction = updateAction;
            this.dimensions = dimensions;
            this.collisionAction = collisionAction;
            name = entityType.Name;
        }
    }
}
