using UnityEngine;

namespace UI
{
  public interface  IOptionsMenu
  {
    public float MusicLevel { get; }
    public float SFXLevel { get; }
    public float NoiseLevel { get; }

    public void SetLevels(float musicLevel, float sfxLevel, float noiseLevel);
  }
}