using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Entities
{
    public class Entity
    {
        public static Dictionary<string, Type> types = new Dictionary<string, Type>();
        public int id;
        public Vector2 position;
        public Vector2 dimensions;
        public Rectangle boundary;
        public Critter critter;
        public string critterName;
        public Dictionary<string, object> properties;

        public Entity CopyOf()
        {
            Entity returnedEntity = new Entity();
            returnedEntity.id = id;
            returnedEntity.position = new Vector2(position.X, position.Y);
            returnedEntity.dimensions = new Vector2(dimensions.X, position.Y);
            returnedEntity.critter = critter;
            return returnedEntity;
        }

        public Entity()
        {
            properties = new Dictionary<string, object>();
        }

        public Entity(int id)
        {
            this.id = id;
            position = new Vector2(100.0f, 100.0f);
            properties = new Dictionary<string, object>();
            boundary = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);
        }

        public bool AttmeptCollide(Rectangle rect)
        {
            boundary = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);
            bool collide = boundary.Intersects(rect);
            if (collide)
                Collide();
            return collide;
        }

        public virtual void Collide()
        {

        }

        public void SetCritter(Critter critter)
        {
            this.critter = critter;
            this.dimensions = critter.dimensions;
            this.critterName = critter.name;
        }

        public void SetPosition(int x, int y)
        {
            position.X = x;
            position.Y = y;
        }

        public void SetPosition(Vector2 newPos)
        {
            position = newPos;
        }

        public void SetDimensions(int x1, int x2, int y1, int y2)
        {
            dimensions = new Vector2(x2 - x1, y2 - y1);
        }

        public void SetDimensions(Vector2 newDim)
        {
            dimensions = newDim;
        }

        public void ShiftPosition(Vector2 shiftPos)
        {
            position += shiftPos;
        }
    }
}
