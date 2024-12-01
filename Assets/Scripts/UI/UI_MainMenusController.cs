using System;
using System.Collections.Generic;
using AudioScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UI_MainMenusController : MonoBehaviour
    {
        [Header("Menus")]
        public UI_MainMenu MainMenu;
        public UI_MainMenuOptions OptionsMenu;
        public UI_MainMenuCredits CreditsMenu;
        public UI_MainMenuControls ControlsMenu;

        private readonly Stack<GameObject> _canvasStack = new();
        private void AddCanvasToStack(GameObject canvas)
        {
            canvas.SetActive(true);
            _canvasStack.Push(canvas);
        }
        public void RemoveLatestFromStack()
        {
            if (_canvasStack.Count == 0) return;
            _canvasStack.Pop().SetActive(false);
        }

        private void Awake()
        {
            OptionsMenu.gameObject.SetActive(false);
            CreditsMenu.gameObject.SetActive(false);
            ControlsMenu.gameObject.SetActive(false);
            MainMenu.OnOptionsButtonPressed = () => AddCanvasToStack(OptionsMenu.gameObject);
            
            OptionsMenu.OnCreditsButtonPressed = () => AddCanvasToStack(CreditsMenu.gameObject);
            OptionsMenu.OnControlsButtonPressed = () => AddCanvasToStack(ControlsMenu.gameObject);
            
            OptionsMenu.OnCloseButtonPressed = RemoveLatestFromStack;
            ControlsMenu.OnCloseButtonPressed = RemoveLatestFromStack;
            CreditsMenu.OnCloseButtonPressed = RemoveLatestFromStack;
        }

        
    }
}

