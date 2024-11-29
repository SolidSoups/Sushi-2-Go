using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AudioScripts
{
    public class AudioManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private UI_OptionCanvas _optionCanvas;
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
                        sound.source.volume = sound.volume * _optionCanvas.MusicLevel; 
                        break;
                    case SoundType.VFX:
                        sound.source.volume = sound.volume * _optionCanvas.SFXLevel;
                        break;
                    case SoundType.BACKGROUND:
                        sound.source.volume = sound.volume * _optionCanvas.NoiseLevel; 
                        break;
                }
            }

            Save();
        }

        public void Load()
        {
            Debug.Log($"_optionCanvas: {!!_optionCanvas}");
            _optionCanvas.SetLevels(
                PlayerPrefs.GetFloat("MusicVolume"),
                PlayerPrefs.GetFloat("VFXVolume"),
                PlayerPrefs.GetFloat("BackgroundVolume"));
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("MusicVolume", _optionCanvas.MusicLevel);
            PlayerPrefs.SetFloat("VFXVolume", _optionCanvas.SFXLevel);
            PlayerPrefs.SetFloat("BackgroundVolume", _optionCanvas.NoiseLevel);
            PlayerPrefs.Save();
        }
    }
}           
