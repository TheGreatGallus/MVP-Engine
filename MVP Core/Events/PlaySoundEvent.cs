using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Events
{
    class PlaySoundEvent
    {
        public string effectName;
        public float volume, pan, pitch;
        public Boolean isLooping;

        public void SetValues(string effectName, float volume, float pan, float pitch, Boolean isLooping)
        {
            this.effectName = effectName;
            this.volume = volume;
            this.pan = pan;
            this.pitch = pitch;
            this.isLooping = isLooping;
        }
    }
}
