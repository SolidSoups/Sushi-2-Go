using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _countDownText;
    [SerializeField] private GameObject _countDownObj;
    [SerializeField] private TimerScore _timerScore;

    public void SetHighScore(int score) => _scoreText.text = "HIGHSCORE: " + score.ToString();

    public void HighScoreUpdate()
    {
        //Checks if there are a highscore
        if (PlayerPrefs.HasKey("SavedHighScore"))
        {
            //If the new score is higher then the saved one
            if(_timerScore.Score > PlayerPrefs.GetInt("SavedHighScore"))
            {
                //Sets the new highscore
                PlayerPrefs.SetInt("SavedHighScore", _timerScore.Score);
            }   
        }
        else
        {
            //if there is no highscore. set it
            PlayerPrefs.SetInt("SavedHighScore", _timerScore.Score);
        }
        
        PlayerPrefs.Save();
        //updating the TMP
        _finalScoreText.text = _timerScore.Score.ToString();
        _highScoreText.text =  PlayerPrefs.GetInt("SavedHighScore").ToString();
    }
}