using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Global;
using MVP_Core.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVP_Core.Entities
{
    public class Room
    {
        public string AreaPath;
        public int width
        {
            get { return currentRoomState.width; }
            set { if (ChangeRoomState()) currentRoomState.width = value; }
        }
        public int height
        {
            get { return currentRoomState.height; }
            set { if (ChangeRoomState()) currentRoomState.height = value; }
        }
        public Collision collisionLayer
        {
            get { return currentRoomState.collisionLayer; }
            set { if (ChangeRoomState()) currentRoomState.collisionLayer = value; }
        }
        public ObservableCollection<Layer> layers
        {
            get { return currentRoomState.layers; }
            set { if (ChangeRoomState()) currentRoomState.layers = value; }
        }
        public string name
        {
            get { return currentRoomState.name; }
            set { if (ChangeRoomState()) currentRoomState.name = value; }
        }
        public List<Zone> zones
        {
            get { return currentRoomState.zones; }
            set { if (ChangeRoomState()) currentRoomState.zones = value; }
        }
        public List<Entity> defaultEntities
        {
            get { return currentRoomState.defaultEntities; }
            set { if (ChangeRoomState()) currentRoomState.defaultEntities = value; }
        }
        public string SongName
        {
            get { return currentRoomState.SongName; }
            set { if (ChangeRoomState()) currentRoomState.SongName = value; }
        }
        RoomState currentRoomState;
        Stack<RoomState> pastRoomStates;
        Stack<RoomState> futureRoomStates;

        public bool ChangeRoomState()
        {
            while (futureRoomStates.Count > 0)
                futureRoomStates.Pop();
            futureRoomStates = new Stack<RoomState>();
            pastRoomStates.Push(currentRoomState.CopyOf());
            return true;
        }

        public bool UndoRoomState()
        {
            if (pastRoomStates.Count != 0)
            {
                futureRoomStates.Push(currentRoomState.CopyOf());
                currentRoomState = pastRoomStates.Pop();
                return true;
            }
            else
                return false;
        }

        public bool RedoRoomState()
        {
            if (futureRoomStates.Count != 0)
            {
                pastRoomStates.Push(currentRoomState.CopyOf());
                currentRoomState = futureRoomStates.Pop();
                return true;
            }
            return false;
        }

        public Room(int w, int h, int num)
        {
            currentRoomState = new RoomState();
            pastRoomStates = new Stack<RoomState>();
            futureRoomStates = new Stack<RoomState>();

            AreaPath = GameValues.Path + "Brin/";
            zones = new List<Zone>();
            defaultEntities = new List<Entity>();
            width = w;
            height = h;
            this.name = "" + num;

            layers = new ObservableCollection<Layer>();
            layers.Add(new Layer());
        }

        public Room(int w, int h, string name)
        {
            currentRoomState = new RoomState();
            pastRoomStates = new Stack<RoomState>();
            futureRoomStates = new Stack<RoomState>();
            AreaPath = GameValues.Path + "Brin/";
            zones = new List<Zone>();
            defaultEntities = new List<Entity>();
            width = w;
            height = h;
            this.name = name;

            collisionLayer = new Collision();
            collisionLayer.tiles = new Tile[width, height];
            collisionLayer.width = w;
            collisionLayer.height = h;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Tile refTile = Tile.tileDictionary["0"];
                    Tile newTile = new Tile(i * GameValues.tileDim, j * GameValues.tileDim, refTile.c, refTile.t, refTile.sequence, "0");
                    collisionLayer.tiles[i, j] = newTile;
                }
            }

            layers = new ObservableCollection<Layer>();
            layers.Add(new Layer());
            layers[0].name = "layer";
            layers[0].width = w;
            layers[0].height = h;
            layers[0].depth = 0;
        }

        public Room(int num)
        {
            currentRoomState = new RoomState();
            pastRoomStates = new Stack<RoomState>();
            futureRoomStates = new Stack<RoomState>();
            AreaPath = GameValues.Path + "Brin/";
            this.name = "" + num;
            LoadRoom();
        }

        public Room(string name)
        {
            currentRoomState = new RoomState();
            pastRoomStates = new Stack<RoomState>();
            futureRoomStates = new Stack<RoomState>();
            AreaPath = GameValues.Path + "Brin/";
            this.name = name;
            LoadRoom();
        }

        public void LoadRoom()
        {
            // Load
            XDocument doc = XDocument.Load(AreaPath + name + ".xml");
            name = doc.Descendants("roomname").Single().Value;
            SongName = doc.Descendants("songname").Single().Value;
            width = Int32.Parse(doc.Descendants("roomwidth").Single().Value);
            height = Int32.Parse(doc.Descendants("roomheight").Single().Value);

            zones = new List<Zone>();
            Zone newZone;
            foreach (XElement elm in doc.Descendants("zone"))
            {
                int x1 = Int32.Parse(elm.Descendants("x1").Single().Value);
                int x2 = Int32.Parse(elm.Descendants("x2").Single().Value);
                int y1 = Int32.Parse(elm.Descendants("y1").Single().Value);
                int y2 = Int32.Parse(elm.Descendants("y2").Single().Value);
                Dictionary<string, string> args = new Dictionary<string, string>();
                foreach (XElement arg in elm.Descendants("args").Descendants())
                {
                    args.Add(arg.Name.LocalName, arg.Value);
                }
                // TODO FIX THIS STUFF 
                newZone = (Zone)Activator.CreateInstance(Zone.types["Transition"], new object[]
                    {x1 * GameValues.tileDim, x2 * GameValues.tileDim - 1, y1 * GameValues.tileDim, y2 * GameValues.tileDim - 1, args});
                //{3 * GameValues.tileDim, 4 * GameValues.tileDim - 1, 7 * GameValues.tileDim, 10 * GameValues.tileDim - 1, 1, 1, 1, new Vector2(-1, 0)});
                zones.Add(newZone);
            }
            foreach (XElement elm in doc.Descendants("entity"))
            {
                int x = Int32.Parse(elm.Descendants("x").Single().Value);
                int y = Int32.Parse(elm.Descendants("y").Single().Value);
                string critter = elm.Descendants("critter").Single().Value;
                Entity newEntity = (Entity)Activator.CreateInstance(Entity.types[critter], new object[] { });
                newEntity.SetPosition(new Vector2(x, y));
                newEntity.SetCritter(CritterManager.Instance.GetItem(critter));
                defaultEntities.Add(newEntity);
            }

            layers = new ObservableCollection<Layer>();
            foreach (XElement elm in doc.Descendants("layers").Single().Descendants("layer"))
            {
                string text = elm.Descendants("tiles").Single().Value;
                string[] delimiters = { " ", "\r\n", "\n", "\t" };
                string[] tileArray = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                int layerHeight = Int32.Parse(elm.Descendants("height").Single().Value);
                int layerWidth = Int32.Parse(elm.Descendants("width").Single().Value);
                int layerDepth = Int32.Parse(elm.Descendants("depth").Single().Value);
                float layerScale = float.Parse(elm.Descendants("scale").Single().Value);
                string layerName = elm.Descendants("name").Single().Value;
                layers.Add(new Layer(tileArray, layerWidth, layerHeight, layerDepth, layerScale, layerName));
            }

            //layers = new List<Layer>();
            XElement colElm = doc.Descendants("collision").Single();
            string colText = colElm.Descendants("tiles").Single().Value;
            string[] colDelimiters = { " ", "\r\n", "\n", "\t" };
            string[] colTileArray = colText.Split(colDelimiters, StringSplitOptions.None);
            collisionLayer = new Collision(colTileArray, Int32.Parse(colElm.Descendants("width").Single().Value), Int32.Parse(colElm.Descendants("height").Single().Value));
        }

        public void NewLoadRoom()
        {

        }

        public void SaveRoom()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("room");
            root.Add(new XElement("roomname", name));
            root.Add(new XElement("songname", SongName));
            root.Add(new XElement("roomwidth", width));
            root.Add(new XElement("roomheight", height));
            XElement collision = new XElement("collision");
            collision.Add(new XElement("width", collisionLayer.width));
            collision.Add(new XElement("height", collisionLayer.height));
            collision.Add(new XElement("tiles", collisionLayer.LayerTileString()));
            root.Add(collision);

            // Add Layers
            XElement layersElm = new XElement("layers");
            foreach (Layer layer in layers)
            {
                XElement layerElm = new XElement("layer");
                layerElm.Add(new XElement("name", layer.name));
                layerElm.Add(new XElement("height", layer.height));
                layerElm.Add(new XElement("width", layer.width));
                layerElm.Add(new XElement("depth", layer.depth / GameValues.tileDim));
                layerElm.Add(new XElement("tiles", layer.LayerTileString()));
                layersElm.Add(layerElm);
            }
            root.Add(layersElm);

            // Add Zones
            XElement zonesElm = new XElement("zones");
            foreach (Zone zone in zones)
            {
                XElement zoneElm = new XElement("zone");
                zoneElm.Add(new XElement("x1", zone.boundary.Left / GameValues.tileDim));
                zoneElm.Add(new XElement("x2", (zone.boundary.Right + 1) / GameValues.tileDim));
                zoneElm.Add(new XElement("y1", zone.boundary.Top / GameValues.tileDim));
                zoneElm.Add(new XElement("y2", (zone.boundary.Bottom + 1) / GameValues.tileDim));
                zoneElm.Add(new XElement("type", zone.type));
                XElement args = new XElement("args");
                if (zone.args != null)
                {
                    foreach (string key in zone.args.Keys)
                    {
                        args.Add(new XElement(key, zone.args[key].ToString()));
                    }
                }
                zoneElm.Add(args);
                zonesElm.Add(zoneElm);
            }
            root.Add(zonesElm);

            // Add Entities
            XElement entitiesElm = new XElement("entities");
            foreach (Entity entity in defaultEntities)
            {
                XElement entityElm = new XElement("entity");
                entityElm.Add(new XElement("x", entity.position.X));
                entityElm.Add(new XElement("y", entity.position.Y));
                entityElm.Add(new XElement("critter", entity.critter.name));
                entitiesElm.Add(entityElm);
            }
            root.Add(entitiesElm);

            doc.Add(root);
            doc.Save(AreaPath + name + ".xml");
        }

        public Tile[] GetTileRow(int y)
        {
            Tile[] row = new Tile[width];
            for (int i = 0; i < width; i++)
            {
                row[i] = collisionLayer.tiles[i, y];
            }
            return row;
        }

        public Tile[,] GetTileRows(int top, int bottom)
        {
            // invery x and y for calculation logic?
            int yDiff = bottom - top;
            Tile[,] rows = new Tile[yDiff, width];
            for (int j = 0; j < yDiff; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (i < 0 || i >= collisionLayer.tiles.GetLength(0) || j + top < 0 || j + top >= collisionLayer.tiles.GetLength(1))
                    {
                        rows[j, i] = new Tile(0, 0, Color.Black, Tile.Type.WALL, 0, "1");
                        continue;
                    }
                    rows[j, i] = collisionLayer.tiles[i, j + top];
                }
            }
            return rows;
        }

        public Tile[] GetTileColumn(int x)
        {
            Tile[] column = new Tile[height];
            for (int j = 0; j < height; j++)
            {
                column[j] = collisionLayer.tiles[x, j];
            }
            return column;
        }

        public Tile[,] GetTileColumns(int left, int right)
        {
            int xDiff = right - left;
            Tile[,] columns = new Tile[xDiff, height];
            for (int i = 0; i < xDiff; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i + left < 0 || i + left >= collisionLayer.tiles.GetLength(0) || j < 0 || j >= collisionLayer.tiles.GetLength(1))
                    {
                        columns[i, j] = new Tile(0, 0, Color.Black, Tile.Type.WALL, 0, "1");
                        continue;
                    }
                    columns[i, j] = collisionLayer.tiles[i + left, j];
                }
            }
            return columns;
        }

        public Tile[,] GetTileCluster(int xLeft, int xRight, int yTop, int yBottom)
        {
            int xDiff = xRight - xLeft;
            int yDiff = yBottom - yTop;
            Tile[,] cluster = new Tile[xDiff, yDiff];
            for (int i = 0; i < xDiff; i++)
            {
                if (i + xLeft < 0 || i + xLeft >= this.width)
                    continue;
                for (int j = 0; j < yDiff; j++)
                {
                    if (j + yTop + 1 < 0 || j + yTop + 1 >= this.height)
                        continue;
                    cluster[i, j] = collisionLayer.tiles[i + xLeft, j + yTop + 1];
                    collisionLayer.tiles[i + xLeft, j + yTop + 1].isChecked = true;
                }
            }
            return cluster;
        }

        // TODO: Check implementation when adding functionality
        public void GrowRoomSize(Vector2 increment)
        {
            int oldWidth = width;
            int oldHeight = height;
            width += (int)Math.Abs(increment.X);
            height += (int)Math.Abs(increment.Y);
            int xInc = (increment.X > 0) ? 0 : (int)increment.X;
            int yInc = (increment.Y > 0) ? 0 : (int)increment.Y;
            foreach (Layer layer in layers)
            {
                string[] tileArray = new string[width * height];
                // Zero Array
                for (int j = 0; j < width; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (i + xInc < 0 || i + xInc >= oldWidth || j + yInc < 0 || j + yInc >= oldHeight)
                            tileArray[i + j * width] = "0";
                        else
                            tileArray[i + j * width] = layer.tileArray[(i + xInc) + (j + yInc) * width];
                    }
                }
            }
        }

        public void ShrinkRoomSize(Vector2 increment)
        {
            int oldWidth = width;
            int oldHeight = height;
            width -= (int)Math.Abs(increment.X);
            height -= (int)Math.Abs(increment.Y);
            int xInc = (increment.X > 0) ? 0 : -(int)increment.X;
            int yInc = (increment.Y > 0) ? 0 : -(int)increment.Y;
            foreach (Layer layer in layers)
            {
                string[] tileArray = new string[width * height];

                // Zero Array
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        tileArray[i + j * width] = layer.tileArray[i + xInc + (j + yInc) * width];
                    }
                }

            }
        }

        //public void Draw(int xLeft, int xRight, int yTop, int yBottom, SpriteBatch spriteBatch, Camera cam, bool renderCollision, bool renderZones)
        //{
        //    Vector2 dim = new Vector2(this.width, this.height);
        //    for (int h = 0; h < layers.Count; h++)
        //    {
        //        layers[h].Draw(xLeft, xRight, yTop, yBottom, spriteBatch, cam, false, dim);
        //    }
        //    if (renderCollision)
        //    {
        //        collisionLayer.Draw(xLeft, xRight, yTop, yBottom, spriteBatch, cam, true, dim);
        //    }
        //    if (renderZones)
        //    {
        //        foreach (Zone zone in zones)
        //        {
        //            zone.Draw(spriteBatch);
        //        }
        //    }
        //}
        public void Draw(int xLeft, int xRight, int yTop, int yBottom, SpriteBatch spriteBatch, OrthographicCamera cam, bool renderCollision, bool renderZones)
        {
            Vector2 dim = new Vector2(this.width, this.height);
            for (int h = 0; h < layers.Count; h++)
            {
                layers[h].Draw(xLeft, xRight, yTop, yBottom, spriteBatch, cam, false, dim);
            }
            if (renderCollision)
            {
                collisionLayer.Draw(xLeft, xRight, yTop, yBottom, spriteBatch, cam, true, dim);
            }
            if (renderZones)
            {
                foreach (Zone zone in zones)
                {
                    zone.Draw(spriteBatch);
                }
            }
        }

        public void ChangeProperties(string newName, int newHeight, int newWidth, string newSong)
        {
            if (name != newName)
            {
                RoomManager.Instance.UpdateCurrentRoomName(newName);
                name = newName;
            }
            height = newHeight;
            width = newWidth;
            if (SongManager.Instance.GetItem(newSong) != null)
                SongName = newSong;
            SongManager.Instance.PlaySong(SongName);
        }

        public void Draw(SpriteBatch spriteBatch, OrthographicCamera cam, bool renderCollision, bool renderZones)
        {
            Vector2 dim = new Vector2(this.width, this.height);
            for (int h = layers.Count - 1; h >= 0; h--)
            {
                float scale = layers[h].scale;
                //cam.
                //cam.Zoom = scale;
                RectangleF vis = cam.BoundingRectangle;
                //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.TransformMatrix(new Vector2((float)layers[h].width / (float)width, (float)layers[h].height / (float)height)));
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.GetViewMatrix());
                //layers[h].Draw(vis.X / GameValues.tileDim, (vis.X + vis.Width) / GameValues.tileDim, vis.Y / GameValues.tileDim, (vis.Y + vis.Height) / GameValues.tileDim, spriteBatch, cam, false, dim);
                layers[h].Draw((int)(vis.Left/GameValues.tileDim), (int)vis.Right / GameValues.tileDim, (int)vis.Top / GameValues.tileDim, (int)vis.Bottom / GameValues.tileDim, spriteBatch, cam, false, dim);
                spriteBatch.End();
            }
        }

        public void DrawEngineData(SpriteBatch spriteBatch, OrthographicCamera cam, bool renderCollision, bool renderZones, bool renderGrid)
        {
            Vector2 dim = new Vector2(this.width, this.height);
            RectangleF vis = cam.BoundingRectangle;

            if (renderCollision)
                collisionLayer.Draw((int)(vis.Left / GameValues.tileDim), (int)vis.Right / GameValues.tileDim, (int)vis.Top / GameValues.tileDim, (int)vis.Bottom / GameValues.tileDim, spriteBatch, cam, true, dim);
            //collisionLayer.Draw(vis.X / GameValues.tileDim, (vis.X + vis.Width) / GameValues.tileDim, vis.Y / GameValues.tileDim, (vis.Y + vis.Height) / GameValues.tileDim, spriteBatch, cam, true, dim);
            if (renderZones)
            {
                foreach (Zone zone in zones)
                {
                    zone.Draw(spriteBatch);
                }
            }
            if (renderGrid)
            {
                for (int i = 0; i < GameValues.TiledWidth + 1; i++)
                    spriteBatch.Draw(GameValues.pixel, new Rectangle(i * GameValues.tileDim + (int)cam.Position.X - (int)cam.Position.X % GameValues.tileDim, (int)cam.Position.Y, 1, GameValues.screenHeight), new Color(255, 0, 255));
                for (int j = 0; j < GameValues.TiledHeight + 1; j++)
                    spriteBatch.Draw(GameValues.pixel, new Rectangle((int)cam.Position.X, j * GameValues.tileDim + (int)cam.Position.Y - (int)cam.Position.Y % GameValues.tileDim, GameValues.screenWidth, 1), new Color(255, 0, 255));
            }
        }

        // Layer Functions
        public void AddLayer(int index)
        {
            Layer newLayer = new Layer(width, height);
            newLayer.name = "test" + layers.Count;

            layers.Insert(index, newLayer);
        }

        public void DuplicateLayer(int index)
        {
            Layer newLayer = new Layer();
            newLayer.name = "test" + layers.Count;

            layers.Insert(index, newLayer);
        }

        public bool MoveLayerDown(int index)
        {
            if (index < layers.Count)
            {
                layers.Move(index, index + 1);
                return true;
            }
            return false;
        }

        public bool MoveLayerUp(int index)
        {
            if (index >= 0)
            {
                layers.Move(index, index - 1);
                return true;
            }
            return false;
        }

        public void DeleteLayer(int index)
        {
            layers.Remove(layers[index]);
        }
    }
}
