using State_Machine;
using State_Machine.GameStates;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class GameManager : StateMachine
    {
        public static GameManager Instance;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SwitchState<SetupState>();
        }

        private void Update()
        {
            UpdateStateMachine();
        }


        private void FixedUpdate()
        {
            FixedUpdateStateMachine();
        }

        public void ResetScene() => SceneManager.LoadScene(1);
        public void MainMenuScene() => SceneManager.LoadScene(0);
    }
}
