using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MVP_Core.Entities;
using MVP_Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MVP_Core.Managers
{
    public class AnimationManager : Manager<Animation>
    {
        private static readonly Lazy<AnimationManager> lazy =
            new Lazy<AnimationManager>(() => new AnimationManager());

        public static AnimationManager Instance { get { return lazy.Value; } }
        public List<QueuedAnimation> animationQueue;

        private AnimationManager()
        {
            Initialize();
            animationQueue = new List<QueuedAnimation>();
        }

        public void Draw(SpriteBatch spriteBatch, string animationName, Vector2 position, Rectangle? sourceRectangle, Color color, int frameCount)
        {
            Animation animation = GetItem(animationName);
            animation.DrawFrame(spriteBatch, animationName, position, sourceRectangle, color, frameCount);
        }

        public void DrawQueue(SpriteBatch spriteBatch, bool frameStart)
        {
            foreach (QueuedAnimation queuedAnimation in animationQueue.Where(qa => qa.frameStart == frameStart).ToList())
            {
                Draw(spriteBatch, queuedAnimation.animationName, queuedAnimation.position, queuedAnimation.sourceRectangle, queuedAnimation.color, queuedAnimation.frameCount);
                animationQueue.Remove(queuedAnimation);
            }
        }

        public void Queue(string animationName, Vector2 position, Rectangle? sourceRectangle, Color color, int frameCount, bool frameStart)
        {
            animationQueue.Add(new QueuedAnimation(animationName, position, sourceRectangle, color, frameCount, frameStart));
        }

        public List<string> GetAnimationNames()
        {
            return bank.Keys.ToList();
        }

        public List<string> GetFrameNames(string animation)
        {
            return bank[animation].Frames.Select(f => f.Value.spriteName).ToList();
        }

        public void SaveAnimations()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("animations");
            foreach (Animation animation in bank.Values)
            {
                XElement animElm = new XElement("animation");
                animElm.Add(new XElement("name", animation.name));
                animElm.Add(new XElement("endFrame", animation.endFrame));

                XElement frames = new XElement("frames");
                foreach (Frame frame in animation.Frames.Values)
                {
                    XElement frameElm = new XElement("frame");
                    frameElm.Add(new XElement("spriteName", frame.spriteName));
                    frameElm.Add(new XElement("frameCount", frame.frameCount));
                    Rectangle source = frame.sourceRectangle;
                    frameElm.Add(new XElement("x", source.X));
                    frameElm.Add(new XElement("y", source.Y));
                    frameElm.Add(new XElement("width", source.Width));
                    frameElm.Add(new XElement("height", source.Height));
                    frames.Add(frameElm);
                }
                animElm.Add(frames);
                root.Add(animElm);
            }
            doc.Add(root);
            doc.Save(GameValues.Path + "Sprites/Animations.xml");
        }

    }

    public class QueuedAnimation
    {
        public string animationName;
        public Vector2 position;
        public Rectangle? sourceRectangle;
        public Color color;
        public int frameCount;
        public bool frameStart = false;

        public QueuedAnimation(string animationName, Vector2 position, Rectangle? sourceRectangle, Color color, int frameCount, bool frameStart)
        {
            this.animationName = animationName;
            this.position = position;
            this.sourceRectangle = sourceRectangle;
            this.color = color;
            this.frameCount = frameCount;
            this.frameStart = frameStart;
        }
    }
}
