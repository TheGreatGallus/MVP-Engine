using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MVP_Core.Entities;
using MVP_Core.Global;
using MVP_Core.Managers;
using MVP_Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Num = System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Editor.Util
{
    class EditorWindows
    {
        public static Game1 editor;

        static Num.Vector2 cursorPos = new Num.Vector2(0f, 0f);
        static Num.Vector2 mousePos = new Num.Vector2(0f, 0f);
        static int mouseX = 0;
        static int mouseY = 0;

        public static bool layersWindowOpen;
        public static bool roomsWindowOpen;
        public static bool animationsWindowOpen;
        public static bool tilesetsWindowOpen;
        public static bool crittersWindowOpen;
        public static bool musicWindowOpen;
        public static bool toolsWindowOpen;

        public static int selectedLayerIndex = -1;
        static Layer selectedLayer;
        static int selectedRoomIndex = -1;
        static Room selectedRoom;
        static int selectedAnimationIndex = -1;
        static Animation selectedAnimation;
        static int selectedTilesetIndex = -1;
        //static Tileset selectedTileset;
        static int selectedCritterIndex = -1;
        static Critter selectedCritter;
        static int selectedMusicIndex = -1;
        static Song selectedSong;
        static int selectedToolIndex = 3;

        static RenderTarget2D tilesetRenderTarget;
        static Texture2D tilesetRenderTexture;
        static IntPtr TilesetImGuiTexture;
        static Num.Vector2 tilesetDimensions = new Num.Vector2(0f, 0f);
        static IntPtr PrimaryTileTexture;
        static IntPtr SecondaryTileTexture;

        public static void LayersWindow()
        {
            if (!layersWindowOpen)
                return;
            ImGui.Begin("Layers", ref layersWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Layers", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
            int n = 0;
            foreach (Layer layer in RoomManager.Instance.GetLayers())
            {
                if (ImGui.Selectable(layer.name, selectedLayerIndex == n))
                {
                    selectedLayerIndex = n;
                    selectedLayer = layer;
                }
                n++;
            }
            ImGui.EndChild();
            ImGui.Text("Layer Properties");
            if (selectedLayer != null)
            {
                ImGui.InputText("Name", ref selectedLayer.name, 100);
                ImGui.InputInt("Width", ref selectedLayer.width);
                ImGui.InputInt("Height", ref selectedLayer.height);
                ImGui.InputInt("Depth", ref selectedLayer.depth);
                ImGui.InputFloat("Scale", ref selectedLayer.scale);
            }
            ImGui.End();
        }

        public static void RoomsWindow()
        {
            if (!roomsWindowOpen)
                return;
            ImGui.Begin("Rooms", ref roomsWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Rooms", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
            int n = 0;
            foreach (Room room in RoomManager.Instance.GetRooms())
            {
                if (ImGui.Selectable(room.name, selectedRoomIndex == n))
                {
                    selectedRoomIndex = n;
                    selectedRoom = room;
                    RoomManager.Instance.ChangeRoom(room.name);
                }
                n++;
            }
            ImGui.EndChild();
            ImGui.Text("Room Properties");
            if (selectedRoom != null)
            {
                //ImGui.InputText("Name", ref selectedRoom.name, 100);
                //ImGui.InputInt("Width", ref selectedRoom.width);
                //ImGui.InputInt("Height", ref selectedRoom.height);
                int testSelect = -1;
                string[] songList = SongManager.Instance.GetSongNames().ToArray();
                ImGui.Combo("Song", ref testSelect, songList, songList.Count());

            }
            ImGui.End();
        }

        public static void AnimationsWindow()
        {
            if (!animationsWindowOpen)
                return;
            ImGui.Begin("Animations", ref animationsWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Animations", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
            int n = 0;
            foreach (Animation animation in AnimationManager.Instance.GetAll())
            {
                if (ImGui.Selectable(animation.name, selectedAnimationIndex == n))
                {
                    selectedAnimationIndex = n;
                    selectedAnimation = animation;
                }
            }
            ImGui.EndChild();
            ImGui.Text("Animation Properties");
            if (selectedAnimation != null)
            {
                ImGui.InputText("Name", ref selectedAnimation.name, 100);
            }
            ImGui.End();
        }

        public static void TilesetsWindow()
        {
            if (!tilesetsWindowOpen)
                return;
            ImGui.Begin("Tilesets", ref tilesetsWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Tilesets", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
            int n = 0;
            foreach (String tilesetName in TextureManager.Instance.GetTilesetNames())
            {
                if (ImGui.Selectable(tilesetName, selectedTilesetIndex == n))
                {
                    selectedTilesetIndex = n;
                    SetTilesetImage(selectedTilesetIndex);
                }
                n++;
            }
            ImGui.EndChild();
            ImGui.Text("Current Tileset");
            if (selectedTilesetIndex >= 0)
            {
                ImGui.ImageButton(TilesetImGuiTexture, tilesetDimensions, System.Numerics.Vector2.Zero, System.Numerics.Vector2.One, 0);
                if (ImGui.IsItemHovered())
                {
                    bool test = ImGui.GetIO().MouseClicked[0];
                    cursorPos = ImGui.GetCursorScreenPos();
                    cursorPos.Y -= tilesetDimensions.Y + 6;
                    mousePos = ImGui.GetIO().MousePos;
                    float region_sz = 0.0f;

                    float region_x = mousePos.X - cursorPos.X - region_sz * 0.5f; if (region_x < 0.0f) region_x = 0.0f; else if (region_x > tilesetDimensions.X - region_sz) region_x = tilesetDimensions.X - region_sz;
                    float region_y = mousePos.Y - cursorPos.Y - region_sz * 0.5f; if (region_y < 0.0f) region_y = 0.0f; else if (region_y > tilesetDimensions.Y - region_sz) region_y = tilesetDimensions.Y - region_sz;

                    //mouseX = (int)(region_x / GameValues.scaleFactor);
                    //mouseY = (int)(region_y / GameValues.scaleFactor);
                    mouseX = (int)(region_x);
                    mouseY = (int)(region_y);

                    int tileNum = (int)(mouseX / GameValues.tileDim + (mouseY / GameValues.tileDim) * (tilesetDimensions.X / GameValues.tileDim));
                    string tileNumString = "" + selectedTilesetIndex + "-" + tileNum;

                    if (ImGui.IsItemClicked(0))
                    {
                        Game1.activeTool.primaryTileNum = tileNumString;
                        SetPrimaryTileImage(selectedTilesetIndex, tileNum);
                    }
                    if (ImGui.IsItemClicked(1))
                    {
                        Game1.activeTool.secondaryTileNum = tileNumString;
                        SetSecondaryTileImage(selectedTilesetIndex, tileNum);
                    }
                }
                //if(ImGui.IsItemHovered())
                //{
                    
                //}
            }
            ImGui.End();
        }
            public static void SetTilesetImage(int index)
            {
                //tilesetRenderTarget = new RenderTarget2D(editor.GraphicsDevice, GameValues.screenWidth, GameValues.screenHeight);
                //tilesetRenderTexture = new Texture2D(editor.GraphicsDevice, GameValues.screenWidth, GameValues.screenHeight);
                Texture2D tempTilesetTexture = TextureManager.Instance.GetItem(TextureManager.Instance.GetTilesetName(index));
                tilesetDimensions.X = tempTilesetTexture.Width;
                tilesetDimensions.Y = tempTilesetTexture.Height;
                TilesetImGuiTexture = editor.ImGuiRenderer.BindTexture(tempTilesetTexture);
            }

        public static void CrittersWindow()
        {
            if (!crittersWindowOpen)
                return;
            ImGui.Begin("Critters", ref crittersWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Critters", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
            int n = 0;
            foreach(Critter critter in CritterManager.Instance.GetAll())
            {
                if (ImGui.Selectable(critter.name, selectedCritterIndex == n))
                {
                    selectedCritterIndex = n;
                    selectedCritter = critter;
                }
            }
            ImGui.EndChild();
            ImGui.Text("Critter Properties");
            if (selectedCritter != null)
            {
                ImGui.InputText("Name", ref selectedCritter.name, 100);
                ImGui.InputFloat("Width", ref selectedCritter.dimensions.X);
                ImGui.InputFloat("Height", ref selectedCritter.dimensions.Y);
            }
            ImGui.End();
        }

        public static void MusicWindow()
        {
            if (!musicWindowOpen)
                return;
            ImGui.Begin("Music", ref musicWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Songs", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
            int n = 0;
            foreach (string songName in SongManager.Instance.GetSongNames())
            {
                if (ImGui.Selectable(songName, selectedMusicIndex == n))
                {
                    selectedMusicIndex = n;
                    //selectedRoom = room;
                    SongManager.Instance.PlaySong(songName);
                }
                n++;
            }
        }

        public static void ToolsWindow()
        {
            if (!toolsWindowOpen)
                return;
            ImGui.Begin("Tools", ref toolsWindowOpen, ImGuiWindowFlags.MenuBar);
            ImGui.BeginChild("Tools", new Num.Vector2(ImGui.GetWindowContentRegionWidth(), 130f));
           
            if(ImGui.RadioButton("Bucket", ref selectedToolIndex, 0))
            {
                string primary = Game1.activeTool.primaryTileNum;
                string secondary = Game1.activeTool.secondaryTileNum;
                Game1.activeTool = new BucketTool(editor.GraphicsDevice, RoomManager.Instance.room, editor.game.camera);
                Game1.activeTool.primaryTileNum = primary;
                Game1.activeTool.secondaryTileNum = secondary;
            }
            if(ImGui.RadioButton("Collision", ref selectedToolIndex, 1))
            {
                string primary = Game1.activeTool.primaryTileNum;
                string secondary = Game1.activeTool.secondaryTileNum;
                Game1.activeTool = new CollisionTool(editor.GraphicsDevice, RoomManager.Instance.room, editor.game.camera);
                Game1.activeTool.primaryTileNum = primary;
                Game1.activeTool.secondaryTileNum = secondary;
            }
            if(ImGui.RadioButton("Eraser", ref selectedToolIndex, 2))
            {
                string primary = Game1.activeTool.primaryTileNum;
                string secondary = Game1.activeTool.secondaryTileNum;
                Game1.activeTool = new EraserTool(editor.GraphicsDevice, RoomManager.Instance.room, editor.game.camera);
                Game1.activeTool.primaryTileNum = primary;
                Game1.activeTool.secondaryTileNum = secondary;
            }
            if(ImGui.RadioButton("Pencil", ref selectedToolIndex, 3))
            {
                string primary = Game1.activeTool.primaryTileNum;
                string secondary = Game1.activeTool.secondaryTileNum;
                Game1.activeTool = new PencilTool(editor.GraphicsDevice, RoomManager.Instance.room, editor.game.camera);
                Game1.activeTool.primaryTileNum = primary;
                Game1.activeTool.secondaryTileNum = secondary;
            }
            if(ImGui.RadioButton("Zone", ref selectedToolIndex, 4))
            {
                string primary = Game1.activeTool.primaryTileNum;
                string secondary = Game1.activeTool.secondaryTileNum;
                Game1.activeTool = new ZoneTool(editor.GraphicsDevice, RoomManager.Instance.room, editor.game.camera);
                Game1.activeTool.primaryTileNum = primary;
                Game1.activeTool.secondaryTileNum = secondary;
            }
            ImGui.EndChild();
            ImGui.Image(PrimaryTileTexture, new Num.Vector2(GameValues.tileDim, GameValues.tileDim));
            ImGui.SameLine();
            ImGui.Image(SecondaryTileTexture, new Num.Vector2(GameValues.tileDim, GameValues.tileDim));
        }
            public static void SetPrimaryTileImage(int index, int tile)
            {
                Texture2D tilesetImage = TextureManager.Instance.GetItem(TextureManager.Instance.GetTilesetName(index));
                int tileWidthCount = tilesetImage.Width / GameValues.tileDim;
                Rectangle sourceRect = new Rectangle(tile % tileWidthCount * GameValues.tileDim, tile / tileWidthCount * GameValues.tileDim, GameValues.tileDim, GameValues.tileDim);
                Texture2D tileImage = new Texture2D(editor.GraphicsDevice, sourceRect.Width, sourceRect.Height);
                Color[] data = new Color[sourceRect.Width * sourceRect.Height];
                tilesetImage.GetData(0, sourceRect, data, 0, data.Length);
                tileImage.SetData(data);
                PrimaryTileTexture = editor.ImGuiRenderer.BindTexture(tileImage);
            }
            public static void SetSecondaryTileImage(int index, int tile)
            {
                Texture2D tilesetImage = TextureManager.Instance.GetItem(TextureManager.Instance.GetTilesetName(index));
                int tileWidthCount = tilesetImage.Width / GameValues.tileDim;
                Rectangle sourceRect = new Rectangle(tile % tileWidthCount * GameValues.tileDim, tile / tileWidthCount * GameValues.tileDim, GameValues.tileDim, GameValues.tileDim);
                Texture2D tileImage = new Texture2D(editor.GraphicsDevice, sourceRect.Width, sourceRect.Height);
                Color[] data = new Color[sourceRect.Width * sourceRect.Height];
                tilesetImage.GetData(0, sourceRect, data, 0, data.Length);
                tileImage.SetData(data);
                SecondaryTileTexture = editor.ImGuiRenderer.BindTexture(tileImage);
            }
    }
}
