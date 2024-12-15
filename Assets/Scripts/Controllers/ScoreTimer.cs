using UnityEngine;

namespace Controllers
{
    public class ScoreTimer
    {
        public float TimePassed { get; private set; }

        public int Score
        {
            get
            {
                return (int)(TimePassed * ScorePerSecond / 10f)*10;
            }
        }

        public int ScorePerSecond { get; set; }

        public ScoreTimer()
        {
            Initialize();
        }

        public void Initialize() => TimePassed = 0;

        public void Tick()
        {
            TimePassed += Time.deltaTime;
        }
    }
}
