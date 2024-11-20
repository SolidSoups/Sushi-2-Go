using UnityEngine;

namespace Controllers
{
    public class ScoreTimer
    {
        public float Time { get; private set; }

        public int Score
        {
            get
            {
                return ((int)((Time * ScorePerSecond) / 10f))*10;
            }
        }

        public int ScorePerSecond { get; set; }        

        public void Initialize() => Time = 0;

        public void Tick()
        {
            Time += UnityEngine.Time.deltaTime;
        }
    }
}
