using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using System.Collections;
using TMPro;
using UnityEngine;

public class PauseState : State
{
    [SerializeField] private TextMeshProUGUI _countDownText;
    [SerializeField] private GameObject _countDownObj;
    [SerializeField] private UI_Player _uiPlayer;
    public GameObject PauseCanvas;
    public GameObject OptionsCanvas;

    //..

    public override void EnterState()
    {
        base.EnterState();
        PauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        timing = false;
    }

    public override void UpdateState()
    {
        
        base.UpdateState();

        if (!timing && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            UnPause();  
        }
    }

    public void UnPause()
    {
        PauseCanvas.SetActive(false);
        OptionsCanvas.SetActive(false);
        
        StartCoroutine(Timer());
    }

    public override void ExitState()
    {
        base.ExitState();

        StopAllCoroutines();

    }

    private bool timing = false;
    private IEnumerator Timer()
    {
        timing = true;
        _countDownObj.SetActive(true);

        _countDownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        _countDownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        _countDownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        _countDownObj.SetActive(false);
        
        GameManager.Instance.SwitchState<PlayingState>();
    }
}
