using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Media
{
    class EnhancedSoundEffectInstance
    {
        SoundEffectInstance instance;

        // Read Onlys
        public bool IsDisposed
        {
            get { return instance.IsDisposed; }
        }

        public SoundState State
        {
            get { return instance.State; }
        }

        // Settable
        public bool IsLooped
        {
            get { return instance.IsLooped; }
            set { instance.IsLooped = value; }
        }
        public float Pan
        {
            get { return instance.Pan; }
            set { instance.Pan = value; }
        }
        public float Pitch
        {
            get { return instance.Pitch; }
            set { instance.Pitch = value; }
        }
        public float Volume
        {
            get { return instance.Volume; }
            set { instance.Volume = value; }
        }

        bool DefaultIsLooped;
        float DefaultPan;
        float DefaultPitch;
        float DefaultVolume;

        public EnhancedSoundEffectInstance(SoundEffectInstance instance)
        {
            this.instance = instance;
            DefaultIsLooped = instance.IsLooped;
            DefaultPan = instance.Pan;
            DefaultPitch = instance.Pitch;
            DefaultVolume = instance.Volume;
        }

        public void Stop()
        {
            instance.Stop();
        }

        public void Mute()
        {
            instance.Volume = 0.0f;
        }

        public void Unmute()
        {
            instance.Volume = DefaultVolume;
        }
    }
}
