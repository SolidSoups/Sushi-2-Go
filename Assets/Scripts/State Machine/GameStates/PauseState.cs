using System;
using System.Collections;
using Controllers;
using TMPro;
using UnityEngine;

namespace State_Machine.GameStates
{
    public class PauseState : State
    {
        [Header("References")]
        [SerializeField] private UI_Controller _uiController;

        private void Awake()
        {
            _uiController.PauseMenu.OnResumeToGame = () => GameManager.Instance.SwitchState<PlayingState>();
        }

        public override void EnterState()
        {
            base.EnterState();
            _uiController.PauseMenu.EnablePauseMenu();
            Time.timeScale = 0f;
        }

        public override void UpdateState()
        {
        
            base.UpdateState();

            if (Input.GetKeyDown(KeyCode.Escape))
                _uiController.PauseMenu.OnPressedEscape();
        }
    }
}
