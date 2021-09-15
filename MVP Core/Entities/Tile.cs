using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MVP_Core.Managers;
using System;
using System.Collections.Generic;

namespace MVP_Core.Entities
{
    public class Tile
    {
        public static Texture2D tileNormal;
        public static Texture2D colTileType;
        public Texture2D image;
        public Rectangle rect;
        public Vector2 vect;

        public int x;
        public int y;
        public int leftY;
        public int rightY;
        public bool isChecked = false;
        public bool isColliding = false;
        public int solidityMap;
        public Color c;
        public string tileNum;
        public Type t;
        public int sequence;
        public bool isSlope;
        public bool isSlopeTop;

        public Tile CopyOf()
        {
            Tile returnTile = new Tile(0, 0, Color.Black, Type.AIR, 0, "0");
            returnTile.x = x;
            returnTile.y = y;
            returnTile.vect = new Vector2(vect.Y, vect.Y);
            returnTile.c = c;
            returnTile.t = t;
            returnTile.sequence = sequence;
            returnTile.isSlope = isSlope;
            returnTile.isSlopeTop = isSlopeTop;
            returnTile.leftY = y;
            returnTile.rightY = y;
            returnTile.tileNum = tileNum;
            return returnTile;
        }

        public static Dictionary<Type, Rectangle> sourceRects;
        public static Dictionary<Type, Rectangle> colSourceRects;

        public static Dictionary<string, Tile> tileDictionary = new Dictionary<string, Tile>()
            {
                {"", new Tile(0, 0, Color.Black, Tile.Type.AIR, 0, "")},
                {"0", new Tile(0, 0, Color.Black, Tile.Type.AIR, 0, "0")},
                {"1", new Tile(0, 0, Color.Black, Tile.Type.WALL, 0, "1")},
                {"2", new Tile(0, 0, Color.Black, Tile.Type.Y_ONE_WAY, 0, "2")},
                {"14", new Tile(0, 0, Color.Black, Tile.Type.WALL, 0, "14")},
                {"15", new Tile(0, 0, Color.Black, Tile.Type.AIR, 0, "15")}
            };

        // In progress
        public static Dictionary<String, int[]> heightMaps = new Dictionary<string, int[]>
        {
            { "Air", new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} },
            { "Wall", new int[] {16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16} },
            { "1TSlopeR1", new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15} },
            { "2TSlopeR1", new int[] {0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7} },
            { "2TSlopeR2", new int[] {8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15} }
        };

        public enum Type
        {
            AIR,
            WALL,
            Y_ONE_WAY
        }

        public Tile(int xIn, int yIn, Color cIn, Type typeIn, int sequenceIn, string tileNum)
        {
            x = xIn;
            y = yIn;
            vect = new Vector2(x, y);
            c = cIn;
            t = typeIn;
            sequence = sequenceIn;
            isSlope = false;
            isSlopeTop = false;
            leftY = y;
            rightY = y;
            this.tileNum = tileNum;
        }

        public bool Matches(Tile t)
        {
            return t.c == this.c && t.t == this.t && t.sequence == this.sequence;
        }
    }
}
