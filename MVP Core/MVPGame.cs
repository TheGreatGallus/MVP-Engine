using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MVP_Core.Components;
using MVP_Core.Entities;
using MVP_Core.Events;
using MVP_Core.Global;
using MVP_Core.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MVP_Core
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MVPGame //: Game
    {
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }
        private GraphicsDeviceManager graphics;

        private GraphicsDevice GraphicsDevice
        {
            get { return (graphics != null) ? graphics.GraphicsDevice : null; }
        }


        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }
        private SpriteBatch spriteBatch;

        public RenderTarget2D RenderTarget
        {
            get { return renderTarget; }
            set { renderTarget = value; }
        }
        private RenderTarget2D renderTarget;

        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;

        //public Camera cam;
        public OrthographicCamera camera;
        float torchValue = 0.0f;
        public Random random = new Random();
        public static List<Critter> critters = new List<Critter>();
        public RenderTarget2D localRenderTarget;
        public ObservableCollection<string> roomList;
        public ObservableCollection<string> zoneTypes;
        public bool showGrid = false;
        public bool showAABBs = false;
        Action<int> func;
        public bool renderCollisions;
        public bool renderZones;
        public bool renderGrid;

        public MVPGame()
        {
            ComponentManager.Instance.entities = new List<Entity>();
            Entity player = new Entity(0);
            player.SetDimensions(new Vector2(16, 28));
            ComponentManager.Instance.entities.Add(player);

            ComponentManager.Instance.RegisterItem("UC0", new UpdateComponent(0) { IsPlayerComponent = true });
            ComponentManager.Instance.RegisterItem("RC0", new RenderComponent(0) { IsPlayerComponent = true });

            for (int i = 1; i < 4; i++)
            {
                ComponentManager.Instance.RegisterItem("UC" + i, new UpdateComponent(i));
                ComponentManager.Instance.RegisterItem("RC" + i, new RenderComponent(i));
            }
        }

        public MVPGame(int width, int height)
        {
            //graphics.PreferredBackBufferWidth = width;
            //graphics.PreferredBackBufferHeight = height;
            //graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            // TODO: Add your initialization logic here

            //base.Initialize();
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferMultiSampling = true;
            //cam = new Camera(GraphicsDevice.Viewport);
            //var viewportAdapter = new BoxingViewportAdapter(this.Gra, GraphicsDevice, GameValues.screenWidth, GameValues.screenHeight);
            //var viewportAdapter = new BoxingViewportAdapter(GraphicsDevice);
            camera = new OrthographicCamera(GraphicsDevice);
            //cam.Location = new Vector2(0, 0);

            MVPScripts.SetGame(this);


            //renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight,
            //    false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            localRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, GameValues.ScaledWidth, GameValues.ScaledHeight);

            RoomManager.Instance.ChangeRoom(1);

            GameValues.pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            GameValues.pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it
            //GameValues.Initialize()

            SoundEffectManager.Instance.Initialize();

            RoomManager.Instance.RoomChanged += HandleRoomChanged;
        }

        // TODO: Move and update
        void HandleRoomChanged(Room room)
        {
            //activeTool.switchRooms(RoomManager.Instance.room);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent(ContentManager Content)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight,
            //    false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            LoadShaders();
            LoadAudio(Content);
            //LoadScripts();
            LoadInputs();
            //LoadCritters();
            LoadRooms();
            LoadTextures();
            LoadAnimations();

            foreach (RenderComponent rc in ComponentManager.Instance.RenderComponents())
            {
                rc.SetTexture("TestCharacterSprite");
                //rc.SetAnimationState("StandDemo");
                rc.Activate();
            }

            RoomManager.Instance.ChangeRoom(1);
            //// Create a new SpriteBatch, which can be used to draw textures.
            ////spriteBatch = new SpriteBatch(GraphicsDevice);

            //// TODO: use this.Content to load your game content here
            //LoadShaders();
            //LoadAudio();
            ////LoadScripts();
            //LoadInputs();
            //LoadCritters();
            //LoadRooms();
            //LoadTextures();

            //foreach (RenderComponent rc in ComponentManager.Instance.RenderComponents())
            //{
            //    rc.SetTexture("saabb");
            //    rc.Activate();
            //}
        }

        public void LoadTextures()
        {
            string filepath = GameValues.Path + "Sprites";
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.png"))
            {
                string fullPath = filepath + "/" + file.Name;
                string spriteName = Path.GetFileNameWithoutExtension(fullPath);
                FileStream fileStream = new FileStream(fullPath, FileMode.Open);
                Texture2D texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(GraphicsDevice, fileStream);
                TextureManager.Instance.RegisterItem(spriteName, texture);
                fileStream.Dispose();

                //if (spriteName == "TestTileSet")
                //{
                //    TextureManager.Instance.RegisterTileset(0, spriteName);
                //}
            }
            LoadTilesets();


            // TODO: RELOCATE

            Tile.colTileType = TextureManager.Instance.GetItem("CollisionTiles");
            Tile.sourceRects = new Dictionary<Tile.Type, Rectangle>();
            Tile.sourceRects.Add(Tile.Type.WALL, new Rectangle(224, 0, 16, 16));
            Tile.sourceRects.Add(Tile.Type.Y_ONE_WAY, new Rectangle(0, 0, 16, 16));

            Tile.colSourceRects = new Dictionary<Tile.Type, Rectangle>();
            Tile.colSourceRects.Add(Tile.Type.WALL, new Rectangle(224, 0, 16, 16));
            Tile.colSourceRects.Add(Tile.Type.Y_ONE_WAY, new Rectangle(0, 0, 16, 16));
        }

        public void LoadTilesets()
        {
            string filepath = GameValues.Path + "Tilesets";
            //DirectoryInfo d = new DirectoryInfo(filepath);
            XDocument doc = XDocument.Load(GameValues.Path + "Tilesets/Tilesets.xml");
            foreach (XElement elm in doc.Descendants("tileset"))
            {
                string fileName = elm.Descendants("fileName").Single().Value;
                int tilesetNum = Int32.Parse(elm.Descendants("tilesetNum").Single().Value);
                string fullPath = filepath + "/" + fileName;
                string spriteName = Path.GetFileNameWithoutExtension(fullPath);

                FileStream fileStream = new FileStream(fullPath, FileMode.Open);
                Texture2D texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(GraphicsDevice, fileStream);
                TextureManager.Instance.RegisterItem(spriteName, texture);
                fileStream.Dispose();
                TextureManager.Instance.RegisterTileset(tilesetNum, spriteName);
            }
        }

        public void LoadAnimations()
        {
            XDocument doc = XDocument.Load(GameValues.Path + "Sprites/Animations.xml");
            foreach (XElement elm in doc.Descendants("animation"))
            {
                string animationName = elm.Descendants("name").Single().Value;
                int animationEndFrame = Int32.Parse(elm.Descendants("endFrame").Single().Value);
                Dictionary<int, Frame> newAnimationFrames = new Dictionary<int, Frame>();
                Animation newAnimation = new Animation();
                newAnimation.endFrame = animationEndFrame;
                newAnimation.name = animationName;
                foreach (XElement frame in elm.Descendants("frame"))
                {
                    string spriteName = frame.Descendants("spriteName").Single().Value;
                    int frameCount = Int32.Parse(frame.Descendants("frameCount").Single().Value);
                    int x = Int32.Parse(frame.Descendants("x").Single().Value);
                    int y = Int32.Parse(frame.Descendants("y").Single().Value);
                    int width = Int32.Parse(frame.Descendants("width").Single().Value);
                    int height = Int32.Parse(frame.Descendants("height").Single().Value);
                    Rectangle frameRectangle = new Rectangle(x, y, width, height);
                    Frame newFrame = new Frame(spriteName, frameCount, frameRectangle);
                    newAnimationFrames.Add(frameCount, newFrame);
                }
                newAnimation.Frames = newAnimationFrames;
                AnimationManager.Instance.RegisterItem(animationName, newAnimation);
            }
        }


        public void SetRoomList()
        {
            roomList = new ObservableCollection<string>();
            foreach (string name in RoomManager.Instance.GetRoomNames())
            {
                roomList.Add(name);
            }
        }

        public void LoadRooms()
        {
            try
            {
                XDocument doc = XDocument.Load(GameValues.Path + "Brin/Manifest.xml");
                foreach (XElement elm in doc.Descendants("room"))
                {
                    string roomName = elm.Descendants("name").Single().Value;
                    Room newRoom = new Room(roomName);
                    RoomManager.Instance.RegisterItem(newRoom.name, newRoom);
                }
                SetRoomList();
            }
            catch (Exception e)
            {

            }
        }

        //public void LoadCritters()
        //{
        //    XDocument doc = XDocument.Load(GameValues.Path + "Brin/Enemy.xml");
        //    foreach (XElement elm in doc.Descendants("critter"))
        //    {
        //        string type = elm.Descendants("type").Single().Value;
        //        int width = Int32.Parse(elm.Descendants("width").Single().Value);
        //        int height = Int32.Parse(elm.Descendants("height").Single().Value);
        //        Tuple<Type, Action<int>> scriptData = ScriptManager.Instance.GetItem(type);
        //        Critter newCritter = new Critter(scriptData.Item1, scriptData.Item2, new Vector2(width, height), null);
        //        CritterManager.Instance.RegisterItem(type, newCritter);
        //        //critters.Add(newCritter);
        //    }
        //}

        public void LoadAudio(ContentManager content)
        {
            var soundEffectPath = GameValues.Path + "Audio/Sounds/";
            XDocument config = XDocument.Load("D:\\Users\\The Great Gallus\\Documents\\MVPDemo\\config.xml");
            XElement gameValues = config.Descendants("soundListings").SingleOrDefault();
            foreach (var value in gameValues.Descendants())
            {
                FileStream fileStream = new FileStream(soundEffectPath + value.Value + ".wav", FileMode.Open);
                SoundEffect effect = Microsoft.Xna.Framework.Audio.SoundEffect.FromStream(fileStream);
                SoundEffectManager.Instance.RegisterItem(value.Name.LocalName, effect);
            }

            // TODO: FIX 
            // Song Registry 
            var musicPath = @"" + GameValues.Path + "Audio/Music/";
            DirectoryInfo d = new DirectoryInfo(musicPath);
            var songEn = Directory.EnumerateFiles(musicPath, "*.ogg");
            foreach(string filename in songEn)
            {
                //FileStream fileStream = new FileStream(filename, FileMode.Open);
                //SoundEffect song = SoundEffect.FromStream(fileStream);
                //OpenAL.AL.
                Uri baseUri = new Uri(@"" + Directory.GetCurrentDirectory()+"/Content");
                Uri testUri = new Uri(@"" + filename);
                Uri relativeUri = baseUri.MakeRelativeUri(testUri);
                Song song = Song.FromUri(Path.GetFileNameWithoutExtension(filename), relativeUri);
                SongManager.Instance.RegisterItem(song.Name, song);
            }
            //var songE = songEn.Select(file => Song.FromUri(file, new Uri(file, UriKind.Relative)));
            //var songs = songE.ToList();

            //foreach (Song song in songs)
            //{
            //    SongManager.Instance.RegisterItem(song.Name, song);
            //}
            //Song.FromUri("test", Uri)

            //Song song = Song.FromUri("RedBrin", new Uri(musicPath + "RedBrin.mp3", UriKind.Relative));
            //SongManager.Instance.RegisterItem(song.Name, song);
        }

        public void LoadInputs()
        {
            var soundEffectPath = GameValues.Path + "Audio/Sounds/";
            XDocument config = XDocument.Load("D:\\Users\\The Great Gallus\\Documents\\MVPDemo\\config.xml");
            XElement gameValues = config.Descendants("keyFunctions").SingleOrDefault();
            foreach (var value in gameValues.Descendants())
            {
                Keys key = (Keys)System.Enum.Parse(typeof(Keys), value.Name.LocalName);
                InputActionEvent.Actions action = (InputActionEvent.Actions)System.Enum.Parse(typeof(InputActionEvent.Actions),
                    value.Value);
                InputManager.RegisterInput(key, new InputActionEvent(action));
            }
        }

        public void LoadShaders()
        {
            BasicEffect defaultEffect = new BasicEffect(GraphicsDevice);
            EffectManager.Instance.RegisterItem("Default", defaultEffect);
            string filepath = GameValues.Path + "Shaders";
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.fx"))
            {
                string strCmdText;
                string fullPath = GameValues.Path + "Shaders/" + file.Name;
                string effectName = Path.GetFileNameWithoutExtension(fullPath);
                string newFullPath = GameValues.Path + "Shaders/Compiled/" + effectName + ".mgfxo";
                strCmdText = "\"C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/2MGFX.exe\" \"" + fullPath + "\" \"" + newFullPath + "\" /Profile:DirectX_11";
                using (Process exeProcess = new Process())
                {
                    exeProcess.StartInfo.UseShellExecute = false;
                    exeProcess.StartInfo.RedirectStandardInput = true;
                    exeProcess.StartInfo.RedirectStandardOutput = true;
                    exeProcess.StartInfo.CreateNoWindow = true;
                    exeProcess.StartInfo.FileName = "CMD.exe";
                    exeProcess.Start();
                    exeProcess.StandardInput.WriteLine(strCmdText);
                    exeProcess.StandardInput.Flush();
                    exeProcess.StandardInput.Close();
                    exeProcess.WaitForExit();
                    Console.WriteLine(exeProcess.StandardOutput.ReadToEnd());
                }
                try
                {
                    Effect compiledEffect = new Effect(GraphicsDevice, (byte[])File.ReadAllBytes(newFullPath));
                    EffectManager.Instance.RegisterItem(effectName, compiledEffect);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        //public void LoadScripts()
        //{
        //    try
        //    {
        //        ScriptManager.Instance.ClearItems();
        //        LoadOtherScripts();
        //        LoadEntityScripts();
        //        LoadZoneScripts();
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

        //private void LoadEntityScripts()
        //{
        //    string filepath = GameValues.Path + "Scripts/Entities";
        //    DirectoryInfo d = new DirectoryInfo(filepath);
        //    Entity.types.Clear();

        //    foreach (var file in d.GetFiles("*.cs"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string entityTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //        Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //        Type entityType = newAssembly.GetType(entityTypeName);
        //        var updateMethod = entityType.GetMethod("update");
        //        Action<int> updateFunc = (Action<int>)updateMethod.CreateDelegate(typeof(Action<int>));
        //        ScriptManager.Instance.RegisterItem(entityTypeName, new Tuple<Type, Action<int>>(entityType, updateFunc));
        //        Entity.types.Add(entityTypeName, entityType);
        //    }
        //    LoadCritters();

        //    if (ComponentManager.Instance.entities.Count > 2)
        //    {
        //        foreach (Entity entity in ComponentManager.Instance.entities.Where(e => e.id > 1).ToList())
        //        {
        //            entity.SetCritter(CritterManager.Instance.GetItem(entity.critterame));
        //            UpdateComponent uc = ComponentManager.Instance.GetComponentByEntityId(entity.id, typeof(UpdateComponent)) as UpdateComponent;
        //            uc.SetUpdate(entity.critter.updateAction);
        //        }
        //    }
        //}

        //private void LoadZoneScripts()
        //{
        //    string filepath = GameValues.Path + "Scripts/Zones";
        //    DirectoryInfo d = new DirectoryInfo(filepath);
        //    Zone.types.Clear();
        //    zoneTypes.Clear();

        //    foreach (var file in d.GetFiles("*.cs"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string zoneTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //        Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //        var zoneType = newAssembly.GetType(zoneTypeName);
        //        Zone.types.Add(zoneTypeName, zoneType);
        //        zoneTypes.Add(zoneTypeName);
        //    }
        //}

        //private void LoadOtherScripts()
        //{
        //    string filepath = GameValues.Path + "Scripts/Other";
        //    DirectoryInfo d = new DirectoryInfo(filepath);

        //    foreach (var file in d.GetFiles("*.cs"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string otherTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //        Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //        var type = newAssembly.GetType(otherTypeName);
        //        var method = type.GetMethod("update");
        //        func = (Action<int>)method.CreateDelegate(typeof(Action<int>));
        //        int id = (otherTypeName == "TestScripts") ? 0 : 1;
        //        UpdateComponent playerUC = ComponentManager.Instance.GetComponentByEntityId(id, typeof(UpdateComponent)) as UpdateComponent;
        //        playerUC.SetUpdate(func);
        //        playerUC.SetType(type);
        //        playerUC.Activate();
        //    }
        //    //string filepath = "Content/Scripts/Other";
        //    //DirectoryInfo d = new DirectoryInfo(filepath);

        //    //foreach (var file in d.GetFiles("*.cs"))
        //    //{
        //    //    string fullPath = filepath + "/" + file.Name;
        //    //    string zoneTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //    //    Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //    //}
        //}


        //public void LoadTextures()
        //{
        //    string filepath = GameValues.Path + "Sprites";
        //    DirectoryInfo d = new DirectoryInfo(filepath);

        //    foreach (var file in d.GetFiles("*.png"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string spriteName = Path.GetFileNameWithoutExtension(fullPath);
        //        FileStream fileStream = new FileStream(fullPath, FileMode.Open);
        //        Texture2D texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(GraphicsDevice, fileStream);
        //        TextureManager.Instance.RegisterItem(spriteName, texture);
        //        fileStream.Dispose();
        //    }

        //    // TODO: RELOCATE
        //    Tile.sourceRects = new Dictionary<Tile.Type, Rectangle>();
        //    Tile.sourceRects.Add(Tile.Type.WALL, new Rectangle(224, 0, 16, 16));
        //    Tile.sourceRects.Add(Tile.Type.Y_ONE_WAY, new Rectangle(0, 0, 16, 16));

        //    Tile.colSourceRects = new Dictionary<Tile.Type, Rectangle>();
        //    Tile.colSourceRects.Add(Tile.Type.WALL, new Rectangle(224, 0, 16, 16));
        //    Tile.colSourceRects.Add(Tile.Type.Y_ONE_WAY, new Rectangle(0, 0, 16, 16));
        //}

        //public void LoadRooms()
        //{
        //    try
        //    {
        //        XDocument doc = XDocument.Load(GameValues.Path + "Brin/Manifest.xml");
        //        Room newRoom = new Room(1);
        //        List<Zone> zones = new List<Zone>();
        //        Zone newZone;
        //        foreach (XElement elm in doc.Descendants("zone"))
        //        {
        //            int x1 = Int32.Parse(elm.Descendants("x1").Single().Value);
        //            int x2 = Int32.Parse(elm.Descendants("x2").Single().Value);
        //            int y1 = Int32.Parse(elm.Descendants("y1").Single().Value);
        //            int y2 = Int32.Parse(elm.Descendants("y2").Single().Value);
        //            Dictionary<string, string> args = new Dictionary<string, string>();
        //            foreach (XElement arg in elm.Descendants("args").Descendants())
        //            {
        //                args.Add(arg.Name.LocalName, arg.Value);
        //            }
        //            newZone = (Zone)Activator.CreateInstance(Zone.types["Transition"], new object[]
        //                // TODO FIX THIS SHIT
        //                {x1 * GameValues.tileDim, x2 * GameValues.tileDim - 1, y1 * GameValues.tileDim, y2 * GameValues.tileDim - 1, args});
        //            //{3 * GameValues.tileDim, 4 * GameValues.tileDim - 1, 7 * GameValues.tileDim, 10 * GameValues.tileDim - 1, 1, 1, 1, new Vector2(-1, 0)});
        //            zones.Add(newZone);
        //            newRoom.zones = zones;
        //        }
        //        foreach (XElement elm in doc.Descendants("entity"))
        //        {
        //            int x = Int32.Parse(elm.Descendants("x").Single().Value);
        //            int y = Int32.Parse(elm.Descendants("y").Single().Value);
        //            string critter = elm.Descendants("critter").Single().Value;
        //            Entity newEntity = new Entity();
        //            newEntity.SetPosition(new Vector2(x, y));
        //            newEntity.SetCritter(critters.Where(b => b.name == critter).First());
        //            newRoom.defaultEntities.Add(newEntity);
        //        }
        //        newRoom.SongName = "RedBrin";
        //        RoomManager.Instance.RegisterItem(newRoom.name, newRoom);
        //        //roomList = new ObservableCollection<string>();
        //        //roomList.Add(newRoom.name);
        //        //rooms.Add(1, newRoom);

        //        newRoom = new Room(19, 15, "2");
        //        zones = new List<Zone>();
        //        newZone = (Zone)Activator.CreateInstance(Zone.types["Transition"], new object[]
        //            {3 * GameValues.tileDim, 4 * GameValues.tileDim - 1, 7 * GameValues.tileDim, 10 * GameValues.tileDim - 1, 1, 1, 1, new Vector2(-1, 0)});
        //        zones.Add(newZone);
        //        newZone = (Zone)Activator.CreateInstance(Zone.types["Transition"], new object[]
        //            {7 * GameValues.tileDim, 10 * GameValues.tileDim - 1, 3 * GameValues.tileDim, 4 * GameValues.tileDim - 1, 2, 1, 2, new Vector2(0, -1)});
        //        zones.Add(newZone);
        //        newRoom.zones = zones;
        //        newRoom.SongName = "Temple";
        //        RoomManager.Instance.RegisterItem(newRoom.name, newRoom);
        //        //rooms.Add(2, newRoom);

        //        //roomList.Add(newRoom.name);
        //        //roomList = new ObservableCollection<int>();
        //        //roomList.Add(1);
        //        //roomList.Add(2);
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

        //public void LoadCritters()
        //{
        //    XDocument doc = XDocument.Load(GameValues.Path + "/Brin/Enemy.xml");
        //    foreach (XElement elm in doc.Descendants("critter"))
        //    {
        //        string type = elm.Descendants("type").Single().Value;
        //        int width = Int32.Parse(elm.Descendants("width").Single().Value);
        //        int height = Int32.Parse(elm.Descendants("height").Single().Value);
        //        Tuple<Type, Action<int>> scriptData = ScriptManager.Instance.GetItem(type);
        //        Critter newCritter = new Critter(scriptData.Item1, scriptData.Item2, new Vector2(width, height), );
        //        critters.Add(newCritter);
        //    }
        //}

        //public void LoadAudio()
        //{
        //    // TODO: FIX
        //    //var soundConfig = ConfigurationManager.GetSection("customSettings/soundListings") as NameValueCollection;
        //    //var soundEffectPath = GameValues.Path + "/Audio/Sounds/";
        //    //foreach (var collectionKey in soundConfig.AllKeys)
        //    //{
        //    //    FileStream fileStream = new FileStream(soundEffectPath + soundConfig.GetValues(collectionKey).FirstOrDefault() + ".wav", FileMode.Open);
        //    //    SoundEffect effect = Microsoft.Xna.Framework.Audio.SoundEffect.FromStream(fileStream);
        //    //    SoundEffectManager.Instance.RegisterItem(collectionKey, effect);
        //    //}

        //    // Song Registry 
        //    // TODO: CLEAN UP!
        //    //var musicPath = GameValues.Path + "/Audio/Music/";
        //    //DirectoryInfo d = new DirectoryInfo(musicPath);
        //    //var songEn = Directory.EnumerateFiles(musicPath, "*.mp3");
        //    //var songE = songEn.Select(file => Song.FromUri(file, new Uri(file, UriKind.Relative)));
        //    //var songs = songE.ToList();

        //    //foreach (Song song in songs)
        //    //{
        //    //    SongManager.Instance.RegisterItem(song.Name, song);
        //    //}
        //}

        //public void LoadInputs()
        //{
        //    //var inputConfig = ConfigurationManager.GetSection("customSettings/keyFunctions") as NameValueCollection;
        //    //foreach (var collectionKey in inputConfig.AllKeys)
        //    //{
        //        // DO NOT DELETE
        //        //Keys key = (Keys)System.Enum.Parse(typeof(Keys), collectionKey);
        //        //InputActionEvent.Actions action = (InputActionEvent.Actions)System.Enum.Parse(typeof(InputActionEvent.Actions),
        //        //    inputConfig.GetValues(collectionKey).FirstOrDefault());
        //        //InputManager.RegisterInput(key, new InputActionEvent(action));
        //    //}
        //}

        //public void LoadShaders()
        //{
        //    //string filepath = "Content/Shaders";
        //    string filepath = GameValues.Path + "\\Shaders";
        //    DirectoryInfo d = new DirectoryInfo(filepath);

        //    foreach (var file in d.GetFiles("*.fx"))
        //    {
        //        string strCmdText;
        //        //string fullPath = filepath + "/" + file.Name;
        //        string fullPath = GameValues.Path + "Shaders/" + file.Name;
        //        //string newFullPath = filepath + "/" + Path.GetFileNameWithoutExtension(fullPath) + ".mgfxo";
        //        string effectName = Path.GetFileNameWithoutExtension(fullPath);
        //        string newFullPath = GameValues.Path + "Shaders/Compiled/" + effectName + ".mgfxo";
        //        //strCmdText = "\"C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/2MGFX.exe\" \"" + fullPath + "\" \"" + newFullPath + "\" /Profile:OpenGL";
        //        strCmdText = "\"C:/Program Files (x86)/MSBuild/MonoGame/v3.0/Tools/2MGFX.exe\" \"" + fullPath + "\" \"" + newFullPath + "\" /Profile:DirectX_11";
        //        using (Process exeProcess = new Process())
        //        {
        //            exeProcess.StartInfo.UseShellExecute = false;
        //            exeProcess.StartInfo.RedirectStandardInput = true;
        //            exeProcess.StartInfo.RedirectStandardOutput = true;
        //            exeProcess.StartInfo.CreateNoWindow = true;
        //            exeProcess.StartInfo.FileName = "CMD.exe";
        //            exeProcess.Start();
        //            exeProcess.StandardInput.WriteLine(strCmdText);
        //            exeProcess.StandardInput.Flush();
        //            exeProcess.StandardInput.Close();
        //            exeProcess.WaitForExit();
        //            Console.WriteLine(exeProcess.StandardOutput.ReadToEnd());
        //        }
        //        try
        //        {
        //            Effect compiledEffect = new Effect(GraphicsDevice, (byte[])File.ReadAllBytes(newFullPath));
        //            EffectManager.Instance.RegisterItem(effectName, compiledEffect);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
        //    }

        //}

        //public void LoadScripts()
        //{
        //try
        //{
        //    Assembly assembly = CSharpScriptingEngine.Compile(File.ReadAllText("Content/Scripts/TestScripts.cs"));
        //    var type = assembly.GetType("TestScripts");
        //    var method = type.GetMethod("update");
        //    func = (Action<int>)method.CreateDelegate(typeof(Action<int>));
        //    UpdateComponent playerUC = ComponentManager.Instance.GetComponentByEntityId(0, typeof(UpdateComponent)) as UpdateComponent;
        //    playerUC.SetUpdate(func);
        //    playerUC.SetType(type);
        //    playerUC.Activate();

        //    string filepath = GameValues.Path + "Scripts";
        //    DirectoryInfo d = new DirectoryInfo(filepath);

        //    foreach (var file in d.GetFiles("*.cs"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string entityTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //        Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //        Type entityType = newAssembly.GetType(entityTypeName);
        //        var updateMethod = entityType.GetMethod("update");
        //        Action<int> updateFunc = (Action<int>)updateMethod.CreateDelegate(typeof(Action<int>));
        //        ScriptManager.Instance.RegisterItem(entityTypeName, new Tuple<Type, Action<int>>(entityType, updateFunc));
        //        if (entityType.IsSubclassOf(typeof(Zone)))
        //        {
        //            Zone.types.Add(entityTypeName, entityType);
        //        }

        //    }
        //}
        //catch (Exception e)
        //{

        //}

        //LoadEntityScripts();
        //LoadZoneScripts();
        //}

        //private void LoadEntityScripts()
        //{
        //    string filepath = "Content/Scripts/Entities";
        //    DirectoryInfo d = new DirectoryInfo(filepath);

        //    foreach (var file in d.GetFiles("*.cs"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string entityTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //        Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //        Type entityType = newAssembly.GetType(entityTypeName);
        //        var updateMethod = entityType.GetMethod("update");
        //        Action<int> updateFunc = (Action<int>)updateMethod.CreateDelegate(typeof(Action<int>));
        //        ScriptManager.Instance.RegisterItem(entityTypeName, new Tuple<Type, Action<int>>(entityType, updateFunc));

        //    }
        //}

        //private void LoadZoneScripts()
        //{
        //    string filepath = "Content/Scripts/Zones";
        //    DirectoryInfo d = new DirectoryInfo(filepath);

        //    foreach (var file in d.GetFiles("*.cs"))
        //    {
        //        string fullPath = filepath + "/" + file.Name;
        //        string zoneTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //        Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
        //        var zoneType = newAssembly.GetType(zoneTypeName);
        //        Zone.types.Add(zoneTypeName, zoneType);
        //    }
        //}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            // TODO: Add your update logic here
            if (RoomManager.Instance.room == null)
                return;

            InputManager.PollStateChanges();
            InputManager.RunKeyActions(ComponentManager.Instance.GetComponentByEntityId(0, typeof(UpdateComponent)) as UpdateComponent);

            foreach (UpdateComponent uc in ComponentManager.Instance.UpdateComponents().Where(uc => uc.Active))
            {
                uc.Update(uc.entityId);
            }

            Vector2 position = ComponentManager.Instance.entities.Where(e => e.id == 0).FirstOrDefault().position;
            Rectangle colli = new Rectangle((int)position.X, (int)position.Y, GameValues.tileDim, GameValues.tileDim * 2);
            foreach (Zone zone in RoomManager.Instance.GetZones())
            {
                zone.AttmeptCollide(colli);
            }

            SoundEffectManager.Instance.Update();


            // TODO: MOVE AND FIX THIS CAMERA CODE
            //if (cam.x < 0)
            //    cam.x = 0;
            //if (cam.x + (GameValues.ScaledWidth) > RoomManager.Instance.RoomWidth * GameValues.tileDim)
            //    cam.x = RoomManager.Instance.RoomWidth - (GameValues.ScaledWidth);
            //if (cam.y < 0)
            //    cam.y = 0;
            //if (cam.y + (GameValues.ScaledHeight) > RoomManager.Instance.RoomHeight)
            //    cam.y = RoomManager.Instance.RoomHeight * GameValues.tileDim - (GameValues.ScaledHeight);
            
            //base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            ComponentManager.Instance.UpdateComponents().Where(uc => uc.entityId == 1).FirstOrDefault().Update(1);

            GameValues.frame = (int)(gameTime.TotalGameTime.TotalMilliseconds / (1000 / 60.0f));

            //camera.Position = new Vector2(ComponentManager.Instance.entities[0].position.X - 100, ComponentManager.Instance.entities[0].position.Y - 100);
            //camera.Position = new Vector2(ComponentManager.Instance.entities[0].position.X - GameValues.ScaledWidth/2, ComponentManager.Instance.entities[0].position.Y - GameValues.ScaledHeight/2);
            if (camera.Position.X > RoomManager.Instance.RoomWidth - GameValues.ScaledWidth)
                camera.Position = new Vector2(RoomManager.Instance.RoomWidth - GameValues.ScaledWidth, camera.Position.Y);
            if (camera.Position.X < 0)
                camera.Position = new Vector2(0, camera.Position.Y);
            if (camera.Position.Y > RoomManager.Instance.RoomHeight - GameValues.ScaledHeight)
                camera.Position = new Vector2(camera.Position.X, RoomManager.Instance.RoomHeight - GameValues.ScaledHeight);
            if (camera.Position.Y < 0)
                camera.Position = new Vector2(camera.Position.X, 0);
            //camera.Position = ComponentManager.Instance.entities[0].position;
            var transformMatrix = camera.GetViewMatrix();

            //GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.SetRenderTarget(localRenderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (RoomManager.Instance.room == null)
                return;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            AnimationManager.Instance.DrawQueue(spriteBatch, true);
            TextureManager.Instance.DrawQueue(spriteBatch, true);
            spriteBatch.End();

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            //if (torchValue == 0.0f || random.Next(100) >= 75)
            //    torchValue = random.Next(50, 75) / 100.0f;
            torchValue = 1.0f;

            //EffectManager.Instance.ApplyEffect("Test");

            //List<Tuple<string, Type, object>> effectParameters = new List<Tuple<string, Type, object>>();
            //effectParameters.Add(new Tuple<string, Type, object>("World", typeof(Matrix), Matrix.Identity));
            //effectParameters.Add(new Tuple<string, Type, object>("View", typeof(Matrix), cam.TransformMatrix(Vector2.One)));
            //effectParameters.Add(new Tuple<string, Type, object>("Projection", typeof(Matrix), projection));
            //effectParameters.Add(new Tuple<string, Type, object>("lightCount", typeof(int), 2));
            //effectParameters.Add(new Tuple<string, Type, object>("DiffuseIntensity", typeof(float), torchValue));
            //effectParameters.Add(new Tuple<string, Type, object>("DiffuseLightPosition", typeof(Vector3[]), new Vector3[] {
            //    new Vector3(ComponentManager.Instance.entities[0].position.X, ComponentManager.Instance.entities[0].position.Y, -16.0f),
            //    new Vector3(320, 80, 144.0f)}));

            //EffectManager.Instance.ApplyParameters(effectParameters);

            //Rectangle camView = cam.VisibleArea;

            //RoomManager.Instance.room.Draw(spriteBatch, cam, true, true);
            RoomManager.Instance.room.Draw(spriteBatch, camera, true, true);
            //RoomManager.Instance.room.Draw(spriteBatch, cam, (MyGameEditor.activeTool is CollisionTool) ? true : false, (MyGameEditor.activeTool is ZoneTool) ? true : false);

            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.TransformMatrix(Vector2.One));
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.TransformMatrix(Vector2.One));
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformMatrix);

            foreach (RenderComponent rc in ComponentManager.Instance.RenderComponents().Where(rc => rc.IsActive))
            {
                Entity ent = ComponentManager.Instance.entities.Where(e => e.id == rc.entityId).FirstOrDefault();
                Vector2 pos = (ent == null) ? new Vector2(0.0f, 0.0f) : ent.position;
                rc.Draw(spriteBatch, pos);
            }

            DrawHUD(gameTime, spriteBatch);
            if (showAABBs)
            {
                foreach (Entity entity in ComponentManager.Instance.entities)
                {

                    spriteBatch.Draw(GameValues.pixel, new Rectangle((int)entity.position.X, (int)entity.position.Y, (int)entity.dimensions.X, (int)entity.dimensions.Y), Color.LimeGreen);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            AnimationManager.Instance.DrawQueue(spriteBatch, false);
            TextureManager.Instance.DrawQueue(spriteBatch, false);
            spriteBatch.End();

            // Collision And Zones
            // TODO: Generalize this
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetViewMatrix());
            RoomManager.Instance.room.DrawEngineData(spriteBatch, camera, renderCollisions, renderZones, renderGrid);
            //RoomManager.Instance.room.DrawEngineData(spriteBatch, cam, (MyGameEditor.activeTool is CollisionTool) ? true : false, (MyGameEditor.activeTool is ZoneTool) ? true : false, showGrid);

            spriteBatch.End();

            //Matrix projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
            //Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            ////if (torchValue == 0.0f || random.Next(100) >= 75)
            ////    torchValue = random.Next(50, 75) / 100.0f;

            ////SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.TransformMatrix(new Vector2(0f, 0f)));
            ////EffectManager.Instance.ApplyEffect("Test");

            ////List<Tuple<string, Type, object>> effectParameters = new List<Tuple<string, Type, object>>();
            ////effectParameters.Add(new Tuple<string, Type, object>("World", typeof(Matrix), Matrix.Identity));
            ////effectParameters.Add(new Tuple<string, Type, object>("View", typeof(Matrix), cam.TransformMatrix));
            ////effectParameters.Add(new Tuple<string, Type, object>("Projection", typeof(Matrix), projection));
            ////effectParameters.Add(new Tuple<string, Type, object>("lightCount", typeof(int), 2));
            ////effectParameters.Add(new Tuple<string, Type, object>("DiffuseIntensity", typeof(float), torchValue));
            ////effectParameters.Add(new Tuple<string, Type, object>("DiffuseLightPosition", typeof(Vector3[]), new Vector3[] {
            ////    new Vector3(ComponentManager.Instance.entities[0].position.X, ComponentManager.Instance.entities[0].position.Y, -16.0f),
            ////    new Vector3(320, 80, 144.0f)}));

            ////EffectManager.Instance.ApplyParameters(effectParameters);

            //Rectangle camView = cam.VisibleArea;

            //RoomManager.Instance.room.Draw(spriteBatch, cam, false, false);// (MyGameEditor.activeTool is CollisionTool) ? true : false, (MyGameEditor.activeTool is ZoneTool) ? true : false);
            //SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.TransformMatrix(new Vector2(0f, 0f)));
            //foreach (RenderComponent rc in ComponentManager.Instance.RenderComponents().Where(rc => rc.IsActive))
            //{
            //    Entity ent = ComponentManager.Instance.entities.Where(e => e.id == rc.entityId).FirstOrDefault();
            //    Vector2 pos = (ent == null) ? new Vector2(0.0f, 0.0f) : ent.position;
            //    rc.Draw(spriteBatch, pos);
            //}

            //DrawHUD(gameTime, spriteBatch);
            //spriteBatch.End();

            GraphicsDevice.SetRenderTarget(renderTarget);

            // these draw calls will now render onto backbuffer
            GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            this.spriteBatch.Draw(this.localRenderTarget, new Rectangle(0, 0, this.localRenderTarget.Width * GameValues.scaleFactor, this.localRenderTarget.Height * GameValues.scaleFactor), Color.White);
            this.spriteBatch.End();

            //base.Draw(gameTime);
        }

        public void DrawHUD(GameTime time, SpriteBatch spriteBatch)
        {

        }
    }
}
