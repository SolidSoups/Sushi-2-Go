using State_Machine;
using State_Machine.GameStates;

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
            SwitchState<IntroState>();
        }

        private void Update()
        {
            UpdateStateMachine();
        }


        private void FixedUpdate()
        {
            FixedUpdateStateMachine();
        }
    }
}
