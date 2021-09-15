using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Global;
using MVP_Core.Managers;
using System;

namespace MVP_Core.Entities
{
    public class Collision
    {
        public string name;
        public Tile[,] tiles;
        public string[] tileArray;
        public int width;
        public int height;
        public int depth = 0;

        public Collision CopyOf()
        {
            Collision returnedLayer = new Collision();
            returnedLayer.name = name;
            returnedLayer.tiles = new Tile[tiles.GetLength(0), tiles.GetLength(1)];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    returnedLayer.tiles[i, j] = tiles[i, j].CopyOf();
                    returnedLayer.tileArray = new string[tileArray.Length];
                    tileArray.CopyTo(returnedLayer.tileArray, 0);
                }
            }
            returnedLayer.width = width;
            returnedLayer.height = height;
            returnedLayer.depth = depth;
            return returnedLayer;
        }

        public Collision()
        {

        }

        public Collision(string[] tileArray, int width, int height)
        {
            BuildLayer(tileArray, width, height, 0, "none");
        }

        public Collision(string[] tileArray, int width, int height, int depth)
        {
            BuildLayer(tileArray, width, height, depth, "none");
        }

        public Collision(string[] tileArray, int width, int height, int depth, string name)
        {
            BuildLayer(tileArray, width, height, depth, name);
        }

        public void BuildLayer(string[] tileArray, int width, int height, int depth, string name)
        {
            tiles = new Tile[width, height];
            this.name = name;
            this.depth = depth * GameValues.tileDim;
            this.width = width;
            this.height = height;
            this.tileArray = tileArray;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    string[] tileString = tileArray[i + j * width].Split('-');

                    try
                    {
                        Tile refTile = Tile.tileDictionary[tileString[1]];
                        Tile newTile = new Tile(i * GameValues.tileDim, j * GameValues.tileDim, refTile.c, refTile.t, refTile.sequence, tileString[1]);
                        tiles[i, j] = newTile;
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        public override string ToString()
        {
            return name;
        }

        public void Draw(int xLeft, int xRight, int yTop, int yBottom, SpriteBatch spriteBatch, OrthographicCamera cam, bool isCollision, Vector2 dim)
        {
            for (int i = 0; i < tileArray.Length; i++)
            {
                string[] components = tileArray[i].Split('-');
                if (components.Length == 2)
                {
                    int tileNum = Int32.Parse(components[1]);
                    Texture2D tilesetTexture = TextureManager.Instance.GetItem(TextureManager.Instance.GetTilesetName(Int32.Parse(components[0])));
                    int tilesetWidth = (int)(tilesetTexture.Width / GameValues.tileDim);

                    TextureManager.Instance.Draw(spriteBatch, "CollisionTiles", new Vector2(i % width * GameValues.tileDim, i / width * GameValues.tileDim),
                        new Rectangle(tileNum % tilesetWidth * GameValues.tileDim, tileNum / tilesetWidth * GameValues.tileDim, GameValues.tileDim, GameValues.tileDim),
                        Color.Pink);
                }
            }
        }

        public string LayerTileString()
        {
            string layerTiles = "";

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (i != 0)
                    {
                        layerTiles += " ";
                    }
                    layerTiles += tileArray[j * width + i];
                }
                layerTiles += "\n";
            }

            return layerTiles;
        }
    }
}
