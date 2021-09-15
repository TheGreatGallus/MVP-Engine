using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MVP_Core;
using MVP_Core.Components;
using MVP_Core.Entities;
using MVP_Core.Global;
using MVP_Core.Managers;
using MVP_Editor.Managers;
using MVP_Editor.Tools;
using MVP_Editor.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Num = System.Numerics;

namespace MVP_Editor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public MVPGame game;
        private RenderTarget2D renderTarget;
        private Texture2D renderTexture;
        private IntPtr imGuiTexture;
        public ImGuiRenderer ImGuiRenderer;
        private bool muted = false;
        private bool paused = true;
        public ObservableCollection<string> zoneTypes;
        public static MouseState oldMouseState = Mouse.GetState();
        public static KeyboardState oldKeyboardState = Keyboard.GetState();
        public static Tool activeTool;

        Action<int> func;
        public EditorStates engineState;
        Num.Vector2 cursorPos;
        Num.Vector2 mousePos;
        int mouseX = 0;
        int mouseY = 0;


        public enum EditorStates
        {
            Play,
            Edit
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferMultiSampling = true;

            GameValues.screenWidth = 1024;
            GameValues.screenHeight = 576;

            Content.RootDirectory = "Content";
            engineState = EditorStates.Edit;
            game = new MVPGame() { Graphics = graphics };
            //game.Graphics = graphics;
            IsMouseVisible = true;

            zoneTypes = new ObservableCollection<string>();
            EditorWindows.editor = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ImGuiRenderer = new ImGuiRenderer(this);
            ImGuiRenderer.RebuildFontAtlas();

            game.Initialize();
            


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            game.SpriteBatch = spriteBatch;

            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, GameValues.screenWidth, GameValues.screenHeight);
            game.RenderTarget = renderTarget;

            renderTexture = new Texture2D(GraphicsDevice, GameValues.screenWidth, GameValues.screenHeight);
            imGuiTexture = ImGuiRenderer.BindTexture(renderTexture);

            // TODO: use this.Content to load your game content here
            //game.LoadContent();

            //LoadScripts();
        }

        //public void LoadScripts()
        //{
        //    try
        //    {
        //        List<string> errors = new List<string>();
        //        //Assembly assembly = CSharpScriptingEngine.Compile(out errors, File.ReadAllText("Content/Scripts/TestScripts.cs"));
        //        Assembly assembly = CSharpScriptingEngine.Compile(out errors, File.ReadAllText(GameValues.Path + "/Scripts/Other/TestScripts.cs"));
        //        var type = assembly.GetType("TestScripts");
        //        var method = type.GetMethod("update");
        //        func = (Action<int>)method.CreateDelegate(typeof(Action<int>));
        //        UpdateComponent playerUC = ComponentManager.Instance.GetComponentByEntityId(0, typeof(UpdateComponent)) as UpdateComponent;
        //        playerUC.SetUpdate(func);
        //        playerUC.SetType(type);
        //        playerUC.Activate();

        //        string filepath = GameValues.Path + "/Scripts";
        //        DirectoryInfo d = new DirectoryInfo(filepath);

        //        foreach (var file in d.GetFiles("*.cs"))
        //        {
        //            string fullPath = filepath + "/" + file.Name;
        //            string entityTypeName = Path.GetFileNameWithoutExtension(fullPath);
        //            Assembly newAssembly = CSharpScriptingEngine.Compile(out errors, File.ReadAllText(fullPath));
        //            Type entityType = newAssembly.GetType(entityTypeName);
        //            var updateMethod = entityType.GetMethod("update");
        //            Action<int> updateFunc = (Action<int>)updateMethod.CreateDelegate(typeof(Action<int>));
        //            ScriptManager.Instance.RegisterItem(entityTypeName, new Tuple<Type, Action<int>>(entityType, updateFunc));
        //            if (entityType.IsSubclassOf(typeof(Zone)))
        //            {
        //                Zone.types.Add(entityTypeName, entityType);
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}
        public void LoadScripts()
        {
            try
            {
                ScriptManager.Instance.ClearItems();
                LoadOtherScripts();
                LoadEntityScripts();
                LoadZoneScripts();
            }
            catch (Exception e)
            {

            }
        }

        private void LoadEntityScripts()
        {
            string filepath = GameValues.Path + "Scripts/Entities";
            DirectoryInfo d = new DirectoryInfo(filepath);
            Entity.types.Clear();

            foreach (var file in d.GetFiles("*.cs"))
            {
                string fullPath = filepath + "/" + file.Name;
                string entityTypeName = Path.GetFileNameWithoutExtension(fullPath);
                List<string> errors = new List<string>();
                Assembly newAssembly = CSharpScriptingEngine.Compile(out errors, File.ReadAllText(fullPath));
                Type entityType = newAssembly.GetType(entityTypeName);
                var updateMethod = entityType.GetMethod("update");
                Action<int> updateFunc = (Action<int>)updateMethod.CreateDelegate(typeof(Action<int>));
                ScriptManager.Instance.RegisterItem(entityTypeName, new Tuple<Type, Action<int>>(entityType, updateFunc));
                Entity.types.Add(entityTypeName, entityType);
            }
            LoadCritters();

            if (ComponentManager.Instance.entities.Count > 2)
            {
                foreach (Entity entity in ComponentManager.Instance.entities.Where(e => e.id > 1).ToList())
                {
                    entity.SetCritter(CritterManager.Instance.GetItem(entity.critterName));
                    UpdateComponent uc = ComponentManager.Instance.GetComponentByEntityId(entity.id, typeof(UpdateComponent)) as UpdateComponent;
                    uc.SetUpdate(entity.critter.updateAction);
                }
            }
        }
        public void LoadCritters()
        {
            XDocument doc = XDocument.Load(GameValues.Path + "Brin/Enemy.xml");
            foreach (XElement elm in doc.Descendants("critter"))
            {
                string type = elm.Descendants("type").Single().Value;
                int width = Int32.Parse(elm.Descendants("width").Single().Value);
                int height = Int32.Parse(elm.Descendants("height").Single().Value);
                Tuple<Type, Action<int>> scriptData = ScriptManager.Instance.GetItem(type);
                Critter newCritter = new Critter(scriptData.Item1, scriptData.Item2, new Vector2(width, height), null);
                CritterManager.Instance.RegisterItem(type, newCritter);
                //critters.Add(newCritter);
            }
        }

        private void LoadZoneScripts()
        {
            string filepath = GameValues.Path + "Scripts/Zones";
            DirectoryInfo d = new DirectoryInfo(filepath);
            Zone.types.Clear();
            zoneTypes.Clear();

            foreach (var file in d.GetFiles("*.cs"))
            {
                string fullPath = filepath + "/" + file.Name;
                string zoneTypeName = Path.GetFileNameWithoutExtension(fullPath);
                List<string> errors = new List<string>();
                Assembly newAssembly = CSharpScriptingEngine.Compile(out errors, File.ReadAllText(fullPath));
                var zoneType = newAssembly.GetType(zoneTypeName);
                Zone.types.Add(zoneTypeName, zoneType);
                zoneTypes.Add(zoneTypeName);
            }
        }

        private void LoadOtherScripts()
        {
            string filepath = GameValues.Path + "Scripts/Other";
            DirectoryInfo d = new DirectoryInfo(filepath);

            foreach (var file in d.GetFiles("*.cs"))
            {
                string fullPath = filepath + "/" + file.Name;
                string otherTypeName = Path.GetFileNameWithoutExtension(fullPath);
                List<string> errors = new List<string>();
                Assembly newAssembly = CSharpScriptingEngine.Compile(out errors, File.ReadAllText(fullPath));
                var type = newAssembly.GetType(otherTypeName);
                var method = type.GetMethod("update");
                func = (Action<int>)method.CreateDelegate(typeof(Action<int>));
                int id = (otherTypeName == "TestScripts") ? 0 : 1;
                UpdateComponent playerUC = ComponentManager.Instance.GetComponentByEntityId(id, typeof(UpdateComponent)) as UpdateComponent;
                playerUC.SetUpdate(func);
                playerUC.SetType(type);
                playerUC.Activate();
            }
            //string filepath = "Content/Scripts/Other";
            //DirectoryInfo d = new DirectoryInfo(filepath);

            //foreach (var file in d.GetFiles("*.cs"))
            //{
            //    string fullPath = filepath + "/" + file.Name;
            //    string zoneTypeName = Path.GetFileNameWithoutExtension(fullPath);
            //    Assembly newAssembly = CSharpScriptingEngine.Compile(File.ReadAllText(fullPath));
            //}
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            game.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();
            //var mouseState = Mouse.GetState();

            // TODO: Add your update logic here
            if(muted && !SoundEffectManager.Instance.IsMuted) { SoundEffectManager.Instance.MuteAudio(); }
            if (!muted && SoundEffectManager.Instance.IsMuted) { SoundEffectManager.Instance.UnmuteAudio(); }

            if (engineState == EditorStates.Play)
            {
                game.Update(gameTime);
            }
            if (engineState == EditorStates.Edit)
            {
                if (keyboardState.IsKeyDown(Keys.W))
                    game.camera.Move(new Vector2(0, -2));
                    //game.cam.y -= 2;
                if (keyboardState.IsKeyDown(Keys.A))
                    game.camera.Move(new Vector2(-2, 0));
                //game.cam.x -= 2;
                if (keyboardState.IsKeyDown(Keys.S))
                    game.camera.Move(new Vector2(0, 2));
                //game.cam.y += 2;
                if (keyboardState.IsKeyDown(Keys.D))
                    game.camera.Move(new Vector2(2, 0));
                //game.cam.x += 2;

                //if (RoomManager.Instance.room != null && EditorWindows.selectedLayerIndex != -1)
                //{
                //    try
                //    {
                //        if (mouseState.LeftButton.wasClickedOrHeld(oldMouseState.LeftButton))
                //        {
                //            if (activeTool.canLeftBeHeld)
                //            {
                //                activeTool.PressLeftClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //            }
                //            else
                //            {
                //                if (mouseState.LeftButton.wasClicked(oldMouseState.LeftButton))
                //                {
                //                    activeTool.PressLeftClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //                }
                //            }
                //        }
                //        if (mouseState.LeftButton.wasReleased(oldMouseState.LeftButton))
                //        {
                //            activeTool.ReleaseLeftClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //        }
                //        if (mouseState.RightButton.wasClickedOrHeld(oldMouseState.RightButton))
                //        {
                //            if (activeTool.canRightBeHeld)
                //            {
                //                activeTool.PressRightClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //            }
                //            else
                //            {
                //                if (mouseState.RightButton.wasClicked(oldMouseState.RightButton))
                //                {
                //                    activeTool.PressRightClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //                }
                //            }
                //        }
                //        if (mouseState.RightButton.wasReleased(oldMouseState.RightButton))
                //        {
                //            activeTool.ReleaseRightClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //        }
                //        if (mouseState.MiddleButton.wasClickedOrHeld(oldMouseState.MiddleButton))
                //        {
                //            activeTool.PressMiddleClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                //            //if (MiddleClickAction != null)
                //            //    MiddleClickAction.Invoke();
                //        }

                //        if (keyboardState.IsKeyDown(Keys.X) && oldKeyboardState.IsKeyUp(Keys.X))
                //        {
                //            activeTool.TypeX();
                //        }

                //        oldMouseState = mouseState;
                //    }
                //    catch (IndexOutOfRangeException ex)
                //    {

                //    }
                //}
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //ImGuiRenderer.UnbindTexture(imGuiTexture);

            game.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            //spriteBatch.Begin();
            //spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);
            //spriteBatch.End();

            //imGuiTexture = ImGuiRenderer.BindTexture((Texture2D)renderTarget);
            Color[] rawData = new Color[(GameValues.screenWidth) * (GameValues.screenHeight)];
            renderTarget.GetData<Color>(rawData);
            renderTexture.SetData(rawData);

            ImGuiRenderer.BeforeLayout(gameTime);
            ImGuiLayout();
            ImGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        protected virtual void ImGuiLayout()
        {
            {
                if (ImGui.BeginMainMenuBar())
                {
                    if (ImGui.BeginMenu("Project"))
                    {
                        if (ImGui.MenuItem("Open...", "Crtl+O"))
                        {
                            ProjectManager.Instance.OpenProject();
                            LoadScripts();
                            game.LoadContent(Content);
                            activeTool = new PencilTool(GraphicsDevice, RoomManager.Instance.room, game.camera);
                            activeTool.primaryTileNum = "0-1";
                            activeTool.secondaryTileNum = "0-15";
                            SongManager.Instance.PauseAudio();
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Util"))
                    {
                        if (ImGui.MenuItem("Rooms", "Ctrl+R"))          EditorWindows.roomsWindowOpen = true;
                        if (ImGui.MenuItem("Layers", "Ctrl+L"))         EditorWindows.layersWindowOpen = true;
                        if (ImGui.MenuItem("Animations", "Ctrl+A"))     EditorWindows.animationsWindowOpen = true;
                        if (ImGui.MenuItem("Tilesets", "Ctrl+T"))       EditorWindows.tilesetsWindowOpen = true;
                        if (ImGui.MenuItem("Critters", "Ctrl+C"))         EditorWindows.crittersWindowOpen = true;
                        if (ImGui.MenuItem("Music", "Ctrl+M"))          EditorWindows.musicWindowOpen = true;
                        if (ImGui.MenuItem("Tools", "Ctrl+O"))          EditorWindows.toolsWindowOpen = true;
                        ImGui.EndMenu();
                    }
                    ImGui.EndMainMenuBar();

                    // Menu Shortcuts
                    KeyboardState shortcutChecker = Keyboard.GetState();
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.R, Keys.LeftControl))      EditorWindows.roomsWindowOpen = true;
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.L, Keys.LeftControl))      EditorWindows.layersWindowOpen = true;
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.A, Keys.LeftControl))      EditorWindows.animationsWindowOpen = true;
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.T, Keys.LeftControl))      EditorWindows.tilesetsWindowOpen = true;
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.C, Keys.LeftControl))      EditorWindows.crittersWindowOpen = true;
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.M, Keys.LeftControl))      EditorWindows.musicWindowOpen = true;
                    if (KeyboardUtil.isShortcutPressed(shortcutChecker, Keys.O, Keys.LeftControl))      EditorWindows.toolsWindowOpen = true;
                }

                ImGui.Begin("Game View", ImGuiWindowFlags.MenuBar);
                if (ImGui.BeginMenuBar())
                {
                    ImGui.Checkbox("Mute", ref muted);
                    if (ImGui.Checkbox("Pause", ref paused))
                    {
                        if (engineState == EditorStates.Edit)
                        {
                            engineState = EditorStates.Play;
                            SongManager.Instance.UnpauseAudio();
                        }
                        else
                        {
                            engineState = EditorStates.Edit;
                            SongManager.Instance.PauseAudio();
                        }
                    }
                    ImGui.Checkbox("Collisions", ref game.renderCollisions);
                    ImGui.Checkbox("Zones", ref game.renderZones);
                    ImGui.Checkbox("Grid", ref game.renderGrid);
                    ImGui.Checkbox("AABB", ref game.showAABBs);

                    ImGui.EndMenuBar();
                }
                ImGui.ImageButton(imGuiTexture, new Num.Vector2(GameValues.screenWidth, GameValues.screenHeight), Num.Vector2.Zero, Num.Vector2.One, 0);
                if(ImGui.IsItemHovered())
                {
                    cursorPos = ImGui.GetCursorScreenPos();
                    cursorPos.Y -= renderTarget.Height + 6;
                    mousePos = ImGui.GetIO().MousePos;
                    float region_sz = 0.0f;

                    float region_x = mousePos.X - cursorPos.X - region_sz * 0.5f; if (region_x < 0.0f) region_x = 0.0f; else if (region_x > renderTarget.Width - region_sz) region_x = renderTarget.Width - region_sz;
                    float region_y = mousePos.Y - cursorPos.Y - region_sz * 0.5f; if (region_y < 0.0f) region_y = 0.0f; else if (region_y > renderTarget.Height - region_sz) region_y = renderTarget.Height - region_sz;

                    mouseX = (int)(region_x / GameValues.scaleFactor);
                    mouseY = (int)(region_y / GameValues.scaleFactor);
                    //Console.WriteLine("x:" + mouseX + " y:" + mouseY);
                    //Console.WriteLine("x:" + region_x + " y:" + region_y);
                    var mouseState = Mouse.GetState();

                    if (RoomManager.Instance.room != null && EditorWindows.selectedLayerIndex != -1)
                    {
                        try
                        {
                            if (mouseState.LeftButton.wasClickedOrHeld(oldMouseState.LeftButton))
                            {
                                if (activeTool.canLeftBeHeld)
                                {
                                    activeTool.PressLeftClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                                }
                                else
                                {
                                    if (mouseState.LeftButton.wasClicked(oldMouseState.LeftButton))
                                    {
                                        activeTool.PressLeftClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                                    }
                                }
                            }
                            if (mouseState.LeftButton.wasReleased(oldMouseState.LeftButton))
                            {
                                activeTool.ReleaseLeftClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                            }
                            if (mouseState.RightButton.wasClickedOrHeld(oldMouseState.RightButton))
                            {
                                if (activeTool.canRightBeHeld)
                                {
                                    activeTool.PressRightClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                                }
                                else
                                {
                                    if (mouseState.RightButton.wasClicked(oldMouseState.RightButton))
                                    {
                                        activeTool.PressRightClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                                    }
                                }
                            }
                            if (mouseState.RightButton.wasReleased(oldMouseState.RightButton))
                            {
                                activeTool.ReleaseRightClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                            }
                            if (mouseState.MiddleButton.wasClickedOrHeld(oldMouseState.MiddleButton))
                            {
                                activeTool.PressMiddleClick(mouseX, mouseY, EditorWindows.selectedLayerIndex);
                                //if (MiddleClickAction != null)
                                //    MiddleClickAction.Invoke();
                            }

                            //if (keyboardState.IsKeyDown(Keys.X) && oldKeyboardState.IsKeyUp(Keys.X))
                            //{
                            //    activeTool.TypeX();
                            //}

                            oldMouseState = mouseState;
                        }
                        catch (IndexOutOfRangeException ex)
                        {

                        }
                    }
                }
                ImGui.End();

                EditorWindows.LayersWindow();
                EditorWindows.RoomsWindow();
                EditorWindows.AnimationsWindow();
                EditorWindows.TilesetsWindow();
                EditorWindows.CrittersWindow();
                EditorWindows.MusicWindow();
                EditorWindows.ToolsWindow();
            }

            //// 2. Show another simple window, this time using an explicit Begin/End pair
            //if (show_another_window)
            //{
            //    ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
            //    ImGui.Begin("Another Window", ref show_another_window);
            //    ImGui.Text("Hello");
            //    ImGui.End();
            //}

            //// 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            //if (show_test_window)
            //{
            //    ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow();
            //}
        }

        //private void OpenProject()
        //{
        //    var fileContent = string.Empty;
        //    var filePath = string.Empty;
        //    using (var ofd = new OpenFileDialog())
        //    {
        //        ofd.InitialDirectory = "c:\\";
        //        ofd.Filter = "XML files|*.xml";
        //        ofd.FilterIndex = 0;
        //        ofd.RestoreDirectory = false;

        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            filePath = ofd.FileName;
        //            GameValues.Path = Path.GetDirectoryName(filePath);
        //            GameValues.Path = GameValues.Path.Replace('\\', '/');
        //            var fileStream = ofd.OpenFile();
        //            try
        //            {
        //                XDocument doc = XDocument.Load(fileStream);
        //                XElement settings = doc.Descendants("settings").Single();
        //                GameValues.Initialize(settings.Descendants("engineSettings").Single());
        //                InputManager.Initialize(settings.Descendants("keyFunctions").Single());
        //            }
        //            catch(Exception e)
        //            {

        //            }
        //        }
        //    }
        //}
    }
}
