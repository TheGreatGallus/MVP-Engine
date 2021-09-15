using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MVP_Core.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Entities
{
    public class Zone : Entity
    {
        public static Dictionary<string, Type> types = new Dictionary<string, Type>();
        //public Rectangle boundary;
        private int thickness = 1;
        public bool selected = false;
        public string type = "";
        public Dictionary<string, string> args;

        public Zone CopyOf()
        {
            Zone returnedZone = new Zone(boundary.X, boundary.X + boundary.Width, boundary.Y, boundary.Y + boundary.Height, type, args);
            return returnedZone;
        }

        public Zone(int x1, int x2, int y1, int y2, string type, Dictionary<string, string> args)
        {
            int x = (x1 < x2) ? x1 : x2;
            int y = (y1 < y2) ? y1 : y2;
            position = new Vector2(x, y);
            int w = Math.Abs(x1 - x2);
            int h = Math.Abs(y1 - y2);
            dimensions = new Vector2(w, h);
            boundary = new Rectangle(x, y, w, h);
            this.type = type;
            if (args != null)
            {
                this.args = new Dictionary<string, string>();
                foreach (string key in args.Keys)
                {
                    this.args.Add(key, args[key]);
                }
            }
        }

        //public bool AttmeptCollide(Rectangle rect)
        //{
        //    bool collide = boundary.Intersects(rect);
        //    if (collide)
        //        Collide();
        //    return collide;
        //}

        //public virtual void Collide()
        //{

        //}

        public Vector2 GetDimensions()
        {
            return new Vector2(boundary.X, boundary.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color lineColor = (selected) ? Color.Yellow : Color.Red;
            spriteBatch.Draw(GameValues.pixel, new Rectangle(boundary.X, boundary.Y, boundary.Width, thickness), lineColor);
            spriteBatch.Draw(GameValues.pixel, new Rectangle(boundary.X, boundary.Y + boundary.Height, boundary.Width + thickness, thickness), lineColor);
            spriteBatch.Draw(GameValues.pixel, new Rectangle(boundary.X, boundary.Y, thickness, boundary.Height), lineColor);
            spriteBatch.Draw(GameValues.pixel, new Rectangle(boundary.X + boundary.Width, boundary.Y, thickness, boundary.Height), lineColor);
        }

        public virtual Dictionary<string, object> GetArgs()
        {
            Dictionary<string, object> argList = new Dictionary<string, object>();
            return argList;
        }

        public ObservableCollection<Tuple<string, string>> GetProperties()
        {
            ObservableCollection<Tuple<string, string>> propertiesList = new ObservableCollection<Tuple<string, string>>();
            propertiesList.Add(new Tuple<string, string>("x1", boundary.Left.ToString()));
            propertiesList.Add(new Tuple<string, string>("x2", boundary.Right.ToString()));
            propertiesList.Add(new Tuple<string, string>("y1", boundary.Top.ToString()));
            propertiesList.Add(new Tuple<string, string>("y2", boundary.Bottom.ToString()));
            foreach (string key in args.Keys)
            {
                propertiesList.Add(new Tuple<string, string>(key, args[key]));
            }
            return propertiesList;
        }
    }
}
