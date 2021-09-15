using MVP_Core.Components;
using MVP_Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Managers
{
    public class ComponentManager : Manager<Component>
    {
        private static readonly Lazy<ComponentManager> lazy =
            new Lazy<ComponentManager>(() => new ComponentManager());

        public static ComponentManager Instance { get { return lazy.Value; } }

        // TODO: MOVE THIS LATER
        public List<Entity> entities;

        private ComponentManager()
        {
            Initialize();
        }

        public List<UpdateComponent> UpdateComponents()
        {
            return bank.Where(c => c.Value.GetType() == typeof(UpdateComponent)).Select(c => (UpdateComponent)c.Value).ToList();
        }

        public List<RenderComponent> RenderComponents()
        {
            return bank.Where(c => c.Value.GetType() == typeof(RenderComponent)).Select(c => (RenderComponent)c.Value).ToList();
        }

        public List<Component> GetComponentByEntityId(int id)
        {
            return bank.Where(c => c.Value.entityId == id).Select(c => c.Value).ToList();
        }

        public Component GetComponentByEntityId(int id, Type type)
        {
            return bank.Where(c => c.Value.entityId == id && c.Value.GetType() == type).Select(c => c.Value).FirstOrDefault();
        }

        public void DeactivateNonPlayerComponents()
        {
            foreach (Component component in bank.Where(c => !c.Value.IsPlayerComponent).Select(c => c.Value).ToList())
            {
                component.Deactivate();
            }
        }
    }
}
