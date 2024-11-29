using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_FadeToBlackCanvas : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Image _blackPanel;
        [SerializeField] private Canvas _canvas;

        [Header("Settings")] 
        [SerializeField] private float _fadeToBlackSeconds;
        [SerializeField] private float _delayAfterFinishSeconds;

        private void Awake()
        {
            _canvas.enabled = false;
            ZeroImageAlpha();
        }

        public void StartFade(Action OnFinished = null)
        {
            _canvas.enabled = true;
            ZeroImageAlpha();
            StartCoroutine(FadeToBlack(OnFinished));
        }
        private IEnumerator FadeToBlack(Action OnFinished = null)
        {
            yield return new WaitUntil(() =>
            {
                Color panelColor = _blackPanel.color; 
                panelColor.a += 1f / _fadeToBlackSeconds*Time.deltaTime;
                if (panelColor.a >= 1f)
                {
                    panelColor.a = 1f;
                    _blackPanel.color = panelColor;
                    return true;
                }
                _blackPanel.color = panelColor;
                return false;
            });
            yield return new WaitForSecondsRealtime(_delayAfterFinishSeconds);
            OnFinished?.Invoke();
        }
        
        public void ZeroImageAlpha()
        {
            Color color =  _blackPanel.color;
            color.a = 0;
            _blackPanel.color = color;
        }

        public void Reset()
        {
            ZeroImageAlpha();
            _canvas.enabled = false;
        }
    }
}
