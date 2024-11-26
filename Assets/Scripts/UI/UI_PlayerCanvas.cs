using Controllers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _countDownText;
        [SerializeField] private GameObject _countDownObj;

        public void SetHighScore(int score) => _scoreText.text = "HIGHSCORE: " + score.ToString();

        public void HighScoreUpdate(int score)
        {
            //Checks if there are a highscore
            if (PlayerPrefs.HasKey("SavedHighScore"))
            {
                //If the new score is higher then the saved one
                if(score > PlayerPrefs.GetInt("SavedHighScore"))
                {
                    //Sets the new highscore
                    PlayerPrefs.SetInt("SavedHighScore", score);
                }   
            }
            else
            {
                //if there is no highscore. set it
                PlayerPrefs.SetInt("SavedHighScore", score);
            }
        
            PlayerPrefs.Save();
            //updating the TMP
            _finalScoreText.text = score.ToString();
            _highScoreText.text =  PlayerPrefs.GetInt("SavedHighScore").ToString();
        }
    }
}