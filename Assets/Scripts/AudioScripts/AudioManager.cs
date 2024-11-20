using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AudioScripts
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider VFXSlider;
        [SerializeField] Slider BackgroundSlider;

        [FormerlySerializedAs("sound")] public SoundsScript[] _sounds;

        private void Awake()
        {
            foreach(SoundsScript s in _sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clips;

                s.source.volume = s.volume;
                s.source.pitch = s.Pitch;
                s.source.loop = s.loopSound;
            }

            if (!PlayerPrefs.HasKey("MusicVolume"))
            {
                ResetSliders();
            }
            else
            {
                Load();
            }
        }

        public void ResetSliders()
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.SetFloat("VFXVolume", 1);
            PlayerPrefs.SetFloat("BackgroundVolume", 1);
            Load();
        }

        public void Play(Component sender, object data)
        {
            if (data is not string)
                return;

            SoundsScript s = Array.Find(_sounds, SoundsScript => SoundsScript.name == (string)data);
            if (s == null)
            {
                Debug.LogError("No sound file, check file name");
                return;
            }

            s.source.Play();
        }

        public void Stop(Component sender, object data)
        {
            if (data is not string)
                return;
        
            SoundsScript s = Array.Find(_sounds, SoundsScript => SoundsScript.name == (string)data);
            if (s == null)
            {
                Debug.LogError("No sound file, check file name");
                return;
            }

            s.source.Stop();
        }

        public void OnChangedVolume()
        {
            //AudioListener.volume = musicSlider.value;
            if(_sounds == null)
            {
                Debug.LogError("Sound array is null");
                return;
            }
        
            foreach(SoundsScript sound in _sounds)
            {
                if (sound == null )
                    continue;

                switch (sound.type)
                {
                    case SoundType.MUSIC:
                        sound.source.volume = sound.volume * musicSlider.value; 
                        break;
                    case SoundType.VFX:
                        sound.source.volume = sound.volume * VFXSlider.value; 
                        break;
                    case SoundType.BACKGROUND:
                        sound.source.volume = sound.volume * BackgroundSlider.value; 
                        break;
                }
            }

            Save();
        }

        public void Load()
        {
            float musicvol = (float)PlayerPrefs.GetFloat("MusicVolume");
            float vfxvol = (float)PlayerPrefs.GetFloat("VFXVolume");
            float bvol = (float)PlayerPrefs.GetFloat("BackgroundVolume");
            Debug.Log($"Music Volume: {musicvol}, VFX: {vfxvol}, Background: {bvol}");
            musicSlider.value = musicvol;
            VFXSlider.value = vfxvol;
            BackgroundSlider.value = bvol;

        }

        private void Save()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
            PlayerPrefs.SetFloat("VFXVolume", VFXSlider.value);
            PlayerPrefs.SetFloat("BackgroundVolume", BackgroundSlider.value);
            PlayerPrefs.Save();
        }
    }
}           
