using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UI_PosterIntroCanvas : MonoBehaviour
    {
        [FormerlySerializedAs("_imagesInOrder")]
        [Header("References")] 
        [SerializeField] private RawImage[] imagesInOrder;

        [FormerlySerializedAs("_fadeFrom")]
        [Header("Settings")] 
        [SerializeField][Range(0f,1f)] private float fadeFrom = 1;
        [SerializeField][Range(0f,1f)] private float fadeTo = 1;
        [SerializeField] private float fadeTimeSeconds = 1;
        [SerializeField] private float pauseTimeSeconds = 1;
    
        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            foreach (RawImage image in imagesInOrder)
            {
               Color imageColor = image.color;
               imageColor.a = fadeFrom;
               image.color = imageColor;
            }
        }

        public void StartFadeInIntro(Action OnFinished = null)
        {
            Reset();
            StartCoroutine(FadeIn(OnFinished));
        }
    

        private IEnumerator FadeIn(Action OnFinished = null)
        {
            foreach (RawImage image in imagesInOrder)
            {
                yield return new WaitUntil (() =>
                {
                    Color panelColor = image.color; 
                    panelColor.a += fadeFrom + ((fadeTo - fadeFrom) / fadeTimeSeconds)*Time.deltaTime;
                    if (panelColor.a >= fadeTo)
                    {
                        panelColor.a = fadeTo;
                        image.color = panelColor;
                        return true;
                    }
                    image.color = panelColor;
                    return false;
                });
                yield return new WaitForSeconds(pauseTimeSeconds);
            }
            OnFinished?.Invoke();
        }
    }
}
