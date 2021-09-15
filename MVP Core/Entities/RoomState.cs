using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Entities
{
    public class RoomState
    {
        public int width = 0;
        public int height = 0;
        public Collision collisionLayer;
        public ObservableCollection<Layer> layers;
        public string name;
        public List<Zone> zones;
        public List<Entity> defaultEntities = new List<Entity>();
        public string SongName;

        public RoomState CopyOf()
        {
            RoomState returnedState = new RoomState();
            returnedState.width = width;
            returnedState.height = height;
            if (collisionLayer != null)
            {
                returnedState.collisionLayer = collisionLayer.CopyOf();
            }
            if (layers != null)
            {
                returnedState.layers = new ObservableCollection<Layer>();
                foreach (Layer layer in layers)
                {
                    returnedState.layers.Add(layer.CopyOf());
                }
            }
            returnedState.name = name;
            if (zones != null)
            {
                returnedState.zones = new List<Zone>();
                foreach (Zone zone in zones)
                {
                    returnedState.zones.Add(zone.CopyOf());
                }
            }
            if (defaultEntities != null)
            {
                returnedState.defaultEntities = new List<Entity>();
                foreach (Entity entity in defaultEntities)
                {
                    returnedState.defaultEntities.Add(entity.CopyOf());
                }
            }
            returnedState.SongName = SongName;
            return returnedState;
        }
    }
}
