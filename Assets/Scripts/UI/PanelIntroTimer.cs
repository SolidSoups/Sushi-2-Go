using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PanelIntroTimer : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private RawImage _panel1;
        [SerializeField] private RawImage _panel2;
        [SerializeField] private RawImage _panel3;
        [SerializeField] private Image _blackPanel;

        [Header("Settings")] 
        [SerializeField][Range(0f,1f)] private float _fadeFrom;
        [SerializeField][Range(0f,1f)] private float _fadeTo;
        [SerializeField] private float _fadeTimeSeconds;
        [SerializeField] private float _pauseTimeSeconds;
    

        private bool _isFinished;
        public bool IsFinished => _isFinished; 
    
        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            Color _p1Color = _panel1.color;
            Color _p2Color = _panel2.color;
            Color _p3Color = _panel3.color;
            _p1Color.a = _fadeFrom;
            _p2Color.a = _fadeFrom;
            _p3Color.a = _fadeFrom;
            _panel1.color = _p1Color;
            _panel2.color = _p2Color;
            _panel3.color = _p3Color;
        }

        public void StartFadeInIntro()
        {
            _isFinished = false;
            Reset();
            StartCoroutine(FadeIn());
        }
    

        private IEnumerator FadeIn()
        {
            yield return new WaitUntil(() => FadePanel(_panel1));
            yield return new WaitForSeconds(_pauseTimeSeconds);
            yield return new WaitUntil(() => FadePanel(_panel2));
            yield return new WaitForSeconds(_pauseTimeSeconds);
            yield return new WaitUntil(() => FadePanel(_panel3));
            yield return new WaitForSeconds(_pauseTimeSeconds);
            _isFinished = true;
        }


        private bool FadePanel(RawImage panel)
        {
            Color panelColor = panel.color; 
            panelColor.a += _fadeFrom + ((_fadeTo - _fadeFrom) / _fadeTimeSeconds)*Time.deltaTime;
            if (panelColor.a >= _fadeTo)
            {
                panelColor.a = _fadeTo;
                panel.color = panelColor;
                return true;
            }
            panel.color = panelColor;
            return false;
        }
    }
}
