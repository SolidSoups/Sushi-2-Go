using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlackCanvas : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Image _blackPanel;

    [Header("Settings")] 
    [SerializeField] private float _fadeToBlackSeconds;

    [SerializeField] private float _delayAfterFinishSeconds;

    private bool _isFinished;
    public bool IsFinished => _isFinished; 
    
    private void Awake()
    {
        Color color =  _blackPanel.color;
        color.a = 0;
        _blackPanel.color = color;
    }

    public void StartFade()
    {
        _isFinished = false;
        Color color =  _blackPanel.color;
        color.a = 0;
        _blackPanel.color = color;
        StartCoroutine(FadeToBlack());
    }
    

    private IEnumerator FadeToBlack()
    {
        yield return new WaitUntil(() => Fade());
        yield return new WaitForSecondsRealtime(_delayAfterFinishSeconds);
        _isFinished = true;
    }

    private bool Fade()
    {
       Color panelColor = _blackPanel.color; 
       panelColor.a += (1f / _fadeToBlackSeconds)*Time.deltaTime;
       if (panelColor.a >= 1f)
       {
           panelColor.a = 1f;
           _blackPanel.color = panelColor;
           return true;
       }
       _blackPanel.color = panelColor;
       return false;
    }
}
