namespace Controllers
{
    public interface IControllable
    {
        public void Initialize();
        public void DoUpdate();

        public void DoFixedUpdate();
    }
}