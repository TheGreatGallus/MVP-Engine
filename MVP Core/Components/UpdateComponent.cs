using System;

namespace MVP_Core.Components
{
    public class UpdateComponent : Component
    {
        private Action<int> update;
        private Type type;
        public bool Active { get { return base.isActive; } }

        public UpdateComponent()
        {

        }

        public UpdateComponent(int eId) : base(eId)
        {

        }

        public void SetType(Type type)
        {
            this.type = type;
        }

        public void Invoke(string method)
        {
            type.GetMethod(method).Invoke(null, new object[] { });
        }

        public void SetUpdate(Action<int> updateAction)
        {
            update = updateAction;
        }

        public void Update(int num)
        {
            if (update != null) update.Invoke(num);
        }
    }
}
