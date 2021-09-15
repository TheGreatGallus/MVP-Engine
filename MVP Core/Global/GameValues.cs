using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVP_Core.Global
{
    public class GameValues
    {
        public static int screenWidth;
        public static int ScaledWidth
        {
            get { return screenWidth / scaleFactor; }
        }
        public static int TiledWidth
        {
            get { return screenWidth / tileDim; }
        }

        public static int screenHeight;
        public static int ScaledHeight
        {
            get { return screenHeight / scaleFactor; }
        }
        public static int TiledHeight
        {
            get { return screenHeight / tileDim; }
        }

        public static int scaleFactor = 1;
        public static int tileDim;

        public static Texture2D pixel;

        public static string Path = String.Empty;
        public static int frame = 0;

        public static void Initialize(XElement parameters)
        {
            screenWidth = Int32.Parse(parameters.Descendants("screenWidth").Single().Value);
            screenHeight = Int32.Parse(parameters.Descendants("screenHeight").Single().Value);
            scaleFactor = Int32.Parse(parameters.Descendants("scaleFactor").Single().Value);
            tileDim = Int32.Parse(parameters.Descendants("tileDim").Single().Value);
        }
    }
}
