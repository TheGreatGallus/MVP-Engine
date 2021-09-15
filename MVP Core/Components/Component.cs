namespace MVP_Core.Components
{
    public class Component
    {
        public int entityId;
        public bool IsActive
        {
            get { return isActive; }
        }
        protected bool isActive;
        public bool IsPlayerComponent;

        public Component()
        {
            isActive = false;
            IsPlayerComponent = false;
        }

        public Component(int eId)
        {
            isActive = true;
            entityId = eId;
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }
    }
}
