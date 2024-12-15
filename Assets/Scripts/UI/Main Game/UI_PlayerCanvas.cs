using System;
using Controllers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        public void SetHighScore(int score) => _scoreText.text = score.ToString();
    }
}