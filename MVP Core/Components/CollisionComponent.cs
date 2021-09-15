using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Components
{
    public class CollisionComponent : Component
    {
        private Action<int> collide;
        private Type type;
        public bool Active { get { return base.isActive; } }

        public CollisionComponent()
        {

        }

        public CollisionComponent(int eId) : base(eId)
        {

        }

        public void SetCollide(Action<int> updateAction)
        {
            collide = updateAction;
        }

        public void Collide(int num)
        {
            if (collide != null) collide.Invoke(num);
        }
    }
}
