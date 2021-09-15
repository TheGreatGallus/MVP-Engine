using Microsoft.Xna.Framework.Audio;
using MVP_Core.Events;
using MVP_Core.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Managers
{
    public class SoundEffectManager : Manager<SoundEffect>
    {
        private static readonly Lazy<SoundEffectManager> lazy =
            new Lazy<SoundEffectManager>(() => new SoundEffectManager());

        public static SoundEffectManager Instance { get { return lazy.Value; } }

        private SoundEffectManager()
        {
            Initialize();
        }

        private readonly int MAX_PENDING = 16;
        private PlaySoundEvent[] pending;
        private int head, tail;
        private List<KeyValuePair<string, EnhancedSoundEffectInstance>> effectInstances;
        public bool IsMuted = false;
        public bool RolledAround = false;

        public override void Initialize()
        {
            base.Initialize();

            pending = new PlaySoundEvent[MAX_PENDING];
            head = 0;
            tail = 0;
            for (int i = 0; i < MAX_PENDING; i++)
            {
                pending[i] = new PlaySoundEvent();
            }

            effectInstances = new List<KeyValuePair<string, EnhancedSoundEffectInstance>>();
        }

        // Test if loop or increment every update
        public void Update()
        {
            effectInstances.RemoveAll(ei => ei.Value.State == SoundState.Stopped);

            if (head == tail)
                return;

            if (RolledAround)
            {
                ActivateSound(MAX_PENDING - 1);
                RolledAround = false;
            }

            for (int i = head; i < tail; i = (i + 1) % MAX_PENDING)
            {
                ActivateSound(i);
            }
            head = tail;
        }

        private void ActivateSound(int i)
        {
            PlaySoundEvent sound = pending[i];
            SoundEffect effect = GetItem(sound.effectName);
            SoundEffectInstance effectInstance = effect.CreateInstance();
            SetInstanceValues(sound, effectInstance);
            effectInstance.Play();
            EnhancedSoundEffectInstance enhancedEffectInstance = new EnhancedSoundEffectInstance(effectInstance);
            if (IsMuted)
                enhancedEffectInstance.Mute();
            effectInstances.Add(new KeyValuePair<string, EnhancedSoundEffectInstance>(sound.effectName, enhancedEffectInstance));
            System.Console.WriteLine("Playing: " + sound.effectName);
        }

        public void PlaySound(string name, float volume = 0.1f, float pitch = 0.0f, float pan = 0.0f, Boolean isLooping = false)
        {
            if ((tail + 1) % MAX_PENDING == head) //collision
            {
                throw new Exception();
            }
            pending[tail].SetValues(name, volume, pan, pitch, isLooping);
            tail = (tail + 1) % MAX_PENDING;
            if (tail == 0)
                RolledAround = true;
        }

        public void StopSound(string name)
        {
            foreach (EnhancedSoundEffectInstance ei in effectInstances.Where(ei => ei.Key == name).Select(ei => ei.Value).ToList())
            {
                ei.Stop();
            }
        }

        private void SetInstanceValues(PlaySoundEvent sound, SoundEffectInstance effectInstance)
        {
            effectInstance.Volume = sound.volume;
            effectInstance.Pitch = sound.pitch;
            effectInstance.Pan = sound.pan;
            effectInstance.IsLooped = sound.isLooping;
        }

        public void MuteAudio()
        {
            IsMuted = true;
            foreach (EnhancedSoundEffectInstance ei in effectInstances.Where(ei => ei.Value.State != SoundState.Stopped).Select(ei => ei.Value).ToList())
            {
                ei.Mute();
            }
        }

        public void UnmuteAudio()
        {
            IsMuted = false;
            foreach (EnhancedSoundEffectInstance ei in effectInstances.Where(ei => ei.Value.State != SoundState.Stopped).Select(ei => ei.Value).ToList())
            {
                ei.Unmute();
            }
        }
    }
}
