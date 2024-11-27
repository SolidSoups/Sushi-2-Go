using Controllers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _countDownText;
        [SerializeField] private GameObject _countDownObj;

        public void SetHighScore(int score) => _scoreText.text = "HIGHSCORE: " + score.ToString();
    }
}