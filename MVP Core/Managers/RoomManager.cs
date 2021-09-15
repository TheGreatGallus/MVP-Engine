using MVP_Core.Components;
using MVP_Core.Entities;
using MVP_Core.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVP_Core.Managers
{
    public class RoomManager : Manager<Room>
    {
        private static readonly Lazy<RoomManager> lazy =
            new Lazy<RoomManager>(() => new RoomManager());

        public static RoomManager Instance { get { return lazy.Value; } }

        public delegate void RoomChangedEventHandler(Room newRoom);
        public event RoomChangedEventHandler RoomChanged;

        public Room room;
        public int RoomWidth
        {
            get { return (room != null) ? room.width * GameValues.tileDim : 0; }
        }
        public int RoomHeight
        {
            get { return (room != null) ? room.height * GameValues.tileDim : 0; }
        }

        private RoomManager()
        {
            Initialize();
        }

        public Room GetItem(int id)
        {
            return bank.ElementAt(id - 1).Value;
        }

        public void ChangeRoom(int id)
        {
            if(bank.Count > id)
                ChangeRoom(bank.ElementAt(id - 1).Value.name);
        }

        public void ChangeRoom(string name)
        {
            room = GetItem(name);
            ComponentManager.Instance.DeactivateNonPlayerComponents();

            if (ComponentManager.Instance.entities.Count > 2)
            {
                ComponentManager.Instance.entities.RemoveRange(2, ComponentManager.Instance.entities.Count - 2);
            }
            int eid = 2;
            foreach (Entity entity in room.defaultEntities)
            {
                //Entity newEntity = new Entity(eid);
                Entity newEntity = (Entity)Activator.CreateInstance(Entity.types[entity.critter.name], new object[] { });
                newEntity.id = eid;
                newEntity.SetPosition(entity.position);
                newEntity.SetDimensions(entity.dimensions);
                ComponentManager.Instance.entities.Add(newEntity);
                UpdateComponent uc = ComponentManager.Instance.GetComponentByEntityId(eid, typeof(UpdateComponent)) as UpdateComponent;
                RenderComponent rc = ComponentManager.Instance.GetComponentByEntityId(eid, typeof(RenderComponent)) as RenderComponent;
                uc.entityId = entity.id;
                uc.SetType(entity.critter.entityType);
                uc.SetUpdate(entity.critter.updateAction);
                uc.entityId = eid;
                uc.Activate();
                rc.SetTexture("saabb");
                rc.Activate();

                eid++;
            }
            RoomChanged(room);
        }

        public void SaveRoom()
        {
            room.SaveRoom();
        }

        public List<string> GetRoomNames()
        {
            List<string> roomNames = new List<string>();
            roomNames = bank.Select(r => r.Value.name).ToList();
            return roomNames;
        }

        public void UpdateCurrentRoomName(string newName)
        {
            string oldName = room.name;
            bank.Remove(oldName);
            bank.Add(newName, room);
        }

        public List<Zone> GetZones()
        {
            if (room.zones.Count > 0)
                return room.zones;
            return new List<Zone>();
        }

        public ObservableCollection<Layer> GetLayers()
        {
            if (room == null)
                return new ObservableCollection<Layer>();
            return room.layers;
        }

        public ObservableCollection<Room> GetRooms()
        {
            ObservableCollection<Room> roomList = new ObservableCollection<Room>();
            foreach(KeyValuePair<string, Room> roomEntry in bank)
            {
                roomList.Add(roomEntry.Value);
            }
            return roomList;
        }
    }
}
