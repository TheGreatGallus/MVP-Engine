using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MVP_Core.Global;
using MVP_Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Entities
{
    public class Layer
    {
        public string name;
        public string[] tileArray;
        public int width;
        public int height;
        public int depth = 0;
        public float scale = 0.5f;

        public Layer CopyOf()
        {
            Layer returnedLayer = new Layer();
            returnedLayer.name = name;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    returnedLayer.tileArray = new string[tileArray.Length];
                    tileArray.CopyTo(returnedLayer.tileArray, 0);
                }
            }
            returnedLayer.width = width;
            returnedLayer.height = height;
            returnedLayer.depth = depth;
            return returnedLayer;
        }

        public Layer()
        {


        }

        public Layer(int width, int height)
        {
            tileArray = new string[width * height];
            for (int i = 0; i < tileArray.Length; i++)
            {
                tileArray[i] = "0-15";
            }
            this.width = width;
            this.height = height;
        }

        public Layer(string[] tileArray, int width, int height)
        {
            BuildLayer(tileArray, width, height, 0, 1.0f, "none");
        }

        public Layer(string[] tileArray, int width, int height, int depth)
        {
            BuildLayer(tileArray, width, height, depth, 1.0f, "none");
        }

        public Layer(string[] tileArray, int width, int height, int depth, string name)
        {
            BuildLayer(tileArray, width, height, depth, 1.0f, name);
        }

        public Layer(string[] tileArray, int width, int height, int depth, float scale, string name)
        {
            BuildLayer(tileArray, width, height, depth, scale, name);
        }

        public void BuildLayer(string[] tileArray, int width, int height, int depth, float scale, string name)
        {
            this.name = name;
            this.depth = depth * GameValues.tileDim;
            this.width = width;
            this.height = height;
            this.scale = scale;
            this.tileArray = tileArray;
        }

        public override string ToString()
        {
            return name;
        }

        public void ChangeProperties(string newName, int newHeight, int newWidth, int newDepth)
        {
            name = newName;
            if (height != newHeight || width != newWidth)
            {
                string[] newTileArray = new string[newWidth * newHeight];
                // Zero Array
                for (int j = 0; j < newHeight; j++)
                {
                    for (int i = 0; i < newWidth; i++)
                    {
                        if (j >= height || i >= width)
                            newTileArray[i + j * newWidth] = "0-0";
                        else
                            newTileArray[i + j * newWidth] = tileArray[i + (j) * width];
                    }
                }
                tileArray = newTileArray;
            }

            height = newHeight;
            width = newWidth;
            depth = newDepth;
        }

        public void Draw(int xLeft, int xRight, int yTop, int yBottom, SpriteBatch spriteBatch, OrthographicCamera cam, bool isCollision, Vector2 dim)
        {
            for (int j = yTop; j <= yBottom; j++)
            {
                for (int i = xLeft; i <= xRight; i++)
                {
                    if (i + (j * width) < 0 || i + (j * width) >= tileArray.Length)
                        continue;
                    string[] components = tileArray[i + (j * width)].Split('-');
                    if (components.Length == 2)
                    {
                        int tileNum = Int32.Parse(components[1]);
                        Texture2D tilesetTexture = TextureManager.Instance.GetItem(TextureManager.Instance.GetTilesetName(Int32.Parse(components[0])));
                        int tilesetWidth = (int)(tilesetTexture.Width / GameValues.tileDim);

                        // Shader Stuff
                        //Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(Matrix.Identity));
                        //List<Tuple<string, System.Type, object>> effectParameters = new List<Tuple<string, System.Type, object>>();
                        //effectParameters.Add(new Tuple<string, System.Type, object>("NormalMap", typeof(Texture), TextureManager.Instance.GetItem("MetTileTypeN")));
                        //effectParameters.Add(new Tuple<string, System.Type, object>("WorldInverseTranspose", typeof(Matrix), worldInverseTransposeMatrix));
                        //effectParameters.Add(new Tuple<string, System.Type, object>("Depth", typeof(float), depth));
                        //EffectManager.Instance.ApplyParameters(effectParameters);
                        ////EffectManager.Instance.ApplyEffect("Test");
                        //EffectManager.Instance.ApplyEffect("Default");

                        if (depth > 0)
                            TextureManager.Instance.Draw(spriteBatch, TextureManager.Instance.GetTilesetName(Int32.Parse(components[0])),
                            new Vector2(i * GameValues.tileDim, j * GameValues.tileDim),
                            new Rectangle(tileNum % tilesetWidth * GameValues.tileDim, tileNum / tilesetWidth * GameValues.tileDim, GameValues.tileDim, GameValues.tileDim), Color.Blue);
                        //0.0f, new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
                        else
                            TextureManager.Instance.Draw(spriteBatch, TextureManager.Instance.GetTilesetName(Int32.Parse(components[0])),
                            new Vector2(i * GameValues.tileDim, j * GameValues.tileDim),
                            new Rectangle(tileNum % tilesetWidth * GameValues.tileDim, tileNum / tilesetWidth * GameValues.tileDim, GameValues.tileDim, GameValues.tileDim), Color.White);
                    }
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
